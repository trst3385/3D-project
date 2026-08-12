using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("몬스터 이동 방향 설정")]
    public Transform spawnPoint;//Waypoint 1번 위치에서 첫 등장(지금은 인스펙터 연결로)

    private bool isBossSpawned = false;//보스 몬스터 중복 소환 방지

    public GameObject waypointGroup;//웨이포인트 그룹 연결

    void Start()
    {
        RoundData data = GameManager.Instance.currentRoundData;//GameManager에 연결된 SO값 받아옴
        Debug.Log("스폰 간격: " + data.spawnInterval);

        GameManager.Instance.OnBossSpawn += SpawnBoss;//보스 소환 이벤트 구독
        GameManager.Instance.OnRoundChanged += StartNewRound;//새로운 라운드 이벤트 구독
        StartCoroutine(SpawnRoutine());//반복해서 적을 생성하는 코루틴 시작
    }

    private void SpawnBoss()//보스 생성
    {
        RoundData currentData = GameManager.Instance.currentRoundData;//GameManager가 가진 현재 RoundData SO에서 값을 꺼내옴

        if (currentData.bossPrefab == null)
        {
            Debug.LogError($"[EnemySpawner] 현재 라운드({currentData.name})에 설정된 보스 프리팹(bossPrefab)이 없습니다! 인스펙터를 확인해!");
            return;
        }
        if (isBossSpawned)
        {
            Debug.LogWarning("[EnemySpawner] 이미 보스가 소환됐어! 중복 소환을 차단할게!");
            return;
        }

        isBossSpawned = true;

        GameObject bossObj = Instantiate(currentData.bossPrefab, spawnPoint.position, Quaternion.identity);//SO에 들어있는 보스 프리팹 사용

        Enemy bossEnemy = bossObj.GetComponent<Enemy>();//컴포넌트들을 가져와서 스포너가 직접 초기화 메서드 호출
        EnemyHealth bossHealth = bossObj.GetComponent<EnemyHealth>();
        EnemyMovement bossMovement = bossObj.GetComponent<EnemyMovement>();

        //데이터 초기화
        if (currentData.bossData != null)
        {
            if (bossEnemy != null) bossEnemy.Init(currentData.bossData);
            if (bossHealth != null) bossHealth.Init(currentData.bossData);
        }

        if (bossMovement != null)//보스 이동 경로 
        {
            bossMovement.Init(waypointGroup);

        }

        if (GameManager.Instance != null && bossHealth != null)
        {
            GameManager.Instance.RegisterEnemy(bossHealth);
        }
    }

    private void SpawnEnemy()
    {
        RoundData currentData = GameManager.Instance.currentRoundData;//GameManager가 가진 현재 RoundData SO에서 값을 꺼내옴

        if (currentData.enemyPrefab != null && spawnPoint != null)
        {   
            //Waypoint 1번 위치에, 회전값 없이 몬스터 생성
            GameObject enemyObj = Instantiate(currentData.enemyPrefab, spawnPoint.position, Quaternion.identity);

            //2. 필요한 컴포넌트들 가져오기
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            EnemyHealth enemyHealth = enemyObj.GetComponent<EnemyHealth>();
            EnemyMovement movement = enemyObj.GetComponent<EnemyMovement>();

            //3. 스포너가 직접 데이터를 넘겨주어 초기화 실행 (Awake나 Start에서 찾지 않도록!)
            if (currentData.enemyData != null)
            {
                if (enemy != null) enemy.Init(currentData.enemyData);
                if (enemyHealth != null) enemyHealth.Init(currentData.enemyData);
                if (movement != null) movement.Init(waypointGroup);//몬스터 이동 경로
            }

            //GameManager에게 등록 (이 과정이 있어야 GameManager가 사망을 인지)
            if (GameManager.Instance != null && enemyHealth != null)
            {
                GameManager.Instance.RegisterEnemy(enemyHealth);
            }
        }
    }

    private void StartNewRound(RoundData newData)//새로운 라운드 시작
    {
        ClearAllEnemies();     //이전 라운드 몬스터 정리
        isBossSpawned = false; //보스 잡힌 상태 초기화
        StopAllCoroutines();   //이전 라운드의 코루틴 멈춤
        StartCoroutine(SpawnRoutine());//새로운 코루틴 시작
        Debug.Log("새로운 라운드 시작! 데이터 적용 완료!");
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

    public void ClearAllEnemies()//다음 라운드로 넘어갈때 남아있을 수도 있는 몬스터들 전부 삭제
    {
        EnemyHealth[] allEnemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);//맵에 있는 모든 적을 찾아 삭제
        foreach (EnemyHealth enemy in allEnemies)
        {
            if (enemy != null) Destroy(enemy.gameObject);
        }
    }
}