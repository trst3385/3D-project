using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadGameScene()//함수 자체에 씬 이름을 고정
    {                          //지금처럼 "GameScene" 으로 이 씬으로만 이동하게 만들었지만,
                               //하드코딩보단 매개변수로 활용해 버튼의 OnClick()에서 이 함수 하나로 여러씬으로 이동하게 하자
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()//게임 종료 버튼에 연결할 함수
    {                      
        Debug.Log("게임 종료!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
