using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("설정")]
    public GameObject bossPrefab;       //보스 프리팹 연결
    public GameObject enemyPrefab;      //생성할 적 프리팹
    public Transform spawnPoint;        //Waypoint 1번 위치(지금은 인스펙터 연결로)
    public float spawnInterval = 3f;    //스폰 간격 (초)

    private bool isBossSpawned = false; //보스 몬스터 중복 소환 방지

    void Start()
    {
        GameManager.Instance.OnBossSpawn += SpawnBoss;//보스 소환 이벤트 구독
        StartCoroutine(SpawnRoutine());//반복해서 적을 생성하는 코루틴 시작
    }

    private void SpawnBoss()//보스 생성
    {
        if (isBossSpawned)
        {
            return;
        }
        isBossSpawned = true;

        GameObject bossObj = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
        EnemyHealth bossHealth = bossObj.GetComponent<EnemyHealth>();

        if (GameManager.Instance != null && bossHealth != null)
        {
            GameManager.Instance.RegisterEnemy(bossHealth);//보스도 등록
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (!isBossSpawned)// 보스가 등장하기 전까지는 계속 일반 몬스터 생성
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {   //Waypoint 1번 위치에, 회전값 없이 적 생성
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            EnemyHealth enemyHealth = enemyObj.GetComponent<EnemyHealth>();//몬스터의 EnemyHealth 컴포넌트를 가져옴

            //GameManager에게 등록 (이 과정이 있어야 GameManager가 사망을 인지)
            if (GameManager.Instance != null && enemyHealth != null)
            {
                GameManager.Instance.RegisterEnemy(enemyHealth);
            }
        }
    }

   
}