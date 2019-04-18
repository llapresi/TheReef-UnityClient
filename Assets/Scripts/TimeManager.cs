using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {

    public bool timerRunning;
    public Text timerText;
    public Slider progressBar;

    public float time = 60; //default 90 seconds for now
    float startingTime;

    // Reference to GameManager
    LoadIntro gameManager;

    void Start()
    {
        gameManager = GameObject.Find("SceneManager").GetComponent<LoadIntro>();
        startingTime = time;
        timerRunning = false;
        progressBar.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update ()
    {
        if(timerRunning)
        {
            time -= Time.deltaTime;

            float minutes = Mathf.Floor(time / 60);
            int seconds = Mathf.RoundToInt(time % 60);

            string minuteText = minutes.ToString("#0");
            string secondText = seconds.ToString("00");

            //update label value
            //Display minutes only if theres a minute to display
            if (time > 60)
            {
                timerText.text = minuteText + ":" + secondText;
            } else
            {
                timerText.text = "0:" + secondText;
            }
            if(time <= 0)
            {
                EndTimer();
            }

            UpdateProgressBar();
        }
    }

    public void UpdateProgressBar()
    {
        //Turn our time to a %
        float progress = (1 - (time / startingTime));
        progressBar.value = (progress);
    }

    public void StartTimer()
    {
        time = startingTime;
        timerRunning = true;
        progressBar.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    void EndTimer()
    {
        // Call endgame in our game manager script when we have no time left and stop the timer
        gameManager.EndGame();
        timerRunning = false;
        timerText.text = "";
        //hide progress bar
        progressBar.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public bool IsTimerRunning()
    {
        return timerRunning;
    }
}
