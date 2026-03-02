using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject Base;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gameOverPanel;

    private string currentSceneName;

    private void Start()
    {
        pausePanel.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        gameOverPanel.SetActive(false);
        currentSceneName = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        Time.timeScale = 0f;
    }

    public void Continue()
    {
        pausePanel.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!player || !Base)
        {
            gameOverPanel.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            Time.timeScale = 0f;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(currentSceneName);
    }
}
