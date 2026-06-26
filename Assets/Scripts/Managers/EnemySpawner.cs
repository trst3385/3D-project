using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("몬스터 이동 방향 설정")]
    public Transform spawnPoint;//Waypoint 1번 위치(지금은 인스펙터 연결로)


    private bool isBossSpawned = false; //보스 몬스터 중복 소환 방지

    void Start()
    {
        RoundData data = GameManager.Instance.currentRoundData;//GameManager에 연결된 SO값 받아옴
        Debug.Log("이번 라운드 몬스터: " + data.enemyPrefab.name);
        Debug.Log("스폰 간격: " + data.spawnInterval);

        GameManager.Instance.OnBossSpawn += SpawnBoss;//보스 소환 이벤트 구독
        StartCoroutine(SpawnRoutine());//반복해서 적을 생성하는 코루틴 시작
    }

    private void SpawnBoss()//보스 생성
    {
        RoundData currentData = GameManager.Instance.currentRoundData;//GameManager가 가진 현재 RoundData SO에서 값을 꺼내옴

        if (currentData.bossPrefab == null)
        {
            return;
        }
        if (isBossSpawned)
        {
            return;
        }

        isBossSpawned = true;

        GameObject bossObj = Instantiate(currentData.bossPrefab, spawnPoint.position, Quaternion.identity);//SO에 들어있는 보스 프리팹 사용
        EnemyHealth bossHealth = bossObj.GetComponent<EnemyHealth>();

        if (GameManager.Instance != null && bossHealth != null)
        {
            GameManager.Instance.RegisterEnemy(bossHealth);
        }
    }

    private void SpawnEnemy()
    {
        RoundData currentData = GameManager.Instance.currentRoundData;//GameManager가 가진 현재 RoundData SO에서 값을 꺼내옴

        if (currentData.enemyPrefab != null && spawnPoint != null)
        {   //Waypoint 1번 위치에, 회전값 없이 몬스터 생성
            GameObject enemyObj = Instantiate(currentData.enemyPrefab, spawnPoint.position, Quaternion.identity);

            EnemyHealth enemyHealth = enemyObj.GetComponent<EnemyHealth>();//몬스터의 EnemyHealth 컴포넌트를 가져옴

            //GameManager에게 등록 (이 과정이 있어야 GameManager가 사망을 인지)
            if (GameManager.Instance != null && enemyHealth != null)
            {
                GameManager.Instance.RegisterEnemy(enemyHealth);
            }
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (!isBossSpawned)// 보스가 등장하기 전까지는 계속 일반 몬스터 생성
        {
            SpawnEnemy();

            float waitTime = GameManager.Instance.currentRoundData.spawnInterval;//SO에 적힌 시간만큼 몬스터 생성
            yield return new WaitForSeconds(waitTime);
        }
    }

}