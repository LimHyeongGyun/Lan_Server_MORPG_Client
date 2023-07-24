using UnityEngine;

public class SlimeFaceManager : MonoBehaviour
{
    private Material mat0;
    private Material mat;
    private void Awake()
    {
        var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        mat0 = renderer.materials[0];
        mat = renderer.materials[1];
    }

    public SlimeFace face;
    private Texture baseTex;
    private Texture overrideTex;
    [HideInInspector] public float duration;
    private float elapsed;

    private Color overrideColor;
    [HideInInspector] public float duration2 = 1f;
    private float elapsed2;

    /// <summary>
    /// 베이스 표정을 지정한다.
    /// </summary>
    public Texture BaseTexture
    {
        get => this.baseTex;
        set
        {
            if (this.baseTex != value)
            {
                this.baseTex = value;
                if (this.overrideTex == null)
                {
                    mat.mainTexture = this.baseTex;
                }
            }
        }
    }
    /// <summary>
    /// 오버라이드 표정을 지정한다. null이 아니면 이 표정이 우선적으로 출력이 된다.
    /// </summary>
    public Texture OverrideTexture
    {
        get => this.overrideTex;
        set
        {
            if (this.overrideTex != value)
            {
                this.overrideTex = value;
                if (this.overrideTex != null)
                {
                    mat.mainTexture = this.overrideTex;
                    elapsed = 0f;
                }
                else
                {
                    mat.mainTexture = this.baseTex;
                }
            }
        }
    }

    private Color cachedHint = Color.white;
    public Color OverrideColor
    {
        get => this.overrideColor;
        set
        {
            if (this.overrideColor != value)
            {
                this.overrideColor = value;
                elapsed2 = 0f;
            }
        }
    }

    private readonly int id1 = Shader.PropertyToID("_HintColor");

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            OverrideTexture = null;
        }
        elapsed2 += Time.deltaTime;
        float ratio = elapsed2 / duration2;
        float cratio = Mathf.Clamp01(ratio);
        Color hint = Color.Lerp(overrideColor, Color.white, cratio);
        if (hint != cachedHint)
        {
            mat0.SetColor(id1, hint);
            mat.SetColor(id1, hint);
        }
    }
}