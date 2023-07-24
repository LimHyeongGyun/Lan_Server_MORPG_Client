using UnityEngine;

public class Player1 : MonoBehaviour
{
	protected float _HP = 65536;
	public float HP => _HP;
	protected bool _isDead = false;
	public bool IsDead => _isDead;

	private void GetDmg(float dmg)
	{
		if (!_isDead)
		{
			float tmp = _HP;
			_HP -= dmg;
			if (_HP <= 0)
			{
				_HP = 0f;
				_isDead = true;
			}
			Debug.Log(string.Format("플레이어 {0}: HP -= {1}({2})", this.name, tmp - _HP, dmg));
			if (_isDead)
			{
				Debug.Log(string.Format("플레이어 {0}: 사망", this.name));
			}
		}
	}
	public void IncreaseHP(float amount)
	{
		if (amount < 0)
		{
			GetDmg(-amount);
		}
	}
}