using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HatPromptUI : MonoBehaviour
{
    public float detectDistance = 12f;
    public float fadeSpeed = 8f;

    private CanvasGroup _group;
    private Canvas _canvas;
    private Camera _cam;

    void Awake()
    {
        _cam = Camera.main;
        var go = new GameObject("HatPromptCanvas");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.sortingOrder = 10;
        var rt = _canvas.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(260f, 50f);
        rt.localScale = Vector3.one * 0.004f;
        // Rotate 180 on Y so text faces the player correctly
        go.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        _group = go.AddComponent<CanvasGroup>();
        _group.alpha = 0f;
        _group.interactable = false;
        _group.blocksRaycasts = false;
        // Background
        var bg = new GameObject("BG");
        bg.transform.SetParent(go.transform, false);
        var img = bg.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.75f);
        img.raycastTarget = false;
        var bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        // Label
        var lGO = new GameObject("Label");
        lGO.transform.SetParent(go.transform, false);
        var label = lGO.AddComponent<TextMeshProUGUI>();
        label.text = "Put on tinfoil hat";
        label.fontSize = 24f;
        label.fontStyle = FontStyles.Bold;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        var lRT = label.GetComponent<RectTransform>();
        lRT.anchorMin = Vector2.zero;
        lRT.anchorMax = Vector2.one;
        lRT.offsetMin = new Vector2(10f, 4f);
        lRT.offsetMax = new Vector2(-10f, -4f);
    }

    void Update()
    {
        float target = 0f;
        if (_cam != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, detectDistance))
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                    target = 1f;
        }
        _group.alpha = Mathf.MoveTowards(_group.alpha, target, fadeSpeed * Time.deltaTime);
        // Billboard — face the camera but keep the 180 flip
        if (_canvas != null)
        {
            Vector3 dir = _canvas.transform.position - _cam.transform.position;
            if (dir != Vector3.zero)
                _canvas.transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
