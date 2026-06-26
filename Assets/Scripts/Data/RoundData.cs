using UnityEngine;

[CreateAssetMenu(fileName = "New Round Data", menuName = "ScriptableObject/Round Data")]


public class RoundData : ScriptableObject
{
    [Header("일반 몬스터 설정")]
    public GameObject enemyPrefab;     //이번 라운드에 등장할 몬스터
    public int enemyCount;             //이번 라운드 목표 처치 수
    public float spawnInterval;        //이번 라운드 스폰 속도

    [Header("보스 설정")]
    public GameObject bossPrefab; //보스가 있다면 연결
}
