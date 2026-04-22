using UnityEngine;

public class DiscoLights : MonoBehaviour
{
    public float changesPerSecond = 2.5f;
    public float lerpSpeed = 8f;

    private Light[]    _lights;
    private Material[] _bulbs;
    private Color[]    _current;
    private Color[]    _target;
    private float[]    _timers;

    private static readonly Color[] Palette = {
        Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow,
        new Color(1f,0.4f,0f), new Color(0.6f,0f,1f), new Color(0f,1f,0.5f), new Color(1f,0.1f,0.6f)
    };

    void Start()
    {
        var lights = new System.Collections.Generic.List<Light>();
        var bulbs  = new System.Collections.Generic.List<Material>();

        foreach (Transform child in transform)
        {
            if (!child.name.StartsWith("CeilingLight_")) continue;
            var l = child.GetComponent<Light>();
            if (l != null) lights.Add(l);

            var bulbT = child.Find(child.name + "_Bulb");
            if (bulbT != null)
            {
                var rend = bulbT.GetComponent<Renderer>();
                if (rend != null) { var m = new Material(rend.sharedMaterial); rend.material = m; bulbs.Add(m); }
                else bulbs.Add(null);
            }
            else bulbs.Add(null);
        }

        int n = lights.Count;
        _lights  = lights.ToArray();
        _bulbs   = bulbs.ToArray();
        _current = new Color[n];
        _target  = new Color[n];
        _timers  = new float[n];
        float interval = 1f / changesPerSecond;

        for (int i = 0; i < n; i++)
        {
            _current[i] = Pick();
            _target[i]  = Pick();
            _timers[i]  = interval * i / n;
            Apply(i, _current[i]);
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        float interval = 1f / changesPerSecond;
        for (int i = 0; i < _lights.Length; i++)
        {
            _timers[i] -= dt;
            if (_timers[i] <= 0f) { _timers[i] = interval; _target[i] = Pick(); }
            _current[i] = Color.Lerp(_current[i], _target[i], lerpSpeed * dt);
            Apply(i, _current[i]);
        }
    }

    void Apply(int i, Color c)
    {
        if (i < _lights.Length && _lights[i] != null) _lights[i].color = c;
        if (i < _bulbs.Length && _bulbs[i] != null)
        {
            _bulbs[i].SetColor("_BaseColor", c);
            _bulbs[i].SetColor("_EmissionColor", c * 2f);
        }
    }

    Color Pick() { return Palette[Random.Range(0, Palette.Length)]; }
}
