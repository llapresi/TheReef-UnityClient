using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetParent : MonoBehaviour {

    // Use this for initialization
    public TargetInfo targetInfo;
    public Collider Collider;
    public Renderer ourRenderer;

	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoHit()
    {
        StartCoroutine(ChangeColor());
    }

    IEnumerator ChangeColor()
    {
        Color originalColor = ourRenderer.material.GetColor("_Color");
        ourRenderer.material.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(0.6f);
        ourRenderer.material.SetColor("_Color", originalColor);
    }
}
