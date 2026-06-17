using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    //여러 나무를 인스펙터에서 리스트로 관리
    public GameObject[] treePrefabs;
    public int currentSelectedTreeIndex = 0;//지금 선택된 나무 인덱스


    void Update()
    {
        if (Input.GetMouseButtonDown(0))//마우스 왼쪽 버튼 클릭 시
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//카메라에서 마우스 위치로 레이 발사
            RaycastHit hit;

            int layerMask = LayerMask.GetMask("TreeSlot");//"TreeSlot" 레이어만 감지하도록 설정 (LayerMask 필수!)

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                TreeSlot slot = hit.collider.GetComponent<TreeSlot>();
                if (slot != null)
                {
                    //@@미래의 나를 위한 친절한 로그@@
                    Debug.LogWarning($"[시스템 안내] 현재 {treePrefabs[currentSelectedTreeIndex].name} 배치 중. " +
                              "나중에 UI를 만들어 나무 선택 기능을 구현하면 여기에서 인덱스(currentSelectedTreeIndex)를 동적으로 변경할 것!");
                    if (treePrefabs[currentSelectedTreeIndex] == null)//인덱스가 비었을때. 빨간색 오류로 띄우기 (문제가 있거나 심각할 때)
                    {
                        Debug.LogError("[시스템 오류] 배치할 나무 프리팹이 연결되지 않았어! 인스펙터를 확인해!");
                    }

                    slot.PlantTree(treePrefabs[currentSelectedTreeIndex]);//현재 선택된 나무를 배치
                }
            }
        }
    }
}
