using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour {

    private float totalTime;

    //Welcome To
    public Text introText;
    //The Reef
    public Text introTextTwo;

    public class TextTransition
    {
        public Text text;
        public bool shouldShow;
        public bool shouldTransition;

        public TextTransition(Text p_text)
        {
            text = p_text;
            shouldShow = true;
            shouldTransition = true;
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

        //Initialize our classes
         intro1 = new TextTransition(introText);
         intro2 = new TextTransition(introTextTwo);

    }
	
	// Update is called once per frame
	void Update () {
        ManageTransitions();
    }



    void ManageTransitions()
    {
        totalTime += Time.deltaTime;
        
        if (intro1.shouldTransition)
        {
            if (totalTime >= 2.0f && intro1.shouldShow)
            {
                intro1.FadeIn(1.0f);
                intro1.shouldShow = false;
            }
            if (totalTime >= 5.0)
            {
                intro1.FadeOut(1.0f);
                intro1.IsDone();
            }
        }
    }
}
