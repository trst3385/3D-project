using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("이 오브젝트가 보스 몬스터면 체크")]
    [Tooltip("지금 이 오브젝트가 보스면 체크(이 오브젝트가 보스인걸 확인")]
    public bool isBoss = false;//인스펙터에서 보스 프리팹만 체크해줘(보스는 카운트에 포함 되지 않게)
    public event System.Action<bool> OnEnemyDie;//(GameManager 등) 몬스터 사망 이벤트 구독

    [Header("획득 골드 팝업 설정")]
    public GameObject goldPopupPrefab;//여기에 인스펙터에서 팝업 프리팹을 넣어주기!

    private Enemy enemyStats;//중심 스크립트의 SO 참조
    private int currentHealth;//현재 체력
    private Slider hpSlider;

    void Awake()
    {
        enemyStats = GetComponent<Enemy>();//중심 스크립트(Enemy) 가져오기

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
        Debug.Log("Die() 함수 진입함!");

        //1. 몬스터 SO에 설정된 보상 골드 가져오기 (EnemyData에 RewardGold가 있다고 가정)
        //만약 EnemyData에 아직 RewardGold가 없다면 임시로 숫자로 넣어도 돼 (예: 10)
        int rewardGold = enemyStats != null && enemyStats.enemyData != null ? enemyStats.enemyData.RewardGold : 10;

        if (GameManager.Instance != null)//2. GameManager에 골드 추가 요청
        {
            GameManager.Instance.AddGold(rewardGold);//GameManager에 AddGold 함수가 있어야 해
        }

        //3. 골드 팝업 텍스트 생성
        SpawnGoldPopup(rewardGold);

        //4. 기존 사망 이벤트 및 파괴
        OnEnemyDie?.Invoke(isBoss);//(옵저버)구독자가 있다면 이벤트를 실행(Invoke)함, 보스면 true, 아니면 false를 전달
        Destroy(gameObject);
    }

    void SpawnGoldPopup(int goldAmount)//처치시 획득 골드 팝업 생성 함수
    {
        Debug.Log("SpawnGoldPopup 함수 실행됨!");

        if (goldPopupPrefab != null)
        {
            //몬스터 머리 위 월드 좌표를 화면 2D 좌표로 변환
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0f);//뒤의 ~f로 획득골드 위치 조절

            //아무 캔버스나 찾지 말고, 이름이 "UI Canvas"인 메인 캔버스를 콕 집어서 찾기!
            GameObject mainCanvas = GameObject.Find("UI Canvas");

            if (mainCanvas != null)
            {
                //찾은 메인 캔버스의 자식으로 팝업 생성
                GameObject popupObj = Instantiate(goldPopupPrefab, mainCanvas.transform);
                popupObj.transform.position = screenPos;

                GoldPopupText popup = popupObj.GetComponent<GoldPopupText>();
                if (popup != null)
                {
                    popup.Setup(goldAmount);
                }
            }
            else
            {
                Debug.LogWarning("이름이 'Canvas'인 오브젝트를 찾을 수 없어! 캔버스 이름이 맞는지 확인해줘!");
            }
        }       
    }
}