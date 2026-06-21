using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private List<GameObject> waypoints;
    private int currentIndex = 0;//현재 추적 중인 waypoints의 인덱스. 
                                 //도착 판정 시 1씩 증가하며 다음 목적지를 가리킴

    void Start()
    {
        GameObject group = GameObject.Find("WaypointGroup");//게임이 시작될 때 WaypointGroup이라는 이름을 가진 오브젝트를 찾아서,                                                  
        if (group != null)                                   //그 자식들을 리스트에 자동으로 다 넣어버리는 로직
        {
            waypoints = new List<GameObject>();
            foreach (Transform child in group.transform)
            {
                waypoints.Add(child.gameObject);
            }
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
  
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                             new Vector3(target.position.x, 0, target.position.z)) <= 0.1f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Count)//마지막 포인트에 도착 시 처리
            {
                onReachedEnd?.Invoke();//OnReachedEnd() 함수의 플레이어에게 데미지를 줌
            }
        }
    }
}
