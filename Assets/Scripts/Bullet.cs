using UnityEngine;

public class Bullet : MonoBehaviour
{
    //---이제 타워한테 받아서 씀---
    private float speed;
    private int damage;
    //-----------------------------

    private Transform target;//따라갈 목표
    private bool isHit = false;//이미 대상에 맞았는지 확인

    public void Initialize(Transform _target, float _speed, int _damage)//발사체 생성 후, 타워가 데이터를 주입해주는 함수(타워에 SO연결)
    {
        target = _target;
        speed = _speed;
        damage = _damage;
    }

    void Update()
    {
        if (isHit)//이미 무언가에 맞아서 처리 중이라면 이동하지 않음
        {
            return;
        }
        if (target == null)//목표가 사라지면 발사체도 삭제
        {
            Destroy(gameObject);
            return;
        }

        //목표를 향한 방향 계산
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        //이동 및 회전
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);//실제로 이동 (Normalized로 방향만 추출)
        transform.LookAt(target);//발사체가 적을 바라보게 하면 더 자연스러움                       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isHit || !other.CompareTag("Enemy"))// 이미 맞았거나, 상대방이 적이 아니면 무시
        {
            return;
        }

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            HitTarget(enemy);
        }
    }

    void HitTarget(EnemyHealth enemy)
    {
        isHit = true;//즉시 중복 충돌 방지

        enemy.TakeDamage(damage);//데미지 전달

        MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();//[시각적 즉시 제거] 자식(Capsule)의 모습만 바로 끈다
        if (mesh != null)
        {
            mesh.enabled = false;
        }

        Collider col = GetComponent<Collider>();//[물리적 즉시 제거] 콜라이더를 꺼서 추가 충돌 방지
        if (col != null)
        {
            col.enabled = false;
        }

        Destroy(gameObject);//실제 오브젝트 삭제
    }
}
