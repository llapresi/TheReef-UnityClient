﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIEndingManagerScript : MonoBehaviour {

    private float totalTime;
    private List<UITransition> uiTransitions;

    
    public Text introText;    //Welcome To
    public Text introTextTwo; //The Reef
    public Text descText;     //11.1 billion

    public LoadIntro parentSceneLoader;
    public float endTime = 38.0f;


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


    public class BubbleGroupTransition : UITransition
    {
        public UIBubbleImage parent;

        public BubbleGroupTransition(UIBubbleImage p_bubbleParent, float p_startTime, float p_endTime) : base(p_startTime, p_endTime)
        {
            parent = p_bubbleParent;
            parent.CrossFadeAlphaAll(0.0f, 0.0f, false);
        }

        public override void FadeIn(float duration)
        {
            parent.CrossFadeAlphaAll(1.0f, duration, false);

            // Start animations
            parent.PlayAnimators();
        }

        public override void FadeOut(float duration)
        {
            parent.CrossFadeAlphaAll(0.0f, duration, false);
        }

    }


    // Use this for initialization
    void Start()
    {
        // Grab out LoadIntro component from the main scene
        parentSceneLoader = GameObject.Find("SceneManager").GetComponent<LoadIntro>();

        totalTime = 0.0f;
        uiTransitions = new List<UITransition>();

        //(UI to display, time to enter, time to leave)
        uiTransitions.Add(new TextTransition(introText, 1.0f, 5.0f));
        uiTransitions.Add(new TextTransition(introTextTwo, 2.0f, 5.0f));
        uiTransitions.Add(new TextTransition(descText, 7.0f, 11.0f));
        
        // Update is called once per frame
    }
    void Update () {
        ManageTransitions();
        CheckIfOver();
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

    void CheckIfOver()
    {
        if (totalTime >= endTime)
        {
            if (parentSceneLoader != null)
            {
                parentSceneLoader.BeginIntro();
            }
        }
    }
}
