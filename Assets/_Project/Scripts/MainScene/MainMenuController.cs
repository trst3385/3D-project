using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public void LoadGameScene(string sceneName)//11.11 버튼 OnClick() 이벤트의 매개변수로 이동할 씬 이름(string)을 입력받음
    {                                          
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else Debug.LogError("이동할 씬 이름이 비어있어! 인스펙터 창을 확인해봐!");
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
