using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Enemy enemy;//Enemy 스크립트 참조(SO를 담은 허브 스크립트)
    private float timer;//(공격 주기 관리 attackInterval) 공격과 공격 사이의 흐른 시간을 누적
    private bool isTargetInRange = false;// 사거리 내에 공격할 나무가 있는지 상태를 체크하는 플래그


    void Awake()
    {
        enemy = GetComponent<Enemy>();//Enemy 스크립트를 가져와서 SO 데이터에 접근할 준비
    }

    void Update()
    {
        if (enemy == null || enemy.enemyData == null) return;//enemyData가 null이면 아직 스포너에게 데이터를 못 받은 거니까 그냥 리턴

        //1. 매 프레임 사거리 내에 가장 가까운 나무 탐색
        TreeHealth nearestTree = GetNearestTree();

        if (nearestTree != null)
        {
            if (!isTargetInRange)// 사거리 안에 나무가 들어와 있으면?
            {
                PerformAttack(nearestTree);//방금 막 공격 사거리 안으로 진입한 순간 -> 즉시 첫 타 공격
                timer = 0f;//타이머 리셋 후 주기 재시작
                isTargetInRange = true;//"나 지금 사거리 안에 있어"라고 상태 전환
            }
            else//이미 사거리 안에 계속 머물러 있는 중 -> 공격 주기(Interval)마다 공격
            {
                timer += Time.deltaTime;
                if (timer >= enemy.enemyData.AttackInterval)
                {
                    PerformAttack(nearestTree);
                    timer = 0f;
                }
            }
        }
        else// 사거리 안에 나무가 없음 (사거리를 벗어남)
        {
            isTargetInRange = false;//상태 초기화
            timer = 0f;//타이머도 초기화
        }
    }

    TreeHealth GetNearestTree()//8.4 기존 TryAttack()의 사거리 내 나무 탐지 로직을 함수로 분리
    {                          //[반환 타입 설명]void가 아니라 'TreeHealth'를 적은 이유:     
                               //이 함수는 탐색을 끝내고 결과물로 "가장 가까운 나무의 TreeHealth 컴포넌트"를,
                               //호출한 곳(Update)에 쥐어주고(return) 끝날 것이기 때문

        //1. 물리 엔진(OverlapSphere) 대신 씬에 있는 모든 TreeHealth(또는 ITree)를 가져옴
        TreeHealth[] allTrees = Object.FindObjectsByType<TreeHealth>(FindObjectsSortMode.None);

        TreeHealth nearestTree = null;
        float minSqrDistance = Mathf.Infinity;
        float attackRange = enemy.enemyData.AttackRange;
        float sqrAttackRange = attackRange * attackRange;//제곱 거리 비교용

        foreach (var tree in allTrees)
        {
            if (tree == null) continue;

            //2. 순수 수학적 거리 계산 (sqrMagnitude 사용으로 성능 최적화)
            float sqrDist = (transform.position - tree.transform.position).sqrMagnitude;

            if (sqrDist <= sqrAttackRange)
            {
                if (sqrDist < minSqrDistance)
                {
                    minSqrDistance = sqrDist;
                    nearestTree = tree;
                }
            }
        }

        return nearestTree;//찾은 가장 가까운 나무(또는 없으면 null)를 결과물로 돌려줌
    }

    void PerformAttack(TreeHealth targetTree)
    {
        if (targetTree == null) return;

        targetTree.TakeDamage();//1. 나무 체력(피격 카운트) 감소

        GameObject effect = enemy.enemyData.attackEffectPrefab;
        if (effect != null)//2. 공격 이펙트 생성 (이펙트 프리팹이 연결되어 있다면)
        {
            Instantiate(effect, targetTree.transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()//씬뷰에서 몬스터 주변에 나무 공격 사거리 시각화 
    {                          //OnDrawGizmosSelected(): 오브젝트를 클릭했을 때만 보임
                               //OnDrawGizmos(): 항상 보임

        if (enemy == null || enemy.enemyData == null) return;//공격 범위가 없으면 안 보이니까 방어적 설계

        Gizmos.color = Color.red;//사거리는 빨간색 원으로
        Gizmos.DrawWireSphere(transform.position, enemy.enemyData.AttackRange);//몬스터SO의 공격 사거리(AttackRange)에 맞춰 크기 변경
    }
}
