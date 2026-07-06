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
}
