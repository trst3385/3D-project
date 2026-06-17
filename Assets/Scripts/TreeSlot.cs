using UnityEngine;

public class TreeSlot : MonoBehaviour
{
    public bool isOccupied = false;//나무가 심어져 있는지 확인

    public void PlantTree(GameObject treePrefab)
    {
        if (isOccupied) return;//이미 심어져 있으면 종료

        Instantiate(treePrefab, transform.position, Quaternion.identity);//나무 생성 (현재 슬롯 위치에)

        isOccupied = true;//슬롯 점유 상태로 변경
        Debug.Log("나무가 심어졌어!");
    }
}
