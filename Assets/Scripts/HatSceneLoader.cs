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
    private AudioSource _audio;

    private void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        _interactable.selectEntered.AddListener(OnHatSelected);
        _audio = GetComponent<AudioSource>();

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
        if (_audio != null) _audio.Play();
        Debug.Log("[HatSceneLoader] Hat selected — fading to: " + targetSceneName);
        SceneFader.Instance.FadeToScene(targetSceneName);
    }
}
