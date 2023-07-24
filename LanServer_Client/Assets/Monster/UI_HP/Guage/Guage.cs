using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public sealed class Guage : MonoBehaviour
{
	private Material _mat;

	// Unity MonoBehaviour::MonoBehaviour.Awake()
	// 호출 순서 조정을 위해 유니티 매직 메서드 대신 직접 호출로 바꿈.
	// ISlimeController.Init_Stat() 참고
	public void Init()
	{
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		_mat = renderer.material;
	}

	public void SetValue(float ratio)
	{
		_mat.SetFloat("_Guage", ratio);
	}
}