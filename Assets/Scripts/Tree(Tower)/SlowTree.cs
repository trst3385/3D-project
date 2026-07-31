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
    private Collider[] overlapResults = new Collider[50];//매번 메모리를 새로 할당(GC 유발)하지 않도록,
                                                         //OverlapSphere 결과를 담아둘 캐싱용 배열  (최대 50마리까지 감지)
    private LayerMask enemyLayer;//"Enemy" 레이어만 골라서 효율적으로 감지하기 위한 레이어 마스크


    void Start()
    {   
        //"Enemy" 레이어 번호를 가져와서 마스크에 저장
        enemyLayer = LayerMask.GetMask("Enemy");

        //(참고!) 굳이 SphereCollider를 물리용으로 쓸 필요가 없으니,
        //만약 콜라이더가 붙어있다면 Trigger로 켜져있거나 끄는 걸 고려할 수 있지만,
        //여기서는 오직 스크립트 수학적 거리/영역 연산으로 처리할게
    }

    void Update()
    {
        //1. 범위 내 적들을 탐지하고 리스트를 갱신 (나가거나 죽은 애들 처리)
        UpdateEnemiesInRange();

        //2. 정리된 리스트를 바탕으로 슬로우를 1번만 일괄 적용
        ApplySlowToAll();
    }

    void UpdateEnemiesInRange()
    {
        //1. 기존 리스트를 뒤에서부터 검사하며 이미 죽었거나(`null`) 비활성화된 적 미리 솎아내기
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            if (enemiesInRange[i] == null || !enemiesInRange[i].gameObject.activeInHierarchy)
            {
                enemiesInRange.RemoveAt(i);
            }
        }

        //2. 타워 위치 중심, treeData.SlowRange 반경 내에 있는 "Enemy" 레이어 콜라이더들을 배열에 싹 긁어오기
        //(OverlapSphereNonAlloc은 새 메모리를 안 만들어서 가비지 컬렉터에 친화적)
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, treeData.SlowRange, overlapResults, enemyLayer);

        for (int i = 0; i < hitCount; i++)//3. 방금 감지된 적들을 리스트에 추가 (이미 들어있는 애는 중복 추가 안 함)
        {
            Transform enemyTransform = overlapResults[i].transform;

            if (!enemiesInRange.Contains(enemyTransform))
            {
                enemiesInRange.Add(enemyTransform);
            }
        }

        //4. 기존 리스트에 있던 애들 중, 이번 프레임 감지 배열(overlapResults)에 없는 애들은 범위 밖으로 나간 것!
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            Transform enemyTrans = enemiesInRange[i];
            bool foundInCurrentFrame = false;

            for (int j = 0; j < hitCount; j++)
            {
                if (overlapResults[j].transform == enemyTrans)
                {
                    foundInCurrentFrame = true;
                    break;
                }
            }

            if (!foundInCurrentFrame)//범위 밖으로 나갔다면 속도 복구 후 리스트에서 제거
            {
                Enemy e = enemyTrans.GetComponent<Enemy>();
                if (e != null)
                {
                    e.ResetSpeed();
                }
                enemiesInRange.RemoveAt(i);
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
