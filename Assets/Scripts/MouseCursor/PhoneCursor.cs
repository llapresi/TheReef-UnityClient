using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneCursor : MonoBehaviour {

    public RectTransform ourTransform;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        // Setting this to mouse for now
        ourTransform.anchoredPosition = Input.mousePosition;

    }
}
