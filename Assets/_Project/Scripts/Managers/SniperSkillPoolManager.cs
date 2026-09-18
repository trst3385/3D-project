using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SniperSkillPoolManager : MonoBehaviour
{
    public static SniperSkillPoolManager Instance;

    [Header("Pool Settings")]
    [SerializeField] private SniperSkill sniperskill;//인스펙터에서 ItemBullet 프리팹 연결
    [SerializeField] private int poolSize = 1;       //풀(창고)에 미리 만들어 둘 오브젝트 개수 (동시 사용 최대 개수)

    [Header("스킬 비용 & 버튼UI(자동찾기)")]
    [SerializeField] private int skillCost = 30;//스킬 사용에 필요한 골드 비용
    [SerializeField] private Button SniperSkillButton;//인스펙터에서 스킬 UI 버튼 연결


    private Queue<SniperSkill> bulletPool = new Queue<SniperSkill>();//SniperSkill 오브젝트 풀링 가져오기

    void Awake()
    {
        //싱글톤 구조 (어디서든 버튼으로 쉽게 호출할 수 있게)
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //SniperSkillButton 버튼 자동 찾기
        GameObject buttonObj = GameObject.Find("SniperSkillButton");//씬에 있는 버튼 오브젝트 이름
        if (buttonObj != null) SniperSkillButton = buttonObj.GetComponent<Button>();
        else Debug.LogWarning("SniperSkillButton을 찾을 수 없어!");

        InitializePool();
    }

    void Update()//매 프레임 골드와 게임 상태를 체크해서 버튼 색상 변경 처리 (활성화/비활성화 상태 표시)
    {
        if (SniperSkillButton != null && GameManager.Instance != null)
        {
            //게임 준비 중이거나 골드가 부족하면 버튼 비활성화 (자동 회색 처리)
            bool canUse = !GameManager.Instance.IsGameReady && GameManager.Instance.CurrentGold >= skillCost;
            SniperSkillButton.interactable = canUse;
        }
    }

    private void InitializePool()//게임 시작 시 미리 오브젝트들을 만들어 풀에 넣어두기
    {
        for (int i = 0; i < poolSize; i++)
        {
            SniperSkill bullet = Instantiate(sniperskill, transform);
            bullet.gameObject.SetActive(false);//비활성화 상태로 대기

            bulletPool.Enqueue(bullet);
        }
    }

    public void UseSkill()//스킬 버튼을 눌렀을 때 외부(UI 버튼 등)에서 호출할 함수
    {
        //1. 게임 시작 전 카운트다운(준비 상태) 중에는 스킬을 사용할 수 없도록 차단
        if (GameManager.Instance != null && GameManager.Instance.IsGameReady)
        {
            Debug.Log("카운트다운 중에는 조준 스킬을 사용할 수 없어!");
            return;
        }

        //2. 골드 소모 시도 (골드가 부족하면 UseGold 내부에서 false를 반환하고 차단됨)
        if (GameManager.Instance != null && !GameManager.Instance.UseGold(skillCost))
        {
            Debug.Log("골드가 부족하여 조준 스킬을 사용할 수 없습니다!");
            //TODO: 나중에 골드 부족 UI 팝업이나 사운드 연출을 여기에 붙이면 돼!
            return;
        }

        //3. 골드가 성공적으로 차감되었고 풀에 여분이 있다면 스킬 발동
        if (bulletPool.Count > 0)
        {
            SniperSkill bullet = bulletPool.Dequeue();//풀에서 하나 꺼냄
            bullet.gameObject.SetActive(true);        //화면에 켜줌

            //탄환 내부의 Init을 실행해 주면서 반납 델리게이트도 함께 전달
            bullet.Init(ReturnToPool);

            Debug.Log($"조준 스킬 아이템 발동! (소모 골드: {skillCost})");
        }
        else Debug.LogWarning("풀에 남은 탄환이 없어! (풀 사이즈를 늘리거나 반납 확인 필요)");
    }

    private void ReturnToPool(SniperSkill bullet)//탄환이 수명을 다하거나 다 썼을 때 다시 풀(큐)로 돌려보내는 함수
    {
        bullet.gameObject.SetActive(false); //화면에서 끔
        bulletPool.Enqueue(bullet);         //큐에 다시 집어넣어 재사용 대기
        Debug.Log("탄환이 오브젝트 풀로 안전하게 반환됨.");
    }
}
