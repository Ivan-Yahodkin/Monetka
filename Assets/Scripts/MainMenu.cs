using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private const string INDEX_KEY = "LevelIndex";

    public void NewGame()
    {
        PlayerPrefs.DeleteKey(INDEX_KEY);
        PlayerPrefs.SetInt(INDEX_KEY, 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(1);
    }
}
