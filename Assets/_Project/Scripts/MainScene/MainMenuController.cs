using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadGameScene()//함수 자체에 씬 이름을 고정
    {
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
