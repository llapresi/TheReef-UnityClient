using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour {

    public bool isHovered;
    public Color changingColor;
    private SpriteRenderer m_spriteRenderer;
    private float colorTransition;

	// Use this for initialization
	void Start () {
        isHovered = false;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        changingColor = Color.white;
        m_spriteRenderer.color = changingColor;
        colorTransition = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        changingColor = Color.Lerp(Color.white, changingColor, colorTransition);
        m_spriteRenderer.color = changingColor;
        AdjustColor();
        Debug.Log("colorTransition " + colorTransition);
    }

    void AdjustColor()
    {
        //Adjust the color to be a bit more green to signify its being hovered
        if (isHovered)
        {
            if (colorTransition <= 0.8)
            {
                colorTransition += .05f;
            }
        } 
        else {
            if (colorTransition > 0.0f)
            {
                colorTransition -= .05f;
            }
        }
    }
}
