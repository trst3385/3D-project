using UnityEngine;

[CreateAssetMenu(fileName = "New GameGold Data", menuName = "ScriptableObject/GameGold")]

public class GameGold : ScriptableObject
{
    [Header("골드 설정")]
    public int startGold = 50;   //시작 골드
    public int treeCost = 25;    //나무당 비용
}
