using UnityEngine;

public class Scaler : MonoBehaviour
{
    private Vector3 scale_base;
    [SerializeField] private Vector3 scale = Vector3.one;

    private void Awake()
    {
        scale_base = transform.localScale;
    }

    private void LateUpdate()
    {
        Vector3 s = Vector3.Scale(scale_base, scale);
        transform.localScale = s;
    }
}