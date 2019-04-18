using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class UIManagerScript : MonoBehaviour {

    private float totalTime;
    private List<UITransition> uiTransitions;
    private bool shouldStartGame;

    private bool shouldKillReef;
    private PostProcessVolume postProcessorScript;


    public Text introText;    //Welcome To
    public Text introTextTwo; //The Reef
    public Text descText;     //11.1 billion
    public Text descTextTwo;  //You can change
    public Text prompt;       //Collect plastic
    public Text final;        //Journey

    public Text bottleText;
    public Text bubbleText;
    public Text phoneText;

    //public Image bottle;
    public UIBubbleImage bottle;
    public UIBubbleImage bubble;
    public UIBubbleImage phone;
    public LoadIntro parentSceneLoader;
    public float endTime = 41.0f;


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

        postProcessorScript = parentSceneLoader.coral.postProcessorScript;
        shouldKillReef = true;
        shouldStartGame = false;
        //Reset the weight to make it clean again
        postProcessorScript.weight = 0.0f;

        totalTime = 0.0f;
        uiTransitions = new List<UITransition>();

        //(UI to display, time to enter, time to leave)
        uiTransitions.Add(new TextTransition(introText, 1.0f, 5.0f));
        uiTransitions.Add(new TextTransition(introTextTwo, 2.0f, 5.0f));
        uiTransitions.Add(new TextTransition(descText, 7.0f, 11.0f));
        uiTransitions.Add(new TextTransition(descTextTwo, 12.0f, 16.0f));
        uiTransitions.Add(new TextTransition(prompt, 17.0f, 22.0f));
        //Final prompt happens at 31 seconds. Lets start off with the reef all pretty, then kill it.
        uiTransitions.Add(new TextTransition(final, 34.0f, 38.0f)); // 31, 35

        //uiTransitions.Add(new ImageTransition(bottle, 23.0f, 30.0f));

        uiTransitions.Add(new TextTransition(bottleText, 23.0f, 25.0f));
        uiTransitions.Add(new TextTransition(bubbleText, 26.0f, 28.0f));
        uiTransitions.Add(new TextTransition(phoneText, 29.0f, 31.0f));

        uiTransitions.Add(new BubbleGroupTransition(bottle, 23.0f, 32.0f));
        uiTransitions.Add(new BubbleGroupTransition(bubble, 25.0f, 32.0f));
        uiTransitions.Add(new BubbleGroupTransition(phone, 28.0f, 32.0f));

        /*
        //Old Transition times
        uiTransitions.Add(new TextTransition(bottleText, 23.0f, 25.0f));
        uiTransitions.Add(new TextTransition(bubbleText, 25.0f, 27.0f));
        uiTransitions.Add(new TextTransition(phoneText, 27.0f, 29.0f));

        uiTransitions.Add(new BubbleGroupTransition(bottle, 23.0f, 30.0f));
        uiTransitions.Add(new BubbleGroupTransition(bubble, 25.0f, 30.0f));
        uiTransitions.Add(new BubbleGroupTransition(phone, 27.0f, 30.0f));
        */


        // Update is called once per frame
    }
    void Update () {
        PlayOnPress();
        ManageTransitions();
        CheckIfOver();
    }

    void ManageTransitions()
    {
        if (shouldStartGame)
        {
            totalTime += Time.deltaTime;
        }

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

        KillReef();
        
        
    }//End Manage Transitions

    void CheckIfOver()
    {
        if (totalTime >= endTime)
        {
            if (parentSceneLoader != null)
            {
                parentSceneLoader.BeginGame();
                StartCoroutine(UnloadIntro());
            }
        }
    }

    IEnumerator UnloadIntro()
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync("StartingSceneAnimationOnly");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // Do stuff after scene loads here
            yield return null;
        }
    }

    //Run this once in the beginning to kill the reef at a certain time slot
    void KillReef()
    {
        if (shouldKillReef && totalTime >= 34.0f)
        {
            StartCoroutine(IncrementWeightValue(0.0f, 1.0f));
            shouldKillReef = false;
        }
    }

    void PlayOnPress()
    {
        if (Input.GetKeyDown("space"))
        {
            shouldStartGame = true;
        }
    }

    //Decrement from the start to the end point over time
    //Gradually change the post processing weight
    IEnumerator IncrementWeightValue(float start, float end)
    {
        for (float f = start; f < end; f += 0.005f)
        {
            postProcessorScript.weight = f;
            yield return null;
        }
    }
}
