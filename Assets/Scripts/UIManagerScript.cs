using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour {

    private float totalTime;
    private List<TextTransition> textTransitions;

    
    public Text introText;    //Welcome To
    public Text introTextTwo; //The Reef
    public Text descText;     //11.1 billion
    public Text descTextTwo;  //You can change
    public Text prompt;       //Collect plastic


    public class TextTransition
    {
        public Text text;
        public bool shouldShow;
        public bool shouldTransition;
        public float startTime;
        public float endTime;

        public TextTransition(Text p_text,float p_startTime, float p_endTime)
        {
            text = p_text;
            shouldShow = true;
            shouldTransition = true;
            startTime = p_startTime;
            endTime = p_endTime;

            //Set intial alpha to 0
            text.CrossFadeAlpha(0.0f, 0.0f, false);
        }

        public void FadeIn(float duration)
        {
            text.CrossFadeAlpha(1.0f, duration, false);
        }

        public void FadeOut(float duration)
        {
            text.CrossFadeAlpha(0.0f, duration, false);
        }

        public void IsDone()
        {
            shouldTransition = false;
        }
    }

    private TextTransition intro1;
    private TextTransition intro2;

    // Use this for initialization
    void Start () {
        totalTime = 0.0f;
        textTransitions = new List<TextTransition>();
        
        //(Text to display, time to enter, time to leave)
        textTransitions.Add(new TextTransition(introText, 1.0f, 5.0f));
        textTransitions.Add(new TextTransition(introTextTwo, 2.0f, 5.0f));
        textTransitions.Add(new TextTransition(descText, 7.0f, 11.0f));
        textTransitions.Add(new TextTransition(descTextTwo, 12.0f, 16.0f));
        textTransitions.Add(new TextTransition(prompt, 17.0f, 22.0f));

    }
	
	// Update is called once per frame
	void Update () {
        ManageTransitions();
    }



    void ManageTransitions()
    {
        totalTime += Time.deltaTime;

        foreach( TextTransition t in textTransitions)
        {
            if (t.shouldTransition)
            {
                if (totalTime >= t.startTime && t.shouldShow)
                {
                    t.FadeIn(1.0f);
                    t.shouldShow = false;
                    Debug.Log("Enter 1st text");
                }
                if (totalTime >= t.endTime)
                {
                    t.FadeOut(1.0f);
                    t.IsDone();
                    Debug.Log("Leave 1st text");
                }
            }
        }
        
    }
}
