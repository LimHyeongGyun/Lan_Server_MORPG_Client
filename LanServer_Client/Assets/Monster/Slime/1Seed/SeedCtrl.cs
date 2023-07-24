public sealed class SeedCtrl : ISlimeController
{
	[UnityEngine.Range(0f, 1f)] public float prob;

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (UnityEngine.Random.Range(0, 1f) < prob)
		{
			IncreaseHP(-1f);
		}
	}

	/*
	private new void OnEnable()
	{
		base.OnEnable();
		ChangeState(StateEnum.DEBUG);
	}

	private new void FixedUpdate()
	{
		base.FixedUpdate();
		switch (this.State)
		{
			case StateEnum.DEBUG:
				if (this.target == null)
				{
					this.target = FindObjectOfType<Player>(true);
				}
				if (this.target != null)
				{
					_agent.destination = this.target.transform.position;
				}
				break;
		}
	}
	*/
}