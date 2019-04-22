using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class UIManagerScript : MonoBehaviour {

    private float totalTime;
    private List<UITransition> uiTransitions;
    private List<BubbleGroupTransition> bubbleTransitions;
    private bool shouldStartGame;
    private Vector3[] SpawnPoints;
    public float yOffset = 0.0f;

    private bool shouldKillReef;
    private PostProcessVolume postProcessorScript;

    public GameObject[] trashPrefabs;
    private bool shouldSpawnPreGameTrash;


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

    public float endTime = 42.0f;
    private float reefKillTime = 35.0f;


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
        public float peekTime;
        public bool shouldPeek;

        public BubbleGroupTransition(UIBubbleImage p_bubbleParent, float p_startTime, float p_endTime,float p_peekTime) : base(p_startTime, p_endTime)
        {
            parent = p_bubbleParent;
            parent.CrossFadeAlphaAll(0.0f, 0.0f, false);
            peekTime = p_peekTime;
            shouldPeek = true;
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

        public void PeekIn(float duration, float opacity)
        {
            //Only peek in once
            if (shouldPeek)
            {
                parent.CrossFadeAlphaAll(opacity, duration, false);
                shouldPeek = false;
            }
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
        shouldSpawnPreGameTrash = true;
        CreateSpawnPoints();


        totalTime = 0.0f;
        uiTransitions = new List<UITransition>();
        bubbleTransitions = new List<BubbleGroupTransition>();

        //(UI to display, time to enter, time to leave)
        uiTransitions.Add(new TextTransition(introText, 1.0f, 5.0f));
        uiTransitions.Add(new TextTransition(introTextTwo, 2.0f, 5.0f));
        uiTransitions.Add(new TextTransition(descText, 7.0f, 11.0f));
        uiTransitions.Add(new TextTransition(descTextTwo, 12.0f, 16.0f));
        uiTransitions.Add(new TextTransition(prompt, 17.0f, 21.0f));
        //Final prompt happens at 31 seconds. Lets start off with the reef all pretty, then kill it.
        uiTransitions.Add(new TextTransition(final, reefKillTime, 41.0f)); // 31, 35

        //uiTransitions.Add(new ImageTransition(bottle, 23.0f, 30.0f));

        uiTransitions.Add(new TextTransition(bottleText, 22.0f, 25.0f));
        uiTransitions.Add(new TextTransition(bubbleText, 26.0f, 29.0f));
        uiTransitions.Add(new TextTransition(phoneText, 30.0f, 33.0f));

        //Now has a time where it will peak into view
        //first one comes in without the peek, so add it to ui transitions
        uiTransitions.Add(new BubbleGroupTransition(bottle, 22.0f, 34.0f, 24.0f));
        bubbleTransitions.Add(new BubbleGroupTransition(bubble, 26.0f, 34.0f, 22.0f));
        bubbleTransitions.Add(new BubbleGroupTransition(phone, 30.0f, 34.0f, 22.0f));

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
        SpawnPreGameTrash();
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
        
        foreach (BubbleGroupTransition b in bubbleTransitions)
        {
            if (b.shouldTransition)
            {
                if (totalTime >= b.startTime && b.shouldShow)
                {
                    b.FadeIn(1.0f);
                    b.shouldShow = false;
                }
                if (totalTime >= b.endTime)
                {
                    b.FadeOut(1.0f);
                    b.IsDone();
                }
            }
            if (totalTime >= b.peekTime)
            {
                //duration, opacity
                b.PeekIn(1.0f, 0.3f);
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
        if (shouldKillReef && totalTime >= reefKillTime)
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

    //Gradually change the post processing weight
    IEnumerator IncrementWeightValue(float start, float end)
    {
        for (float f = start; f < end; f += 0.005f)
        {
            postProcessorScript.weight = f;
            yield return null;
        }
    }

    void CreateSpawnPoints()
    {
        SpawnPoints = new Vector3[5];

        SpawnPoints[0] = new Vector3 (-45.7f, yOffset + 25.7f, 0.0f);
        SpawnPoints[1] = new Vector3(0.0f, yOffset + 6f, 0.0f);
        SpawnPoints[2] = new Vector3(47.3f, yOffset + 18.9f, 0.0f);
        SpawnPoints[3] = new Vector3(-94.4f, yOffset + -3.2f, 0.0f);
        SpawnPoints[4] = new Vector3(101f, yOffset + 23.03f, 0.0f);
    }

    void SpawnPreGameTrash()
    {
        if (totalTime >= reefKillTime && shouldSpawnPreGameTrash)
        {
            for (int i = 0; i < SpawnPoints.Length; i++)
            {
                //Pick a random trash object to instantiate
                GameObject tempObj = Instantiate(
                    trashPrefabs[Random.Range(0, (trashPrefabs.Length))],
                    SpawnPoints[i],
                    Quaternion.identity);

                //Get Parents location

                // xRandomRange = 15.0f;
                //yRandomRange = 50.0f;

                //Stagger the locations slightly, ideally this would be done with 1/16 of the scene width
                //randomPosition.x += Random.Range((-xRandomRange), xRandomRange);
                //randomPosition.y += Random.value * yRandomRange;

                //Set that to our bottles location

                //Also give it a slight rotation
                Vector3 tweakedRotation = tempObj.transform.eulerAngles;
                tweakedRotation.z = (float)(Random.Range(0, 45) - 22.5);
                tempObj.transform.eulerAngles = tweakedRotation;
            }

            shouldSpawnPreGameTrash = false;
        }
        
    }
}
