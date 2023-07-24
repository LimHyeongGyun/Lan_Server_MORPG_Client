public class MONSTER_SETTINGS
{
	/// <summary>
	/// 몬스터 스포너의 생성에 대한 방식을 설정합니다.
	/// [1] 쿨타임이 끝나면 한번에 생성한다.
	/// [2] 속도 * 시간 변화량을 누적시킨 후 그 양이 1 이상이면 1 미만이 될 때 까지 수치를 1씩 감소시켜 1마리씩 생성시킨다.
	/// </summary>
	public const int SPAWN = 2;

	/// <summary>
	/// 몬스터의 얼굴(표정) 변화 여부를 설정합니다.
	/// [false] 얼굴 표정은 변하지 않습니다.
	/// [true] 얼굴 표정은 변할 수 있습니다.
	/// </summary>
	public const bool FACE = true;

	/// <summary>
	/// 몬스터가 데미지를 입을 시 데미지 텍스트를 출력시킬 여부를 설정합니다.
	/// [false] 텍스트가 표시되지 않습니다.
	/// [true] 텍스트가 표시됩니다.
	/// </summary>
	public const bool DMG = true;
}