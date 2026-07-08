using System.Collections.Generic;
using UnityEngine;
 
public class SlowTree : MonoBehaviour, ITree//AttackTree와 유사한 구조, 인터페이스 사용을 위해 클래스 이름 옆에 , ITree를 추가
{
    public TreeData GetTreeData()//인터페이스(ITree)에서 시킨 대로, 데이터를 외부로 전달해주는 '전용 통로'를 만듦.
    {                            //다른 스크립트가 "GetTreeData()"라고 부르면, 내 데이터SO(treeData)를 넘겨줌.
        return treeData;
    }

    [Header("데이터")]
    public TreeData treeData;//데이터 참조
    private List<Transform> enemiesInRange = new List<Transform>();

    void Start()
    {   //콜라이더의 Radius의 값을 5로 해놓든 100으로 해놓든, 게임이 시작되는 순간 towerData.Range 값으로 바껴
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
        {
            col.radius = treeData.SlowRange;
        }
    }

    void Update()
    {
        ApplySlowToAll();
    }

    void ApplySlowToAll()
    {
        //리스트 순회하며 슬로우 적용
        foreach (Transform enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                Enemy e = enemy.GetComponent<Enemy>();
                if (e != null) e.SetSlow(treeData.SlowAmount);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //리스트에서 제거하고 속도 복구
            Enemy e = other.GetComponent<Enemy>();
            if (e != null) e.ResetSpeed();
            enemiesInRange.Remove(other.transform);
        }
    }

    private void OnDestroy()//나무가 파괴(Destroy)될 때 슬로우 상태인 몬스터의 속도를 복구
    {                       //OnDestroy: MonoBehaviour를 상속받는 객체가 파괴되기 직전에 마지막으로 호출되는 생명주기 이벤트 함수
        foreach (Transform enemyTransform in enemiesInRange)//몬스터가 씬에 아직 존재한다면 속도 복구 명령
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
