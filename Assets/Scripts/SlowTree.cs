using System.Collections.Generic;
using UnityEngine;

public class SlowTree : MonoBehaviour//AttackTree와 유사한 구조
{
    public TowerData towerData;
    private List<Transform> enemiesInRange = new List<Transform>();

    void Start()
    {   //콜라이더의 Radius의 값을 5로 해놓든 100으로 해놓든, 게임이 시작되는 순간 towerData.Range 값으로 바껴
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
        {
            col.radius = towerData.Range;
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
                NormalEnemy e = enemy.GetComponent<NormalEnemy>();
                if (e != null) e.SetSlow(towerData.SlowAmount);
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
            NormalEnemy e = other.GetComponent<NormalEnemy>();
            if (e != null) e.ResetSpeed();
            enemiesInRange.Remove(other.transform);
        }
    }
}
