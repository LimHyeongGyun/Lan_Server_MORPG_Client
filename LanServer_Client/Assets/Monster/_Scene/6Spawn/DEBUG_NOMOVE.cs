using UnityEngine;

public class DEBUG_NOMOVE : ISlimeController
{
	[Range(0f, 1f)] public float prob;

	private new void OnEnable()
	{
		base.OnEnable();
		ChangeState((ISlimeController.StateEnum)255);
	}

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
		if (Random.Range(0f, 1f) < prob)
		{
			IncreaseHP(-this.stat.maxHP * 0.1f);
		}
    }
}