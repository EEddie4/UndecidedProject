using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TileBoard board;
    public CanvasGroup gameOver;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    public AudioSource gameOverMusic;
    public AudioSource levelMusic;

    public TimerScript timer;
    
    public float powerUpDuration = 10.0f;

    private int score;

    private void Start()
    {
        NewGame();
    }

    public void NewGame ()
    {   
        SetScore(0);
        highScoreText.text = LoadHighScore().ToString();
        gameOver.alpha = 0f;
        gameOver.interactable = false; 

        board.ClearBoard();
        board.CreateNewTile();
        board.CreateNewTile();
        board.enabled = true;
        levelMusic.Play();

    }


    public void GameOver ()
    {

        board.enabled = false;
        gameOver.interactable = true;
        StartCoroutine(Fade(gameOver, 1f, 1f));
        levelMusic.Stop();
        gameOverMusic.Play();
        
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds (delay);

        float elapsed = 0;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed<duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;

    }


    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    private void SetScore (int score)
    {
        this.score = score;

        scoreText.text = score.ToString();
    }


    private void SaveHighScore ()
    {
        int highscore = LoadHighScore();

        if (score > highscore)
        PlayerPrefs.SetInt("highscore", score);
    }

    private int LoadHighScore ()
    {
        return PlayerPrefs.GetInt("highscore", 0);
    }

    public void SetTimeScale (float scale) 
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = scale* .02f;
    }

    public IEnumerator SlowDownTime()
    {
        if (score >= 500)
            score -= 500;
            SetTimeScale(.25f);
            yield return new WaitForSeconds(powerUpDuration);
            SetTimeScale(1.25f);


    }

    public void Freeze ()
    {
        StartCoroutine(SlowDownTime());
    }
}
