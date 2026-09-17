using UnityEngine;
using System.Collections;

public class SniperSkill : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;//조준 모드 유지 시간 (시간 지나면 자동 종료)
    [SerializeField] private int damage = 50;    //스킬 데미지
    [SerializeField] private int maxAmmo = 5;    //총 발사 가능한 횟수 (예: 5발)
    private int currentAmmo;                     //현재 남은 탄수

    private System.Action<SniperSkill> onReturnToPool;
    private bool isAiming = false;//현재 조준 모드 활성화 여부

    public void Init(System.Action<SniperSkill> returnAction)
    {
        onReturnToPool = returnAction;
        isAiming = true;
        currentAmmo = maxAmmo;//풀에서 꺼내질 때 탄수 초기화

        if (UIManager.Instance != null)//조준 시작 시 UIManager에 초기 탄수 전달 및 텍스트 켜기
        {
            UIManager.Instance.UpdateSniperAmmo(currentAmmo, maxAmmo);
        }

        StartCoroutine(LifeTimer());
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeTime);//lifeTime변수 시간 동안 조준 모드 상태
        EndAimMode();
    }

    void Update()
    {
        if (isAiming && Input.GetMouseButtonDown(0))//조준 모드일 때 마우스 왼쪽 클릭을 '한 번' 누른 순간에만 실행
        {
            TryShootEnemy();
        }
    }

    private void TryShootEnemy()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))//마우스 클릭한 위치로 레이를 쏴서 무언가 맞았는지 확인
        {
            //클릭한 순간에 딱 한 번만 GetComponent 수행, 맞은 오브젝트에서 EnemyHealth 컴포넌트 가져오기
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();

            if (enemyHealth != null)//몬스터의 체력 스크립트에 있는 TakeDamage 호출
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"조준 스킬로 타격 성공! 데미지: {damage} | 남은 탄수: {currentAmmo - 1}/{maxAmmo}");

                currentAmmo--;//몬스터를 맞췄든 아니든(혹은 맞췄을 때만 차감할 수도 있음) 탄수 1발 소모

                if (UIManager.Instance != null)//탄이 소모될 때마다 UIManager에 바뀐 탄수 전달
                {
                    UIManager.Instance.UpdateSniperAmmo(currentAmmo, maxAmmo);
                }

                if (currentAmmo <= 0)//탄을 다 썼다면 조준 모드 종료
                {
                    EndAimMode();
                }
            }
        }
    }
        
    public void EndAimMode()//조준 모드 종료
    {
        isAiming = false;
        StopAllCoroutines();

        if (UIManager.Instance != null)//조준 모드가 끝났으니 UIManager의 잔탄수 텍스트 숨기기 호출
        {
            UIManager.Instance.HideSniperAmmo();
        }

        ReturnToPool();
    }

    public void ReturnToPool()
    {
        //Destroy 대신 풀로 돌려보냄
        onReturnToPool?.Invoke(this);
    }
}
