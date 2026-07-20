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
        UpdatePreviewPosition();//나무 선택 시 마우스 커서에 나무가 따라다니게 함

        //입력 처리 (입력만 감지해서 함수로 넘기기)
        if (Input.GetMouseButtonDown(1)) CancelPlacement();//마우스 오른쪽 버튼: 나무 선택 취소
        if (Input.GetMouseButtonDown(0)) TryPlantTree();//마우스 왼쪽 버튼: 나무 배치 시도      
    }

    private void UpdatePreviewPosition()//나무 선택 시 미리보기 이미지가 마우스를 따라다님
    {
        if (!isTreeSelected || currentPreview == null)//나무가 선택되지 않았거나 미리보기 오브젝트가 없으면 종료
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//1. 마우스 위치에서 카메라 뷰 방향으로 레이 생성
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);//2. 바닥 높이(Y=0)의 가상 평면 정의
        if (groundPlane.Raycast(ray, out float rayDistance))//3. 레이와 평면이 만나는 지점을 계산하여 위치 업데이트
        {
            currentPreview.transform.position = ray.GetPoint(rayDistance);
        }
    }

    private void CancelPlacement()//우클릭으로 나무 선택 취소
    {
        if (!isTreeSelected)//상태 초기화: 선택 해제 및 미리보기 오브젝트 삭제
        {
            return;
        }

        isTreeSelected = false;//나무 선택 상태 해제

        if (currentPreview != null)//미리보기 이미지 삭제(파괴)
        {
            Destroy(currentPreview);
        }
        currentPreview = null;//참조 초기화
        Debug.Log("나무 선택 취소!");
    }

    private void TryPlantTree()
    {
        //UI 클릭 방어: UI를 클릭했을 때는 게임 월드 레이캐스트가 작동하지 않게 함 (클릭 관통 현상 방지)
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
     
        if (!isTreeSelected)//나무가 선택되지 않았으면 로직 중단
        {
            Debug.Log("나무를 먼저 선택해!");
            return;
        }
        if (currentGold < treeCost)//현재 보유한 골드가 나무 비용보다 적으면 중단
        {
            Debug.Log("골드가 부족해! 나무를 심을 수 없어!");
            return;
        }

        //배치 대상 레이어(TreeSlot)만 감지하여 레이 발사
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = LayerMask.GetMask("TreeSlot");

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            TreeSlot slot = hit.collider.GetComponent<TreeSlot>();
            if (slot != null)
            {
                if (treePrefabs[currentSelectedTreeIndex] == null)//나무 프리팹이 연결되지 않았을 경우 오류 처리
                {
                    Debug.LogError("[시스템 오류] 배치할 나무 프리팹이 연결되지 않았어! 인스펙터를 확인해!");
                    return;
                }

                //나무 배치 및 골드 차감
                slot.PlantTree(treePrefabs[currentSelectedTreeIndex]);
                currentGold -= treeCost;

                //배치 완료 후 초기화
                isTreeSelected = false;
                Destroy(currentPreview);
                currentPreview = null;
                Debug.Log($"배치 완료! 남은 골드: {currentGold}");
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
        currentPreview = Instantiate(treePrefabs[currentSelectedTreeIndex]);//변수에 미리보기 이미지 오브젝트가 생성
        Collider col = currentPreview.GetComponent<Collider>();//미리보기 이미지는 충돌 처리가 필요 없으니 콜라이더를 끄자
        if (col != null)
        {
            col.enabled = false;    
        }

        Debug.Log($"{treePrefabs[currentSelectedTreeIndex].name}을(를) 선택했어!");
    }
}
