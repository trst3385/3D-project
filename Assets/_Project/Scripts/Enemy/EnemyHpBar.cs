using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    private Slider hpSlider;

    void Awake()
    {
        hpSlider = GetComponent<Slider>();//자기 자신에게 붙어 있는 Slider 컴포넌트 자동 찾기
        if (hpSlider == null)
        {
            Debug.LogError("EnemyHpBar 프리팹에 Slider 컴포넌트가 없어!");
        }
    }

    public void Setup(float maxHealth, float currentHealth)//몬스터가 생성될 때 체력바의 초기값 세팅
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.minValue = 0;
            hpSlider.value = currentHealth;
        }
    }

    public void UpdateHp(float currentHealth)//몬스터가 데미지를 입을 때 체력바 슬라이더 갱신
    {
        if (hpSlider != null)
        {
            hpSlider.value = currentHealth;
        }
    }
}
