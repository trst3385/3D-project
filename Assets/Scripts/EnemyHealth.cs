using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    private NormalEnemy enemyStats;//중심 스크립트의 SO 참조
    private int currentHealth;//현재 체력
    private Slider hpSlider;

    void Awake()
    {
        enemyStats = GetComponent<NormalEnemy>();//중심 스크립트(NormalEnemy) 가져오기

        //체력바 찾기
        Transform hpBarTransform = transform.Find("EnemyHpBar");
        if (hpBarTransform != null)
        {
            hpSlider = hpBarTransform.GetComponent<Slider>();
        }
        else
        {
            Debug.LogError($"{gameObject.name}에 EnemyHpBar가 없어!");
        }
        if (enemyStats == null)
        {
            Debug.LogError($"{gameObject.name}에 NormalEnemy 스크립트가 붙어있지 않아!");
        }
    }

    void Start()
    {
        if (enemyStats != null && enemyStats.enemyData != null)//SO의 값을 적용
        {
            currentHealth = enemyStats.enemyData.MaxHealth;//실행 시 현재 체력의 SO의 최대 체력으로 적용

            if (hpSlider != null)//체력값을 UI에게도 적용
            {
                hpSlider.maxValue = enemyStats.enemyData.MaxHealth;
                hpSlider.minValue = 0;
                hpSlider.value = currentHealth;
            }
        }
    }

    public void TakeDamage(int amount)//Bullet 등에 의해 받는 데미지를 입는 함수
    {
        currentHealth -= amount;

        if (hpSlider != null)//체력바UI에게 현재 체력을 전달
        {
            hpSlider.value = currentHealth;
        }

        if (currentHealth <= 0)//체력이 0 이하면 파괴
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
