using UnityEngine;
using System.Collections;
using TMPro;

public class GoldPopupText : MonoBehaviour
{
    [Header("GoldPopupText UI자동연결")]
    public TextMeshProUGUI popupText;  //GoldPopupText UI 연결(자동연결)

    private float moveSpeed = 10f;      //위로 올라가는 속도
    private float lifeTime = 1f;        //유지 시간 (초)
    private Vector3 targetWorldPos;     //몬스터의 3D 월드 좌표를 담을 변수

    void Awake()
    {
        //자동 찾기: 내 자신(GameObject)에 붙어 있는 TextMeshProUGUI 컴포넌트를 알아서 가져옴
        popupText = GetComponent<TextMeshProUGUI>();
        if (popupText == null)
        {
            Debug.LogError("GoldPopupText 오브젝트에 TextMeshProUGUI 컴포넌트가 없어!");
        }
    }

    public void Setup(int goldAmount, Vector3 worldPos)//몬스터가 죽을 때 골드 양과 3D 월드 위치를 함께 전달받음
    {
        if (popupText != null)
        {
            popupText.text = $"+{goldAmount}G";
        }

        targetWorldPos = worldPos + Vector3.up * 1f;//머리 위 살짝 띄운 위치를 월드 좌표로 설정

        UpdateScreenPosition();//생성되자마자 정확한 화면 좌표로 즉시 이동
        StartCoroutine(PopupRoutine());//생성되자마자 위로 올라가고 사라지는 코루틴 시작
    }
    void UpdateScreenPosition()
    {
        if (Camera.main != null)//3D 월드 좌표를 2D 화면 좌표로 변환하는 책임을 팝업(이 스크립트) 자신이 가짐!
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetWorldPos);
            transform.position = screenPos;
        }
    }
    IEnumerator PopupRoutine()
    {
        float elapsedTime = 0f;
        Color startColor = popupText.color;

        while (elapsedTime < lifeTime)
        {
            elapsedTime += Time.deltaTime;

            //1. 위로 이동 (UI Canvas 기준)
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            //2. 투명도 서서히 낮추기 (Fade Out)
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / lifeTime);
            popupText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(gameObject);//시간이 다 되면 자기 자신 파괴 (메모리 정리)
    }
}
