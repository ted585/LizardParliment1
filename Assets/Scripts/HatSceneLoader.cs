using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class HatSceneLoader : MonoBehaviour
{
    [Tooltip("Exact name of the scene to load (must be in Build Settings)")]
    public string targetSceneName = "lizard zone";

    private XRSimpleInteractable _interactable;

    private void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        _interactable.selectEntered.AddListener(OnHatSelected);

        // Ensure SceneFader exists in the scene
        if (SceneFader.Instance == null)
        {
            var faderGO = new GameObject("SceneFader");
            faderGO.AddComponent<SceneFader>();
        }
    }

    private void OnDestroy()
    {
        if (_interactable != null)
            _interactable.selectEntered.RemoveListener(OnHatSelected);
    }

    private void OnHatSelected(SelectEnterEventArgs args)
    {
        Debug.Log("[HatSceneLoader] Hat selected — fading to scene: " + targetSceneName);
        SceneFader.Instance.FadeToScene(targetSceneName);
    }
}
