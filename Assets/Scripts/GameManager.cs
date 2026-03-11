using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    [SerializeField] private Button pauseButton;

    [Header("Main Objects")]
    [SerializeField] private GameObject Base;
    [SerializeField] private GameObject player;

    [Header("Audio")]
    [SerializeField] private bool isAudioOn = true;

    //[Header("Settings")]
    //[SerializeField] private GameObject LightAudio;

    [Header("Spawn")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private GameObject enemyPrefab;

    [Header("Wave Settings")]
    [SerializeField] private int enemiesPerWave = 3;
    [SerializeField] private float delayBetweenWaves = 20f;

    private int currentWaveEnemiesLeft;
    private int totalEnemiesForLevel;

    private const string AUDIO_KEY = "Audio";
    private const string INDEX_KEY = "LevelIndex";

    [SerializeField] private int levelIndex;

    private string currentSceneName;

    private void Awake()
    {
        LoadSettings();
    }

    private void Start()
    {
        pausePanel.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);

        currentSceneName = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;

        totalEnemiesForLevel = 3 * levelIndex;

        if (totalEnemiesForLevel > 0)
            StartCoroutine(SpawnWaveRoutine());
    }

    private void LoadSettings()
    {
        // AUDIO
        isAudioOn = PlayerPrefs.GetInt(AUDIO_KEY, 1) == 1;
        AudioListener.pause = !isAudioOn;
        AudioLight(isAudioOn);
        levelIndex = PlayerPrefs.GetInt(INDEX_KEY, 1);

        Debug.Log("Loaded Level Index: " + levelIndex);
    }

    private void AudioLight(bool OnOffAudio)
    {
        //LightAudio.SetActive(OnOffAudio);
    }

    public void OnOffSound()
    {
        isAudioOn = !isAudioOn;
        AudioListener.pause = !isAudioOn;

        PlayerPrefs.SetInt(AUDIO_KEY, isAudioOn ? 1 : 0);
        PlayerPrefs.Save();
        AudioLight(isAudioOn);

    }

    #region UI

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

    public void Restart()
    {
        SceneManager.LoadScene(currentSceneName);
    }

    public void NextDay()
    {
        levelIndex++;
        PlayerPrefs.SetInt(INDEX_KEY, levelIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene(currentSceneName);
    }

    #endregion

    private void Update()
    {
        if (!player || !Base)
        {
            gameOverPanel.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            Time.timeScale = 0f;
        }
    }

    #region Save

    public void Save()
    {
        PlayerPrefs.SetInt(INDEX_KEY, levelIndex);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            Save();
    }

    #endregion

    #region Spawn
    private void SpawnEnemy()
    {
            int randomIndex = Random.Range(0, spawnPoints.Count);

            Transform point = spawnPoints[randomIndex];

            Vector3 direction = (Base.transform.position - point.position);
            direction.y = 0f;

            Quaternion rotation = Quaternion.LookRotation(direction);

            Instantiate(enemyPrefab, point.position, rotation);
    }

    private IEnumerator SpawnWaveRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (totalEnemiesForLevel > 0)
        {
            int waveSize = Mathf.Min(enemiesPerWave, totalEnemiesForLevel);
            currentWaveEnemiesLeft = waveSize;

            for (int i = 0; i < waveSize; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(1f);
            }

            while (currentWaveEnemiesLeft > 0)
            {
                yield return null;
            }

            if (totalEnemiesForLevel <= 0)
            {
                break;
            }

            yield return new WaitForSeconds(delayBetweenWaves);
        }

        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    #endregion

    public void EnemyKilled()
    {
        currentWaveEnemiesLeft--;
        totalEnemiesForLevel--;
    }
}
