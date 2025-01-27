using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreboardUI : MonoBehaviour
{
    private static ScoreboardUI _Instance;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI HighscoreText;

    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public static void Display()
    {
        int score = GameManager.Score;
        int highscore = PlayerPrefs.GetInt("Highscore", 0);

        _Instance.gameObject.SetActive(true);
        _Instance.ScoreText.text = score.ToString();

        if (score > highscore)
        {
            _Instance.HighscoreText.text = "NEW HIGHSCORE!";
            PlayerPrefs.SetInt("Highscore", score);
        }
        else
        {
            _Instance.HighscoreText.text = "Highscore: " + highscore;
        }

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
