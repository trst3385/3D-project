using UnityEngine;

[CreateAssetMenu(fileName = "New Round Data", menuName = "ScriptableObject/Round Data")]


public class RoundData : ScriptableObject
{

    [Header("Weak 러시 몬스터 설정")]
    public GameObject WeaknemyPrefab; //공격 안 하는 빠른 다수 몬스터 프리팹
    public EnemyData WeakEnemyData;   //러시 몬스터 데이터

    [Header("Weak, Normal 몬스터 등장 수, 시간")]
    public int enemyCount;             //이번 라운드 목표 처치 수
    public float spawnInterval;        //이번 라운드 스폰 속도

    [Header("Weak,Normal 몬스터 랜덤 등장 확률")]
    [Range(0f, 1f)]//EnemySpawner의 SpawnEnemy()에서 Random.value(0~1)와 맞추기 위해 인스펙터 입력 범위를 0~1 슬라이더로 제한
    public float WeakSpawnChance = 0.8f;//예: 0.8이면 80% 확률로 weak 몬스터, 20% 확률로 normal 몬스터

    [Header("Normal 몬스터 설정")]
    public GameObject enemyPrefab;     //이번 라운드에 등장할 몬스터
    public EnemyData enemyData;        //enemyData 참조

    [Header("Boss 몬스터 설정")]
    public GameObject bossPrefab; //보스가 있다면 연결
    public EnemyData bossData;    //enemyData 참조
}
