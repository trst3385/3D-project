using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public LayerMask treeLayer;//인스펙터에서 Tree 레이어만 체크하도록 설정

    private Enemy enemy;//Enemy 스크립트 참조(SO를 담은 허브 스크립트)
    private float timer;//(공격 주기 관리 attackInterval) 공격과 공격 사이의 흐른 시간을 누적


    void Awake()
    {
        enemy = GetComponent<Enemy>();//Enemy 스크립트를 가져와서 SO 데이터에 접근할 준비
    }

    void Update()
    {
        //enemy.enemyData.AttackInterval SO의 데이터 사용
        timer += Time.deltaTime;
        if (timer >= enemy.enemyData.AttackInterval)
        {
            TryAttack();
            timer = 0f;//공격 후 공격 주기 리셋
        }
    }

    void TryAttack()
    {
        //1. 공격 범위 내에 있는 모든 물체를 찾음 (Tree 레이어)
        Collider[] hits = Physics.OverlapSphere(transform.position, enemy.enemyData.AttackRange, treeLayer);
        Collider nearestCollider = null;//ITree 대신 Collider를 저장할 변수
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            ITree tree = hit.GetComponent<ITree>();//ITree 인터페이스를 구현한 스크립트가 있는지 확인(데이터 접근을 위한 통로 찾기)
            if (tree != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestCollider = hit;
                }
            }
        }

        if (nearestCollider != null)//찾으면 공격
        {
            PerformAttack(nearestCollider);//이제 Collider를 넘겨줌
        }
    }

    void PerformAttack(Collider targetCollider)
    {
        TreeHealth treeHealth = targetCollider.GetComponent<TreeHealth>();//공격받는 오브젝트에서 바로 'TreeHealth'를 찾기

        if (treeHealth != null)
        {
            treeHealth.TakeDamage();//1. 나무 체력(피격 카운트) 감소

            GameObject effect = enemy.enemyData.attackEffectPrefab;
            if (effect != null)//2. 이펙트 생성 (이펙트 프리팹이 연결되어 있다면!)
            {
                Instantiate(effect, targetCollider.transform.position, Quaternion.identity);//SO에 있는 피격 이펙트 생성
            }
        }
    }


    private void OnDrawGizmos()//씬뷰에서 몬스터 주변에 나무 공격 사거리 시각화 
    {                          //OnDrawGizmosSelected(): 오브젝트를 클릭했을 때만 보임
                               //OnDrawGizmos(): 항상 보임
        if (enemy == null || enemy.enemyData == null)//공격 범위가 없으면 안 보이니까 방어적 설계
        {
            return;
        }

        Gizmos.color = Color.red;//사거리는 빨간색 원으로
        Gizmos.DrawWireSphere(transform.position, enemy.enemyData.AttackRange);
    }
}
