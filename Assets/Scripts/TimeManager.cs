using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {

    public Text timerText;

    public float time = 60; //default 90 seconds for now
	
	// Update is called once per frame
	void Update () {
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
            timerText.text = secondText;
        }

        EndGame();
    }

    void EndGame()
    {
        if (time <= 0)
        {
            //The game ends, so, do end game stuff
        }
        else if (time <= 30)
        {
            //Increase fall speed prolly
        }

    }
}
