using UnityEngine;

//네임스페이스로 분리했기 때문에, 프로젝트 내에 'GameManager'라는 이름이
//있어도 서로 충돌하지 않아! 오류 걱정은 안 해도 돼!!@@
//**네임스페이스는 클래스의 '주소'를 다르게 만들어주기 때문에, 이름이 같아도 유니티는 다른 존재로 인식해**
namespace Pure_GameManagerLogic
{
    public class GameManager//MonoBehaviour상속이 아닌 순수 C#클래스, !유니티 엔진 관련 기능 사용 불가!
    {
        //싱글톤의 핵심 패턴만 보관
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null) _instance = new GameManager();
                return _instance;
            }
        }
        private GameManager() { }

        /* [나중에 이 스크립트로 이사 갈 때 가져갈 핵심 기능 메모]
           1. 라운드 데이터 관리 (currentRoundData, roundDatas)
           2. 이벤트 시스템 (Action을 활용한 UI 통신)
           3. 몬스터 카운트 로직 (EnemyDefeated, LoadNextRound)
           
           * 주의: 이사 갈 때는 MonoBehaviour의 Awake나 Update를 
                   어떻게 대체할지(초기화 로직 등)를 같이 고민해야 해!!
        */
    }
}


