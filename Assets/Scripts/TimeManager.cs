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

    private bool shouldBringBarDown;

 
    RectTransform progBar;
    Vector3 startPos;
    public float speed;

    // Reference to GameManager
    LoadIntro gameManager;

    void Start()
    {
        gameManager = GameObject.Find("SceneManager").GetComponent<LoadIntro>();
        startingTime = time;
        timerRunning = false;
        HideUIElements();

        //For progress bar
        progBar = progressBar.GetComponent<RectTransform>();
        startPos = progBar.transform.position;
        speed = -0.15f;
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
            UpdateBarPosition();
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
        IntroduceUIElements();
    }

    void EndTimer()
    {
        // Call endgame in our game manager script when we have no time left and stop the timer
        gameManager.EndGame();
        timerRunning = false;
        timerText.text = "";
        //hide progress bar
        HideUIElements();
    }

    public bool IsTimerRunning()
    {
        return timerRunning;
    }

    public void HideUIElements()
    {
        timerText.CrossFadeAlpha(0.0f, 0.0f, false);
        progressBar.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void IntroduceUIElements()
    {
        Debug.Log("Introducing UI elements");
        timerText.CrossFadeAlpha(1.0f, 5.0f, false);

        progressBar.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        shouldBringBarDown = true;
        progBar.transform.position = startPos;
    }

    public void UpdateBarPosition()
    {
        if (shouldBringBarDown)
        {
            StartCoroutine(BringBarDown(12.0f));
            shouldBringBarDown = false;
        }
    }

    IEnumerator BringBarDown(float distance)
    {
        float n = 0f;

        while(n < distance)
        {
            n += (-1 * speed);//reverse the translation
            progBar.Translate(0.0f, speed, 0.0f);
            yield return null;
        }
    }
}
