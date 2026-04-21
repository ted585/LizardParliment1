using UnityEngine;
using UnityEngine.XR.Management;

/// <summary>
/// Attach to the XR Device Simulator GameObject.
/// Disables it automatically when a real XR headset is connected,
/// so it only runs when testing in the editor without a headset.
/// </summary>
public class XRSimulatorAutoDisable : MonoBehaviour
{
    private void Awake()
    {
        bool realHMDPresent = false;

        // Check if XR is initialised and has a real display subsystem running
        var xrManager = XRGeneralSettings.Instance?.Manager;
        if (xrManager != null && xrManager.activeLoader != null)
        {
            var displaySubsystems = new System.Collections.Generic.List<UnityEngine.XR.XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displaySubsystems);
            foreach (var sub in displaySubsystems)
            {
                if (sub.running)
                {
                    realHMDPresent = true;
                    break;
                }
            }
        }

        if (realHMDPresent)
        {
            Debug.Log("[XRSimulatorAutoDisable] Real HMD detected — disabling XR Device Simulator.");
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("[XRSimulatorAutoDisable] No real HMD detected — XR Device Simulator remains active.");
        }
    }
}
