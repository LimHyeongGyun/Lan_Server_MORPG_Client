#pragma warning disable IDE0051 // for Unity Magic Methods (ex: Update)
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Graphic))]
public sealed class UI_MATRIX_M : MonoBehaviour
{
    private Material _mat;

    /// <summary>
    /// Unity MonoBehaviour.Awake()<br/>
    /// 기본-초기화합니다.
    /// </summary>
    private void Awake()
    {
        var graphic = GetComponent<Graphic>();
        _mat = graphic.material;
    }

    /// <summary>
    /// Unity MonoBehaviour.LateUpdate()<br/>
    /// UI/Custom/LookAtCamera 셰이더에 대하여 _MATRIX_M과 _MATRIX_I_M의 값을 지정합니다.
    /// </summary>
    private void LateUpdate()
    {
        _mat.SetMatrix("_MATRIX_M", transform.localToWorldMatrix);
        _mat.SetMatrix("_MATRIX_I_M", transform.worldToLocalMatrix);
    }
}