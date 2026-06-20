using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public GameObject[] treePrefabs;//여러 나무를 인스펙터에서 리스트로 관리
    public int currentSelectedTreeIndex = 0;//지금 선택된 나무 인덱스

    private bool isTreeSelected = false;//나무를 선택했는지 확인하는 상태 변수


    void Update()
    {
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

                    isTreeSelected = false;//배치 완료 후 선택 해제
                    Debug.Log("나무 배치 완료! 다음 treeSlot에 배치를 할때 다시 나무를 선택해줘!");
                }
            }
        }
    }

    public void SelectTree(int index)//버튼에 연결할 함수
    {
        //버튼에서 들어온 인덱스로 현재 선택된 나무를 바꿈
        currentSelectedTreeIndex = index;
        isTreeSelected = true;//버튼을 누르면 선택된 상태로 변경
        Debug.Log($"{treePrefabs[currentSelectedTreeIndex].name}을(를) 선택했어!");
    }
}
