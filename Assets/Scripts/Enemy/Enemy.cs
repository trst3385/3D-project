using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour    
{
    [Header("데이터")]
    public EnemyData enemyData;//Enemy Data SO연결

    [Header("이동 설정")]
    public List<GameObject> waypoints;
    private EnemyMovement movement;//이동 로직 컴포넌트
    private float currentSpeed;//현재 속도

    [Tooltip("해당 몬스터 클릭 가능 여부")]
    public bool isSelected = false;//해당 몬스터 클릭(선택) 여부

    private EnemyAttack attack;
    public LineRenderer rangeLineRenderer;



    void Awake()
    {
        attack = GetComponent<EnemyAttack>();
        movement = GetComponent<EnemyMovement>();

        currentSpeed = enemyData.Speed;
    }

    void Start()
    {
        if (rangeLineRenderer != null)//시작할 땐 사거리 표시가 안 보이게
        {
            rangeLineRenderer.enabled = false;
        }
        DrawCircle();//원 모양을 미리 그려둠
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

    void OnMouseDown()//몬스터 클릭 시 공격 사거리 표시(EnemyAttack에 전달)
    {   
        //isSelected = true;로 되어 있으면 한번 클릭하면 계속 선택된 상태로 남아!
        isSelected = !isSelected;//클릭할 때마다 토글

        if (rangeLineRenderer != null)//사거리 표시를 선택 여부에 따라 켜기/끄기
        {
            rangeLineRenderer.enabled = isSelected;
        }
        Debug.Log(gameObject.name + " 선택됨!");

        //여기에 나중의 UI 호출 로직 등을 추가할 수 있어
    }
    void DrawCircle()//몬스터 클릭 사 공격 사거리 노출, 현재는 Start에서 한번 초기화되서 사거리 증가는 아직 불가.
    {
        if (rangeLineRenderer == null)
        {
            return;
        }

        int segments = 50;//원을 구성할 점의 개수
        rangeLineRenderer.positionCount = segments + 1;
        float radius = enemyData.AttackRange;//몬스터 SO의 공격 사거리 값을 받아옴

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;
            rangeLineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
    }
}
    
