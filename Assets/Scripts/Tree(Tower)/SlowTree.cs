using System.Collections.Generic;
using UnityEngine;
 
public class SlowTree : MonoBehaviour, ITree//AttackTree와 유사한 구조, 인터페이스 사용을 위해 클래스 이름 옆에 , ITree를 추가
{
    [Header("데이터")]
    public TreeData treeData;//데이터 참조
    public TreeData GetTreeData()//인터페이스(ITree)에서 시킨 대로, 데이터를 외부로 전달해주는 '전용 통로'를 만듦.
    {                            //다른 스크립트가 "GetTreeData()"라고 부르면, 내 데이터SO(treeData)를 넘겨줌.
        return treeData;
    }

    private List<Transform> enemiesInRange = new List<Transform>();//현재 타워 범위 안에 있는 적들의 Transform을 담아두는 리스트


    void Start()
    {   

    }

    void Update()
    {
        //1. 범위 내 적들을 탐지하고 리스트를 갱신 (나가거나 죽은 애들 처리)
        UpdateEnemiesInRange();

        //2. 정리된 리스트를 바탕으로 슬로우를 1번만 일괄 적용
        ApplySlowToAll();
    }

    void UpdateEnemiesInRange()//8.3일 수정. 물리엔진으로 계산하지 않고 순수 수학적 거리 계산으로 기존과 똑같이 작동하게 수정
    {
        //1. 기존 리스트를 뒤에서부터 검사하며 이미 죽었거나(`null`) 비활성화된 적 미리 솎아내기
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            if (enemiesInRange[i] == null || !enemiesInRange[i].gameObject.activeInHierarchy)
            {
                enemiesInRange.RemoveAt(i);
            }
        }

        //8.8 이전에는 FindObjectsByType로 씬을 뒤져서 Enemy를 찾았기에 부하의 원인이 될 수 있었어
        //Enemy[] allEnemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        var allEnemies = GameManager.Instance.activeEnemies;//2. GameManager의 activeEnemies 리스트를 가져와서 순회

        float rangeSq = treeData.SlowRange * treeData.SlowRange;//사거리의 제곱 값을 미리 구해둠(AttackTree와 유사한 방식)


        foreach (Enemy enemy in allEnemies)//3. 씬에 있는 모든 적들을 하나씩 돌며 거리 비교
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            Transform enemyTransform = enemy.transform;

            //타워와 적 사이의 벡터 거리 구하기 (만약 높이 차이 무시하고 싶다면 dir.y = 0 추가 가능)
            Vector3 dir = enemyTransform.position - transform.position;
            dir.y = 0;//2D 평면 거리처럼 X, Z만 비교하고 싶을 때 유용함!
            float distanceSq = dir.sqrMagnitude;//제곱 거리 (피타고라스 정리의 c^2)

            bool isInRange = distanceSq <= rangeSq;//사거리 내에 있는지 여부

            if (isInRange)
            {
                if (!enemiesInRange.Contains(enemyTransform))//범위 안에 있는데 기존 리스트에 없다면 새로 추가
                {
                    enemiesInRange.Add(enemyTransform);
                }
            }
            else
            {
                if (enemiesInRange.Contains(enemyTransform))//범위 밖에 있는데 기존 리스트에 있었다면? -> 범위 밖으로 나간 것!
                {
                    enemy.ResetSpeed();//속도 원복
                    enemiesInRange.Remove(enemyTransform);//리스트에서 제거
                }
            }
        }
    }


    void ApplySlowToAll()
    {
        //리스트 순회하며 슬로우 적용
        foreach (Transform enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                Enemy e = enemy.GetComponent<Enemy>();
                if (e != null)
                {
                    e.SetSlow(treeData.SlowAmount);
                }
            }
        }
    }

    private void OnDestroy()//나무가 파괴(Destroy)될 때 슬로우 상태인 몬스터의 속도를 복구
    {                       //OnDestroy: MonoBehaviour를 상속받는 객체가 파괴되기 직전에 마지막으로 호출되는 생명주기 이벤트 함수

        //타워가 파괴(Destroy)될 때, 아직 범위 안에 남아있던 모든 적의 속도를 원래대로 복구
        foreach (Transform enemyTransform in enemiesInRange)
        {
            if (enemyTransform != null)
            {
                Enemy e = enemyTransform.GetComponent<Enemy>();
                if (e != null)
                {
                    e.ResetSpeed();//Enemy 스크립트의 ResetSpeed()
                }
            }
        }
    }
}
