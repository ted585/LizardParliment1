using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    public float fadeOutDuration = 1.2f;
    public float fadeInDuration  = 1.0f;

    private CanvasGroup _group;
    private Canvas      _canvas;
    private GameObject  _overlayGO;
    private Camera      _cam;
    private bool        _active = false;

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
        // Grab the new camera — keep trying in Update if not ready yet
        _cam = Camera.main;
    }

    // Track the camera every frame so the overlay always covers the view
    // regardless of whether the camera reference changed
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

        _canvas = _overlayGO.AddComponent<Canvas>();
        _canvas.renderMode   = RenderMode.WorldSpace;
        _canvas.sortingOrder = 999;

        var rt = _canvas.GetComponent<RectTransform>();
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

        _cam = Camera.main;
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

        // 1 — Fade to black
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

        // 2 — Async load, hold until fully loaded
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        // 3 — Activate scene
        op.allowSceneActivation = true;
        while (!op.isDone)
            yield return null;

        // 4 — Wait several frames for XR camera to fully initialise
        for (int i = 0; i < 8; i++)
            yield return null;

        // 5 — Re-acquire camera
        _cam = Camera.main;

        // 6 — Extra half-second pause so the scene is visually ready
        yield return new WaitForSeconds(0.3f);

        // 7 — Fade back in
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        _group.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        _group.alpha = to;
    }
}
