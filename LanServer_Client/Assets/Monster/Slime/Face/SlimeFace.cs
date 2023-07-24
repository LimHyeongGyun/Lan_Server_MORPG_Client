#pragma warning disable IDE0044 // readonly
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Monster/Slime/Face", order = 1)]
public sealed class SlimeFace : ScriptableObject
{
	[SerializeField]
	private Texture Idle = default,
		Walk = default,
		Jump = default,
		Attack = default,
		Damage = default;

	public enum Type
	{
		Idle,
		Walk,
		Jump,
		Attack,
		Damage
	}
	public Texture GetTexture(Type type)
	{
		switch (type)
		{
			case Type.Idle:
				return Idle;
			case Type.Walk:
				return Walk;
			case Type.Jump:
				return Jump;
			case Type.Attack:
				return Attack;
			case Type.Damage:
				return Damage;
			default:
				Debug.LogError("SlimeFace: Invalid Type");
				return null;
		}
	}
}