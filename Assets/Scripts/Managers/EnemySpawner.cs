using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("설정")]
    public GameObject enemyPrefab;      //생성할 적 프리팹
    public Transform spawnPoint;        //Waypoint 1번 위치(지금은 인스펙터 연결로)
    public float spawnInterval = 3f;    //스폰 간격 (초)

    void Start()
    {
        StartCoroutine(SpawnRoutine());//반복해서 적을 생성하는 코루틴 시작
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)//게임이 실행되는 동안 무한 반복
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            //Waypoint 1번 위치에, 회전값 없이 적 생성
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
