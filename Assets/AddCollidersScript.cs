using UnityEngine;
using UnityEditor;

public class AddCollidersScript
{
    public static void Execute()
    {
        GameObject root = GameObject.Find("finished_commons");
        if (root == null)
        {
            Debug.LogError("Could not find finished_commons");
            return;
        }

        int count = 0;
        MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.gameObject.GetComponent<Collider>() == null)
            {
                MeshCollider collider = renderer.gameObject.AddComponent<MeshCollider>();
                count++;
            }
        }

        Debug.Log($"Added {count} MeshColliders to finished_commons.");
    }
}