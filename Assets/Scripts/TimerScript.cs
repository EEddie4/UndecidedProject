using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public float TimeLeft;
    public bool TimerOn = false;

    public TextMeshProUGUI TimerTxt;

    public GameManager gameManager;

    public float powerUpDuration = 10.0f;
   
    void Start()
    {
        TimerOn = true;
    }

    void Update()
    {
        if(TimerOn)
        {
            if(TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);
            }
            else
            {
                Debug.Log("Time is UP!");
                TimeLeft = 0;
                TimerOn = false;
                gameManager.GameOver();

            }
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        TimerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /*public void SetTimeScale (float scale) 
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = scale* .02f;
    }

    public IEnumerator SlowDownTime()
    {
        gameManager.score -= 
        SetTimeScale(.5f);
        yield return new WaitForSeconds(powerUpDuration);
    }*/

}