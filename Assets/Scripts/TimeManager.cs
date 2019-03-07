using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {

    public Text timerText;

    public float time = 90; //default 90 seconds for now
	
	// Update is called once per frame
	void Update () {
        time -= Time.deltaTime;

        float minutes = time / 60;
        float seconds = time % 60;

        //update label value
        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);

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
