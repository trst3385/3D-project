using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackTree : MonoBehaviour, ITree//ITree인터페이스 사용을 위해 클래스 이름 옆에 추가.
{                                             //"너 ITree 약속 지키기로 했지? 그럼 여기에 ITree인터페이스의 GetTreeData 함수를 반드시 만들어",
                                              //라고 강제하기 시작해. 안 만들면 에러를 발생해.
    public TreeData GetTreeData()//인터페이스(ITree)에서 시킨 대로, 데이터를 외부로 전달해주는 '전용 통로'를 만듦.
    {                            //다른 스크립트가 "GetTreeData()"라고 부르면, 내 데이터SO(treeData)를 넘겨줌.
        return treeData;
    }

    [Header("데이터")]
    public TreeData treeData;//Tower Data SO 데이터 연결

    [Header("발사체 발사 위치")]
    public Transform firePoint;//발사체가 생성될 위치
    public LineRenderer lineRenderer;//표적 조준

    private float fireCountdown = 0f;//공격 대기 시간
    private Transform target;


    void Start()
    {
        //8.3 물리 엔진(SphereCollider)을 안 쓰므로 Start는 비워둡니다.                         
    }

    void Update()
    {
        UpdateTarget();

        if (fireCountdown > 0f)//매 프레임 델타 타임을 누적 감소시켜 공격 쿨타임(Cooldown)을 제어
        {
            fireCountdown -= Time.deltaTime;
        }

        if (target != null)//사거리 내 타겟 존재 여부에 따른 라인 렌더러 제어
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint.position); //시작점: 타워 발사 위치(firePoint)
            lineRenderer.SetPosition(1, target.position);    //끝점: 타겟 위치
        }
        else
        {
            lineRenderer.enabled = false;//타겟이 없으면 라인 끄기
        }

        if (target != null && fireCountdown <= 0f)//현재 유효한 타겟이 존재하고 쿨타임이 만료되었을 때 발사 로직 실행
        {
            Shoot();
            fireCountdown = treeData.AttackInterval;//다음 공격을 위해 쿨타임 재설정
        }
    }

    //8.3 OnTriggerEnter와 OnTriggerExit 함수는 물리 엔진의 이벤트니까 이것들도 싹 지워준다.
   
    
    void UpdateTarget()//사거리 안의 적을 찾는 함수, 8.3 물리 대신 순수 수학적 거리 계산으로 가장 가까운 적 찾기
    {
        //1. 씬에 있는 모든 Enemy를 직접 가져옴
        Enemy[] allEnemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        //2. 공격 사거리의 제곱 값 미리 계산 (루트 연산 최적화)
        float rangeSq = treeData.Range * treeData.Range;

        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Enemy enemy in allEnemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            Transform enemyTransform = enemy.transform;

            //타워와 적 사이의 벡터 거리 구하기 (높이 Y축 차이는 무시)
            Vector3 dir = enemyTransform.position - transform.position;
            dir.y = 0;
            float distanceSq = dir.sqrMagnitude; // 거리의 제곱

            if (distanceSq <= rangeSq)//사거리 내에 들어왔는지 확인
            {
                if (distanceSq < shortestDistance)//그중에서도 가장 가까운 적을 타겟으로 삼기 위해 거리 비교
                {
                    shortestDistance = distanceSq;
                    nearestEnemy = enemyTransform;
                }
            }
        }

        target = nearestEnemy;//최종적으로 가장 가까운 적을 타겟으로 지정 (없으면 null)
    }


    void Shoot()
    {
        if (target == null)//공격할 적이 없으면 함수를 바로 종료
        {
            return;
        }

        GameObject bulletGO = Instantiate(treeData.BulletPrefab, firePoint.position, firePoint.rotation);//발사체 생성(Instantiate)
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)//발사체에게 타겟을 지정해줌
        {
            bullet.Initialize(target, treeData.BulletSpeed, treeData.BulletDamage);
        }
    }
}
