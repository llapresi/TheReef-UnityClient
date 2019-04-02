using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour {

    private float totalTime;
    private List<UITransition> uiTransitions;

    
    public Text introText;    //Welcome To
    public Text introTextTwo; //The Reef
    public Text descText;     //11.1 billion
    public Text descTextTwo;  //You can change
    public Text prompt;       //Collect plastic
    public Text final;        //Journey

    public Image bottle;
    public Image bubble;
    public Image phone;


    public class UITransition
    {
        //public Text text;
        public bool shouldShow;
        public bool shouldTransition;
        public float startTime;
        public float endTime;

        public UITransition(float p_startTime, float p_endTime)
        {
            //text = p_text;
            shouldShow = true;
            shouldTransition = true;
            startTime = p_startTime;
            endTime = p_endTime;
        }

        public virtual void FadeIn(float duration)
        {
            //text.CrossFadeAlpha(1.0f, duration, false);
        }

        public virtual void FadeOut(float duration)
        {
            //text.CrossFadeAlpha(0.0f, duration, false);
        }

        public void IsDone()
        {
            shouldTransition = false;
        }
    }

    public class TextTransition : UITransition
    {
        public Text text;

        public TextTransition(Text p_text, float p_startTime, float p_endTime) : base(p_startTime, p_endTime)
        {
            text = p_text;

            //Set intial alpha to 0
            text.CrossFadeAlpha(0.0f, 0.0f, false);
        }

        public override void FadeIn(float duration)
        {
            text.CrossFadeAlpha(1.0f, duration, false);
        }

        public override void FadeOut(float duration)
        {
            text.CrossFadeAlpha(0.0f, duration, false);
        }

    }

    public class ImageTransition : UITransition
    {
        public Image image;

        public ImageTransition(Image p_image, float p_startTime, float p_endTime) : base(p_startTime, p_endTime)
        {
            image = p_image;

            //Set intial alpha to 0
            image.CrossFadeAlpha(0.0f, 0.0f, false);
        }

        public override void FadeIn(float duration)
        {
            image.CrossFadeAlpha(1.0f, duration, false);
        }

        public override void FadeOut(float duration)
        {
            image.CrossFadeAlpha(0.0f, duration, false);
        }

    }


    // Use this for initialization
    void Start()
    {
        totalTime = 0.0f;
        uiTransitions = new List<UITransition>();

        //(UI to display, time to enter, time to leave)
        uiTransitions.Add(new TextTransition(introText, 1.0f, 5.0f));
        uiTransitions.Add(new TextTransition(introTextTwo, 2.0f, 5.0f));
        uiTransitions.Add(new TextTransition(descText, 7.0f, 11.0f));
        uiTransitions.Add(new TextTransition(descTextTwo, 12.0f, 16.0f));
        uiTransitions.Add(new TextTransition(prompt, 17.0f, 22.0f));
        uiTransitions.Add(new TextTransition(final, 31.0f, 35.0f));

        uiTransitions.Add(new ImageTransition(bottle, 23.0f, 30.0f));
        uiTransitions.Add(new ImageTransition(bubble, 23.0f, 30.0f));
        uiTransitions.Add(new ImageTransition(phone, 23.0f, 30.0f));
        
        // Update is called once per frame
    }
    void Update () {
        ManageTransitions();
    }



    void ManageTransitions()
    {
        totalTime += Time.deltaTime;

        foreach (UITransition t in uiTransitions)
        {
            if (t.shouldTransition)
            {
                if (totalTime >= t.startTime && t.shouldShow)
                {
                    t.FadeIn(1.0f);
                    t.shouldShow = false;
                }
                if (totalTime >= t.endTime)
                {
                    t.FadeOut(1.0f);
                    t.IsDone();
                }
            }
        }
        
        
    }//End Manage Transitions
}
