using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Achievements/Achievement")]
public class AchievementData : ScriptableObject
{
    public string Id; // 아이디
    public string Name; // 이름
    public string Description; // 설명
    public int TargetValue; // 예: 목표 점수
}