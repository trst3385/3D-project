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

        Instantiate(treePrefab, transform.position, Quaternion.identity);//나무 생성

        isOccupied = true;//배치된 상태로 변경
        Debug.Log("나무가 심어졌어!");

       
    }
}
