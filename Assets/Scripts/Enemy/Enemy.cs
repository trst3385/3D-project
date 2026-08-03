using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour    
{
    [Header("데이터")]
    public EnemyData enemyData;//Enemy Data SO연결

    [Header("이동 설정")]
    public List<GameObject> waypoints;
    private EnemyMovement movement;//이동 로직 컴포넌트
    private float currentSpeed;//현재 속도


    private EnemyAttack attack;//몬스터 공격


    void Awake()
    {
        attack = GetComponent<EnemyAttack>();
        movement = GetComponent<EnemyMovement>();
        currentSpeed = enemyData.Speed;
    }


    void Update()
    {
        movement.Move(currentSpeed, OnReachedEnd);//EnemyMovement의 이동로직 사용
    }

    //------슬로우 로직 등은 여기에 유지-----
    public void SetSlow(float slowPercent) => currentSpeed = enemyData.Speed * slowPercent;
    //Slow타워의 슬로우. slowPercent가 0.5라면, 50% 속도로 줄어듦
    public void ResetSpeed() => currentSpeed = enemyData.Speed;//사거리에 벗어나면 원래 속도로 복구
    //---------------------------------------


    void OnReachedEnd()//마지막 포인트에 도달하면 오브젝트 삭제 및 플레이어에게 데미지
    {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();//FindFirstObjectByType = 씬에 하나밖에 없는 오브젝트를 찾을 때
        if (player != null)
        {
            player.TakeDamage(enemyData.Damage);
        }
        Destroy(gameObject);
    }  
}
    
