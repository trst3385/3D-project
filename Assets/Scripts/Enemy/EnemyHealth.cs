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

    [Header("체력바 UI 설정")]
    public GameObject hpBarPrefab;      //인스펙터에서 체력바 프리팹 연결(수동연결)
    private GameObject spawnedHpBar;    //생성된 체력바 인스턴스
    private Slider hpSlider;            //생성된 체력바의 슬라이더 컴포넌트
    private RectTransform hpBarRectTransform;//위치 조작을 위한 RectTransform

    private Enemy enemyStats;//중심 스크립트의 SO 참조
    private int currentHealth;//현재 체력

    void Awake()
    {
        enemyStats = GetComponent<Enemy>();//중심 스크립트(Enemy) 가져오기
        if (enemyStats == null)
        {
            Debug.LogError($"{gameObject.name}에 Enemy 스크립트가 붙어있지 않아!");
        }
    }

    void Start()
    {
        if (enemyStats != null && enemyStats.enemyData != null)//SO의 값을 적용
        {
            currentHealth = enemyStats.enemyData.MaxHealth;//실행 시 현재 체력의 SO의 최대 체력으로 적용
        }

        CreateHpBar();//게임 시작 시 메인 캔버스에 체력바 생성 및 연결
    }

    void Update()
    {
        UpdateHpBarPosition();//몬스터가 살아있는 동안 매 프레임 머리 위 위치를 쫓아다니도록 갱신
    }

    void CreateHpBar()
    {
        if (hpBarPrefab != null)
        {
            GameObject mainCanvas = GameObject.Find("UI Canvas");
            if (mainCanvas != null)
            {
                //메인 캔버스의 자식으로 체력바 생성
                spawnedHpBar = Instantiate(hpBarPrefab, mainCanvas.transform);
                hpSlider = spawnedHpBar.GetComponent<Slider>();
                hpBarRectTransform = spawnedHpBar.GetComponent<RectTransform>();

                if (hpSlider != null)
                {
                    hpSlider.maxValue = enemyStats.enemyData.MaxHealth;
                    hpSlider.minValue = 0;
                    hpSlider.value = currentHealth;
                }
                else Debug.LogError("생성된 체력바 프리팹에 Slider 컴포넌트가 없어!");
            }
            else Debug.LogWarning("이름이 'UI Canvas'인 오브젝트를 찾을 수 없어!");
        }
        else Debug.LogWarning("EnemyHealth에 hpBarPrefab이 연결되지 않았어!");
    }

    void UpdateHpBarPosition()
    {
        if (spawnedHpBar != null && Camera.main != null)
        {
            // 몬스터 몸 아래 월드 좌표 (Vector3.up 뒤의 숫자로 높이 조절 가능)
            Vector3 worldPos = transform.position + Vector3.down * 1.5f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0)//카메라 뒤로 갔을 때 UI가 이상하게 튀는 현상 방지
            {
                spawnedHpBar.SetActive(false);
            }
            else
            {
                spawnedHpBar.SetActive(true);
                hpBarRectTransform.position = screenPos;
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
    void OnDestroy()//몬스터 오브젝트가 어떤 이유로든 파괴될 때(사망, 도착지 도달 등) UI도 제거
    {
        if (spawnedHpBar != null)
        {
            Destroy(spawnedHpBar);
        }
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