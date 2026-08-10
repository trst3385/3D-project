using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour    
{
    [Header("이동 설정")]
    public List<GameObject> waypoints;
    private EnemyMovement movement;//이동 로직 컴포넌트
    private float currentSpeed;//현재 속도
    private EnemyAttack attack;//몬스터 공격

    [HideInInspector] public EnemyData enemyData;//8.10 Enemy Data SO연결, 외부(스포너)에서 데이터와 필요한 값들을 받아오기 위한 변수


    void Awake()//8.10 스포너(EnemySpawner)가 중심을 잡고 몬스터에게 데이터를 주입(Init)해 주는 방식으로 변경,
    {           //이젠 Awake에서는 컴포넌트 캐싱만 가볍게 수행
        attack = GetComponent<EnemyAttack>();
        movement = GetComponent<EnemyMovement>();
    }

    public void Init(EnemyData data)//스포너(EnemySpawner)가 호출해 줄 초기화 Init메서드
    {
        enemyData = data;
        currentSpeed = enemyData.Speed;

        if (GameManager.Instance != null) GameManager.Instance.AddEnemy(this);
    }

    void Update()
    {
        movement.Move(currentSpeed, OnReachedEnd);//EnemyMovement의 이동로직 사용
    }

    void OnDestroy()//파괴될 때(사망, 목적지 도달, 라운드 클리어로 삭제 등) GameManager의 리스트에서 제거
    {
        if (GameManager.Instance != null) GameManager.Instance.RemoveEnemy(this);
    }

    //------슬로우 로직 등은 여기에 유지-----
    public void SetSlow(float slowPercent) => currentSpeed = enemyData.Speed * slowPercent;
    //Slow타워의 슬로우. slowPercent가 0.5라면, 50% 속도로 줄어듦
    public void ResetSpeed() => currentSpeed = enemyData.Speed;//사거리에 벗어나면 원래 속도로 복구
    //---------------------------------------


    void OnReachedEnd()//마지막 포인트에 도달하면 오브젝트 삭제 및 플레이어에게 데미지
    {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();//FindFirstObjectByType = 씬에 하나밖에 없는 오브젝트를 찾을 때

        if (player != null) player.TakeDamage(enemyData.Damage);

        Destroy(gameObject);
    }  
}
    
