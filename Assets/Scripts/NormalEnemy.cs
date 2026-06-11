using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class NormalEnemy : MonoBehaviour    
{
    [Header("데이터")]
    public EnemyData enemyData;//Enemy Data SO연결

    [Header("이동 설정")]
    public List<GameObject> waypoints;
    private int currentIndex = 0;//현재 추적 중인 waypoints의 인덱스. 
                                 //도착 판정 시 1씩 증가하며 다음 목적지를 가리킴

    private float baseSpeed;//슬로우 상태 이후 원래 속도 보존용
    private float currentSpeed;//현재 적용된 속도


    void Start()
    {
        baseSpeed = enemyData.Speed;//SO에서 기본 속도를 가져옴
        currentSpeed = baseSpeed;   //처음엔 기본 속도로 시작

        //게임이 시작될 때 WaypointGroup이라는 이름을 가진 오브젝트를 찾아서 
        //그 자식들을 리스트에 자동으로 다 넣어버리는 로직
        GameObject group = GameObject.Find("WaypointGroup");
        if (group != null)
        {
            waypoints = new List<GameObject>();
            foreach (Transform child in group.transform)
            {
                waypoints.Add(child.gameObject);
            }
        }
    }

    void Update()
    {
        MoveToWaypoints();
    }

    void MoveToWaypoints()
    {
        if (currentIndex >= waypoints.Count)//모든 포인트를 다 돌았다면 멈춤(종료)
        {
            return;
        }

        Transform target = waypoints[currentIndex].transform;//[목표 설정] 리스트에서 현재 번호(currentIndex)에 해당하는 위치 정보를 가져와.

        Vector3 dir = target.position - transform.position;//[방향 계산] (목적지 - 현재위치)를 해서 내가 가야 할 방향 화살표를 만듦.
        dir.y = 0;//높낮이 차이는 무시하고 평면 이동만 고려

        //[실제 이동] 계산된 방향(dir)으로 정해진 속도만큼 매 프레임 이동,
        //normalized: 화살표의 길이를 1로 만들어, 거리에 상관없이 일정한 속도로 직선 이동하게 함
        transform.Translate(dir.normalized * currentSpeed * Time.deltaTime, Space.World);

        //[다음 목표 갱신] 목표 지점에 거의 도착(거리 0.1 이하)했다면, 다음 지점으로 가기 위해 번호표(currentIndex)를 1 증가시킴
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
        new Vector3(target.position.x, 0, target.position.z)) <= 0.1f)//Vector3값을 새로 만든 이유,
        {                                                             //Y값은 무시하고 X,Z값만을 보고 다음 포인트로 이동
            currentIndex++;

            //[마지막 포인트에 도착 시 처리]
            if (currentIndex >= waypoints.Count)
            {
                OnReachedEnd();
            }
        }
    }

    public void SetSlow(float slowPercent)//Slow타워의 사거리에 들어오면
    {
        currentSpeed = baseSpeed * slowPercent;//slowPercent가 0.5라면, 50% 속도로 줄어듦
    }
    public void ResetSpeed()
    {
        currentSpeed = baseSpeed;//다시 원래 속도로 복구
    }

    void OnReachedEnd()//마지막 포인트에 도달하면 오브젝트 삭제 및 플레이어에게 데미지
    {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();//FindFirstObjectByType = 씬에 하나밖에 없는 오브젝트를 찾을 때
        if (player != null)
        {
            player.TakeDamage(enemyData.Damage);
        }
        GameManager.Instance.OnEnemyDefeated();//목표 처치 수 카운트 증가
        Destroy(gameObject);
    }
}
    
