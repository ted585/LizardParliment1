using UnityEngine;

/// <summary>
/// Attach to an AudioSource in each scene.
/// Registers itself with SceneFader so music fades with the screen transition.
/// If SceneFader doesn't exist yet (scene loaded directly) it just plays normally.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SceneAudio : MonoBehaviour
{
    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();

        // Register with the fader — it will control play/volume during transitions
        if (SceneFader.Instance != null)
        {
            // Fader is present: let it start us (don't autoplay)
            _source.playOnAwake = false;
            SceneFader.Instance.SceneMusic = _source;

            // If this is the very first scene (no fade in progress), just play
            if (_source.isPlaying == false && SceneFader.Instance != null)
            {
                _source.volume = 1f;
                _source.Play();
            }
        }
        else
        {
            // No fader — standalone scene start, just play
            _source.volume = 1f;
            if (!_source.isPlaying) _source.Play();
        }
    }
}
