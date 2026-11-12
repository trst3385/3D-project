using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;

    [Header("Zoom Settings")] //인스펙터 설정
    public float zoomSpeed;   //줌 속도
    public float minZoom;     //최소 확대 크기 (너무 가까이 갈 때)
    public float maxZoom;     //최대 축소 크기 (너무 멀어질 때)

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        //마우스 휠 스크롤 값 가져오기 (위로 올리면 양수, 아래로 내리면 음수)
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            //휠을 밀고 당길 때 orthographicSize를 조절
            cam.orthographicSize -= scroll * zoomSpeed;

            //너무 좁아지거나 넓어지지 않게 최소/최대값 제한 (Clamp)
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
