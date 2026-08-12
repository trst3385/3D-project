using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    private TreeData _treeData;//타워데이터 참조, SO자동연결 상태(이 스크립트가 들어있는 오브젝트의 나무 스크립트의 SO를 받아옴)
    private int _currentHitCount = 0;//현재 맞은 횟수
    private TreeSlot _mySlot;//나무가 심어진 슬롯 정보

    void Awake()
    {
        //GetComponent<ITree>(): 
        //"내 오브젝트에 붙어있는 스크립트 중에서, ITree 계약서를 지키고 있는(즉, GetTreeData 함수를 가진) 스크립트가 누구야?",
        //라고 물어보고 그 녀석의 주소를 가져와.
        ITree treeScript = GetComponent<ITree>();//ITree(나무 데이터 제공자) 역할을 하는 컴포넌트가 있니?

        if (treeScript != null)//찾았다면 그 스크립트(Itree)가 가지고 있는 'GetTreeData()' 함수를 실행해서 데이터(SO)를 가져옴.
        {
            _treeData = treeScript.GetTreeData();
        }
        else
        {
            Debug.LogError($"{gameObject.name}에 데이터를 가진 나무 스크립트가 없습니다!");
        }
    }
    public void TakeDamage()//몬스터가 공격했을 때 호출될 함수
    {
        _currentHitCount++;
        Debug.Log($"{gameObject.name}이 공격당함! 현재 맞은 횟수: {_currentHitCount}");

        if (_currentHitCount >= _treeData.MaxHitCount)
        {
            DestroyTower();//0이 되면 파괴
        }
    }

    public void SetSlot(TreeSlot slot)//TreeSlot이 나무를 심을 때 이 함수를 호출해줌
    {
        _mySlot = slot;
    }

    private void DestroyTower()
    {
        if (_mySlot != null)//파괴되기 직전에 슬롯에게 "나 이제 사라져, 자리 비워줘!"라고 알려줌
        {
            _mySlot.ClearSlot();//TreeSlot 스크립트의 ClearSlot()에게 슬롯에 자리가 비었다 알림
        }

        Debug.Log("타워가 파괴됐어!");
        //나중에 여기다가 이펙트 재생(Particle)이나 사운드 재생 코드를 추가하기 매우 편함
        Destroy(gameObject);
    }
}
