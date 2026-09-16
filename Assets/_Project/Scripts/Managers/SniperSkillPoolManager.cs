using UnityEngine;
using System.Collections.Generic;

public class SniperSkillPoolManager : MonoBehaviour
{
    public static SniperSkillPoolManager Instance;

    [Header("Pool Settings")]
    [SerializeField] private SniperSkill sniperskill;//인스펙터에서 ItemBullet 프리팹 연결
    [SerializeField] private int poolSize = 1;       //풀(창고)에 미리 만들어 둘 오브젝트 개수 (동시 사용 최대 개수)

    private Queue<SniperSkill> bulletPool = new Queue<SniperSkill>();

    void Awake()
    {
        //싱글톤 구조 (어디서든 버튼으로 쉽게 호출할 수 있게)
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePool();
    }

    private void InitializePool()//게임 시작 시 미리 오브젝트들을 만들어 풀에 넣어두기
    {
        for (int i = 0; i < poolSize; i++)
        {
            SniperSkill bullet = Instantiate(sniperskill, transform);
            bullet.gameObject.SetActive(false);//비활성화 상태로 대기

            //풀로 돌려보내는 콜백(액션) 등록: "나중에 ReturnToPool이 실행되면 이 큐에 다시 넣어줘!"
            bullet.Init(ReturnToPool);

            bulletPool.Enqueue(bullet);
        }
    }

    public void UseSkill()//스킬 버튼을 눌렀을 때 외부(UI 버튼 등)에서 호출할 함수
    {
        if (bulletPool.Count > 0)
        {
            SniperSkill bullet = bulletPool.Dequeue();//풀에서 하나  꺼냄
            bullet.gameObject.SetActive(true);       //화면에 켜줌

            //탄환 내부의 Init을 실행해 주면서 반납 델리게이트도 함께 전달
            bullet.Init(ReturnToPool);

            Debug.Log("조준 스킬 아이템 발동! 탄환 준비");
        }
        else
        {
            Debug.LogWarning("풀에 남은 탄환이 없어! (풀 사이즈를 늘리거나 반납 확인 필요)");
        }
    }

    private void ReturnToPool(SniperSkill bullet)//탄환이 수명을 다하거나 다 썼을 때 다시 풀(큐)로 돌려보내는 함수
    {
        bullet.gameObject.SetActive(false); // 화면에서 끔
        bulletPool.Enqueue(bullet);         // 큐에 다시 집어넣어 재사용 대기
        Debug.Log("탄환이 오브젝트 풀로 안전하게 반환됨.");
    }
}
