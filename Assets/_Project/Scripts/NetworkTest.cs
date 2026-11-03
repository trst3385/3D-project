using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

//1. 서버에서 받아올 JSON 데이터의 구조를 정의하는 클래스
[Serializable]
public class UserData//현재는 서버 테스트 용이니 임시로 추가
{
    public int userId;
    public int id;
    public string title;
    public bool completed;
}

public class NetworkTest : MonoBehaviour
{
    // 테스트용 무료 REST API 주소
    private string testUrl = "https://jsonplaceholder.typicode.com/todos/1";

    void Start()
    {
        // 스크립트가 켜지면 서버 통신 코루틴 시작
        StartCoroutine(GetUserDataRoutine());
    }

    private IEnumerator GetUserDataRoutine()
    {
        Debug.Log("서버에 데이터 요청을 보냅니다...");

        //2. UnityWebRequest를 사용해 URL로 GET 요청 생성
        using (UnityWebRequest request = UnityWebRequest.Get(testUrl))
        {
            yield return request.SendWebRequest();//서버 응답이 올 때까지 기다림 (비동기 처리)

            //3. 통신 결과 확인
            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawJson = request.downloadHandler.text;//서버가 보내준 원본 텍스트(JSON)
                Debug.Log($"[서버 응답 원본 JSON]:\n{rawJson}");

                //4. JSON 텍스트를 C# UserData 객체로 변환 (역직렬화)
                UserData data = JsonUtility.FromJson<UserData>(rawJson);

                //5. C# 객체 데이터 사용 확인
                Debug.Log($"[파싱 완료] 유저 ID: {data.userId} | 제목: {data.title} | 완료여부: {data.completed}");
            }
            else Debug.LogError($"통신 에러 발생: {request.error}");
        }
    }
}
