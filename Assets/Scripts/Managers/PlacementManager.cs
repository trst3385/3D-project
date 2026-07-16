using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;//싱글톤


    //...골드로 나무 구매 시스템(당장은 하드코딩+이 스크립트에 적용...
    public int currentGold = 50;//시작 골드
    public int treeCost = 25;   //나무당 비용
    //.....

    public GameObject[] treePrefabs;//여러 나무를 인스펙터에서 리스트로 관리
    public int currentSelectedTreeIndex = 0;//지금 선택된 나무 인덱스

    private bool isTreeSelected = false;//나무를 선택했는지 확인하는 상태 변수
    private GameObject currentPreview;//나무 아이콘 클릭 시 나무 이미지가 마우스를 따라 움직임

    void Awake()
    {
        if (Instance != null && Instance != this)//인스턴스가 이미 존재하면 파괴하고, 처음이면 자신을 인스턴스로 설정
        {
            Destroy(gameObject);//싱글톤 중복 생성 방지 및 인스턴스 초기화
            return;
        }
        Instance = this;
    }


    void Update()
    {
        if (isTreeSelected && currentPreview != null)//나무 선택 시 나무 오브젝트가 마우스에 붙기
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//1. 마우스 위치에서 카메라 뷰 방향으로 레이 생성
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);//2. 바닥 높이(Y=0)의 가상 평면 정의

            if (groundPlane.Raycast(ray, out float rayDistance))//3. 레이와 평면이 만나는 지점을 계산하여 위치 업데이트
            {
                currentPreview.transform.position = ray.GetPoint(rayDistance);//배치 대상이 마우스 커서를 따라다니도록 실시간 좌표 동기화
            }
        }

        if (Input.GetMouseButtonDown(1))//마우스 오른쪽 버튼. 나무 선택 취소
        {
            isTreeSelected = false;
            if (currentPreview != null)
            {
                Destroy(currentPreview);
            }
            currentPreview = null;
            Debug.Log("나무 선택 취소!");
        }

        if (Input.GetMouseButtonDown(0))//마우스 왼쪽 버튼 클릭 시
        {
            //IsPointerOverGameObject란? : 마우스가 UI 요소(버튼, 패널 등) 위에 있는지 확인,
            //UI 클릭 시 게임 월드에서 레이캐스트가 작동하지 않게 막는 방어 코드
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }//작동 원리: EventSystem이 현재 마우스 위치에 그려진 UI 요소가 있는지 레이캐스트를 쏴서 확인.
             //만약 UI가 있으면 true, 없으면 false를 반환.
             //왜 써야 해?: 이게 없으면 특정 나무를 선택하려고 UI 버튼을 눌렀을 뿐인데,
             //맵 뒤에 있는 다른 오브젝트나 UI, TreeSlot 등. 같이 클릭될 수 있는 '클릭 관통(Click-through)' 현상이 발생해.

            if (!isTreeSelected)//나무가 선택되지 않았다면 아래 로직을 실행하지 않고 중단
            {
                Debug.Log("나무를 먼저 선택해!");
                return;
            }

            if (currentGold < treeCost)//골드 체크 로직
            {
                Debug.Log("골드가 부족해! 나무를 심을 수 없어!");
                return;
            }


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//카메라에서 마우스 위치로 레이 발사
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("TreeSlot");//"TreeSlot" 레이어만 감지하도록 설정 (LayerMask 필수!)
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                TreeSlot slot = hit.collider.GetComponent<TreeSlot>();
                if (slot != null)
                {
                    if (treePrefabs[currentSelectedTreeIndex] == null)//인덱스가 비었을때. 빨간색 오류로 띄우기 (문제가 있거나 심각할 때)
                    {
                        Debug.LogError("[시스템 오류] 배치할 나무 프리팹이 연결되지 않았어! 인스펙터를 확인해!");
                        return;//오류 시 함수 종료
                    }

                    slot.PlantTree(treePrefabs[currentSelectedTreeIndex]);//현재 선택된 나무를 배치

                    currentGold -= treeCost;//보유 골드 차감
                    isTreeSelected = false;//배치 완료 후 선택 해제
                    Debug.Log($"배치 완료! 남은 골드: {currentGold}");
                }
            }

            if (currentPreview != null)//배치 완료 시 미리보기 이미지 삭제
            {
                Destroy(currentPreview);
                currentPreview = null;
            }
        }
    }

    public void SelectTree(int index)//버튼에 연결할 함수
    {
        //버튼에서 들어온 인덱스로 현재 선택된 나무를 바꿈
        currentSelectedTreeIndex = index;
        isTreeSelected = true;//버튼을 누르면 선택된 상태로 변경

        if (currentPreview != null)//나무 선택 시 미리보기 생성
        {
            Destroy(currentPreview);
        }
        currentPreview = Instantiate(treePrefabs[currentSelectedTreeIndex]);
        Collider col = currentPreview.GetComponent<Collider>();//미리보기 이미지는 충돌 처리가 필요 없으니 콜라이더를 끄자
        if (col != null)
        {
            col.enabled = false;    
        }

        Debug.Log($"{treePrefabs[currentSelectedTreeIndex].name}을(를) 선택했어!");
    }
}
