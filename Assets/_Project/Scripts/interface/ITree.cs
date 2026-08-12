using UnityEngine;

public interface ITree
{
    //인터페이스: "이 이름을 가진 스크립트는 무조건 'GetTreeData' 함수를 가지고 있어야 한다"는 계약서.
    //나무 종류가 무엇이든 이 규칙을 따르게 해서,
    //나중에 누가 불러도 똑같은 방식으로 데이터(SO)를 꺼낼 수 있게 만듦.

    //**인터페이스는 컴포넌트로 만들어서 드래그 앤 드롭하는 게 아니야!**

    TreeData GetTreeData();//이 인터페이스를 구현하는 모든 클래스는 반드시 GetTreeData 함수를 만들어야 해

}