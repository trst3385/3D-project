using UnityEngine;

public class TreeSlot : MonoBehaviour
{
    public bool isOccupied = false;//나무가 심어져 있는지 확인

    public void PlantTree(GameObject treePrefab)
    {
        if (isOccupied)//이미 심어져 있으면 종료
        {
            Debug.Log("여기는 이미 나무가 심어져 있어!");
            return;
        }

        //나무를 생성할 때, 생성된 나무에게 이 슬롯 정보를 알려줄 거야
        GameObject newTree = Instantiate(treePrefab, transform.position, Quaternion.identity);

        //새로 심은 나무가 이 슬롯 자리를 기억하게 함
        TreeHealth health = newTree.GetComponent<TreeHealth>();
        if (health != null)
        {
            health.SetSlot(this);
        }

        isOccupied = true;//배치된 상태로 변경
        Debug.Log("나무가 심어졌어!");
    }

    public void ClearSlot()//나무가 파괴된 후 TreeSlot에 빈자리 확인
    {
        isOccupied = false;
        Debug.Log("나무가 파괴되서 슬롯이 비었어! 다시 심어!");
    }
}
