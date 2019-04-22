using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class LoadIntro : MonoBehaviour {

    public TimeManager timer;
    public ItemSpawning items;
    public CoralManaging coral;

    private PostProcessVolume postProcessorScript;

    public bool skipIntro = false;
    bool isFullscreen = false;

    // Use this for initialization
    void Start () {
        if (!skipIntro)
        {
            BeginIntro();
        }
        else
        {
            BeginGame();
        }
        postProcessorScript = coral.postProcessorScript;
    }

    private void Update()
    {
        // Check if F key is pressed to go into Fullscreen
        // Make sure Nvidia Surround is setup so that Unity recognizes the two projectors as one monitor
        if(Input.GetKeyDown(KeyCode.F))
        {
            if (!isFullscreen)
            {
                Screen.SetResolution(3840, 1080, true);
                isFullscreen = true;
            }
            else
            {
                Screen.SetResolution(1280, 360, false);
                isFullscreen = false;
            }
        }
    }

    public void BeginGame()
    {
        // Called by StartingScene when it finished to actually start the game
        timer.StartTimer();
        items.ResetTrash();
    }

    public void EndGame()
    {
        items.StopSpawningTrash();
        StartCoroutine(LoadScene("EndingScene"));
    }

    public void BeginIntro()
    {
        StartCoroutine(UnloadScene(2));
        StartCoroutine(LoadScene("StartingSceneAnimationOnly"));
        //Decrement Weight value from its current weight, to fully clean reef
        StartCoroutine(DecrementWeightValue(postProcessorScript.weight, 0.0f));
    }


    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // Do stuff after scene loads here
            yield return null;
        }
    }

    IEnumerator UnloadScene(int index)
    {

        if(SceneManager.GetSceneByBuildIndex(index).isLoaded)
        {
            AsyncOperation asyncuUnload = SceneManager.UnloadSceneAsync(index);

            // Wait until the asynchronous scene fully loads
            while (!asyncuUnload.isDone)
            {
                // Do stuff after scene loads here
                yield return null;
            }
        }

    }

    //Gradually change the post processing weight
    IEnumerator DecrementWeightValue(float start, float end)
    {
        for (float f = start; f > end; f -= 0.005f)
        {
            postProcessorScript.weight = f;
            yield return null;
        }
    }
}
