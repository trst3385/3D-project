using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private List<GameObject> waypoints;
    private int currentIndex = 0;//현재 추적 중인 waypoints의 인덱스. 
                                 //도착 판정 시 1씩 증가하며 다음 목적지를 가리킴

    public void Init(GameObject waypointGroup)//스포너가 몬스터 생성 시 웨이포인트 그룹을 알려주게 함
    {
        waypoints = new List<GameObject>();
        foreach (Transform child in waypointGroup.transform)
        {
            waypoints.Add(child.gameObject);
        }
    }

    public void Move(float speed, System.Action onReachedEnd)//외부에서 속도와 마지막 도착 시 실행할 기능을 전달받음
    {
        if (waypoints == null || currentIndex >= waypoints.Count)//모든 이동포인트를 다 돌았다면 멈춤(종료)
        {
            return;
        }

        Transform target = waypoints[currentIndex].transform;//[목표 설정] 리스트에서 현재 번호(currentIndex)에 해당하는 위치 정보를 가져옴.
        Vector3 dir = target.position - transform.position;//[방향 계산] (목적지 - 현재위치)를 해서 내가 가야 할 방향 화살표를 만듦.
        dir.y = 0;//높낮이 차이는 무시하고 평면 이동만 고려

        //[실제 이동] 계산된 방향(dir)으로 정해진 속도만큼 매 프레임 이동,
        //normalized: 화살표의 길이를 1로 만들어, 거리에 상관없이 일정한 속도로 직선 이동하게 함
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);


        //----11.13 이동로직을 sqrMagnitude를 적용해서 성능을 최적화----
        //[거리 판정 최적화] Vector3.Distance 대신 sqrMagnitude 사용 (제곱근 연산 제거)
        Vector3 currentPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPos = new Vector3(target.position.x, 0, target.position.z);

        //허용 거리 0.1의 제곱인 0.01f와 비교
        if ((targetPos - currentPos).sqrMagnitude <= 0.01f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Count)//마지막 포인트에 도착 시 처리
            {
                onReachedEnd?.Invoke();//OnReachedEnd() 함수의 플레이어에게 데미지를 줌
            }
        }
        //----------------------이전 방식: Vector3.Distance 내부에서는 정확한 거리를 구하려고 제곱근(루트) 연산을 실행,
        //이 루트 연산은 매 프레임 모든 몬스터가 실행하기에는 은근히 CPU를 먹는 무거운 연산이야.
        //if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
        //                             new Vector3(target.position.x, 0, target.position.z)) <= 0.1f)
        //{
        //    currentIndex++;
        //    if (currentIndex >= waypoints.Count)//마지막 포인트에 도착 시 처리
        //    {
        //        onReachedEnd?.Invoke();//OnReachedEnd() 함수의 플레이어에게 데미지를 줌
        //    }
        //}
    }
}
