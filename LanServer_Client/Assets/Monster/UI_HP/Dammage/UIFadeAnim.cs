#pragma warning disable IDE0051 // for Unity Magic Methods (ex: Update)
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public sealed class UIFadeAnim : MonoBehaviour
{
    [SerializeField] private AnimationCurve anim = default;
    [SerializeField] private float duration = default;
    private float m_elapsed;
    private Graphic m_graphic;

    private void Awake()
    {
        m_graphic = GetComponent<Graphic>();
    }

    private void OnEnable()
    {
        m_elapsed = 0f;
    }

    private void LateUpdate()
    {
        m_elapsed += Time.deltaTime;
        if (m_elapsed >= duration)
        {
            Destroy(this.gameObject);
            return;
        }
        float alpha = anim.Evaluate(m_elapsed / duration);
        Color c = m_graphic.color;
        c.a = alpha;
        m_graphic.color = c;
    }
}