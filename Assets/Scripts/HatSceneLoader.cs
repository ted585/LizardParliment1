using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Attach to the tin_foil_hat. When the player selects (grabs/activates) the hat,
/// they are transported to the Lizard Zone scene.
/// </summary>
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
    }

    private void OnDestroy()
    {
        if (_interactable != null)
            _interactable.selectEntered.RemoveListener(OnHatSelected);
    }

    private void OnHatSelected(SelectEnterEventArgs args)
    {
        Debug.Log("[HatSceneLoader] Hat selected — loading scene: " + targetSceneName);
        SceneManager.LoadScene(targetSceneName);
    }
}
