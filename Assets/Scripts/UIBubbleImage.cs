using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBubbleImage : MonoBehaviour
{

    public Animator[] animators;
    public UnityEngine.UI.Image[] images;

    public void PlayAnimators()
    {
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetBool("StartAnimation", true);
        }
    }

    public void CrossFadeAlphaAll(float p_alpha, float p_duration, bool p_ignoreTimeScale)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].CrossFadeAlpha(p_alpha, p_duration, p_ignoreTimeScale);
        }
    }
}
