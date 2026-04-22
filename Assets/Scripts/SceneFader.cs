using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [Header("Timing")]
    public float fadeOutDuration    = 1.2f;
    public float audioFadeOutDuration = 1.5f;
    public float fadeInDuration     = 1.0f;

    private CanvasGroup _group;
    private GameObject  _overlayGO;
    private Camera      _cam;
    private bool        _active = false;

    public AudioSource SceneMusic { get; set; }

    // ---------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildOverlay();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if (!_active) return;
        if (_cam == null) _cam = Camera.main;
        if (_cam == null) return;
        float z = Mathf.Max(_cam.nearClipPlane + 0.15f, 0.16f);
        _overlayGO.transform.position = _cam.transform.position + _cam.transform.forward * z;
        _overlayGO.transform.rotation = _cam.transform.rotation;
    }

    // ---------------------------------------------------------------

    private void BuildOverlay()
    {
        _overlayGO = new GameObject("FadeOverlay");
        _overlayGO.transform.SetParent(null);
        DontDestroyOnLoad(_overlayGO);

        var canvas = _overlayGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.WorldSpace;
        canvas.sortingOrder = 999;

        var rt = canvas.GetComponent<RectTransform>();
        rt.sizeDelta  = new Vector2(600f, 600f);
        rt.localScale = Vector3.one * 0.002f;

        var imgGO = new GameObject("BlackPanel");
        imgGO.transform.SetParent(_overlayGO.transform, false);
        var img = imgGO.AddComponent<Image>();
        img.color = Color.black;
        img.raycastTarget = false;
        var irt = img.GetComponent<RectTransform>();
        irt.anchorMin = Vector2.zero;
        irt.anchorMax = Vector2.one;
        irt.offsetMin = Vector2.zero;
        irt.offsetMax = Vector2.zero;

        _group = _overlayGO.AddComponent<CanvasGroup>();
        _group.alpha          = 0f;
        _group.interactable   = false;
        _group.blocksRaycasts = false;

        _cam    = Camera.main;
        _active = true;
    }

    // ---------------------------------------------------------------

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeRoutine(sceneName));
    }

    private IEnumerator FadeRoutine(string sceneName)
    {
        _active = true;

        AudioSource outgoingMusic = SceneMusic;

        // 1a — Fade music out independently over 1.5s (fire and forget)
        if (outgoingMusic != null)
            StartCoroutine(FadeOutAudio(outgoingMusic, audioFadeOutDuration));

        // 1b — Fade screen to black over 1.2s
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _group.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            yield return null;
        }
        _group.alpha = 1f;

        // 1c — Hold on black (hat sound plays, music finishes fading)
        yield return new WaitForSecondsRealtime(2f);

        if (outgoingMusic != null) outgoingMusic.Stop();
        SceneMusic = null;

        // 2 — Async load
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f) yield return null;
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        // 3 — Wait for XR + scene audio to initialise
        for (int i = 0; i < 8; i++) yield return null;
        _cam = Camera.main;
        yield return new WaitForSeconds(0.3f);

        // 4 — Fade screen and new music in together
        AudioSource incomingMusic = SceneMusic;
        float targetVol = incomingMusic != null ? incomingMusic.volume : 1f;
        if (incomingMusic != null)
        {
            incomingMusic.volume = 0f;
            incomingMusic.Play();
        }

        elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;
            _group.alpha = Mathf.Lerp(1f, 0f, t);
            if (incomingMusic != null)
                incomingMusic.volume = Mathf.Lerp(0f, targetVol, t);
            yield return null;
        }
        _group.alpha = 0f;
        if (incomingMusic != null) incomingMusic.volume = targetVol;
    }

    // Fades an AudioSource out over the given duration independently
    private IEnumerator FadeOutAudio(AudioSource source, float duration)
    {
        float startVol = source.volume;
        float elapsed  = 0f;
        while (elapsed < duration)
        {
            if (source == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }
        if (source != null) source.volume = 0f;
    }
}
