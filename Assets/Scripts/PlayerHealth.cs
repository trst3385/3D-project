using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    [Header("데이터")]
    public PlayerData playerData;//SO 연결

    private int currentHealth; //현재 실시간 체력
    private Slider playerHpSlider;//플레이어 체력바 UI 참조

    void Awake()
    {
        GameObject hpBarObj = GameObject.Find("PlayerHpBar");
        if (hpBarObj != null)
        {
            playerHpSlider = hpBarObj.GetComponent<Slider>();

            //슬라이더가 컴포넌트에 제대로 붙어있는지 확인
            if (playerHpSlider != null)
            {
                playerHpSlider.maxValue = playerData.maxHealth;
                playerHpSlider.minValue = 0;
                playerHpSlider.value = playerData.maxHealth;
            }
            else
            {
                Debug.LogError("PlayerHpBar 오브젝트에 Slider 컴포넌트가 없어!");
            }
        }
        else
        {
            Debug.LogError("씬에 'PlayerHpBar'라는 이름의 오브젝트를 찾을 수 없어!");
        }
    }

    void Start()
    {
        currentHealth = playerData.maxHealth;     
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)//체력이 0 이하가 되면 강제로 0으로 고정
        {
            currentHealth = 0;
        }

        if (playerHpSlider != null)//데미지를 받으면 체력바 감소
        {
            playerHpSlider.value = currentHealth;
        }

        if (currentHealth <= 0)//체력이 0이 되면
        {
            Debug.Log("플레이어 사망!");
            //곧 게임 오버 로직 추가 예정(옵저버 패턴 방식)
        }
    }
}
