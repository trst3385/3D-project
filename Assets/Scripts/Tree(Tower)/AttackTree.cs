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

    [SerializeField] private List<Transform> enemiesInRange = new List<Transform>();//사거리 내 적들 리스트
    private float fireCountdown = 0f;//공격 대기 시간
    private Transform target;


    void Start()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)//트리거 콜라이더 사이즈 자동 설정 (SO의 range 사용)
        {
            col.radius = treeData.Range;//range변수를 우선으로 사거리를 정해서 콜라이더의 Radius값과 달라도 range값을 우선으로 정함
        }                               
    }

    void Update()
    {
        UpdateTarget();

        if (fireCountdown > 0f)//매 프레임 델타 타임을 누적 감소시켜 공격 쿨타임(Cooldown)을 제어
        {
            fireCountdown -= Time.deltaTime;
        }

        //사거리 내 타겟 존재 여부에 따른 라인 렌더러 제어
        if (target != null)
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

    void OnTriggerEnter(Collider other)//트리거 안에 적이 들어오면 리스트에 추가
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.transform);
            Debug.Log(gameObject.name + "에 적 추가됨! 현재 적 수: " + enemiesInRange.Count);
        }
    }
    void OnTriggerExit(Collider other)//트리거 밖으로 적이 나가면 리스트에서 제거
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.transform);
        }
    }

    
    void UpdateTarget()//사거리 안의 적을 찾는 함수
    {
        if (enemiesInRange.Count == 0)//리스트가 비어있으면 타겟을 null로 만들고 종료
        {
            target = null;
            return;
        }

        enemiesInRange.RemoveAll(t => t == null);//죽어서 비활성화된 오브젝트는 리스트에서 미리 삭제

        //리스트에서 가장 가까운 적 찾기
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;


        //이제 리스트만 순회하면 됨 (OverlapSphere보다 훨씬 빠름)
        foreach (Transform enemy in enemiesInRange)
        {
            float dist = Vector3.Distance(transform.position, enemy.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearestEnemy = enemy;
            }
        }

        target = nearestEnemy;//범위 안에 들어온 오브젝트를 적으로 식별
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
