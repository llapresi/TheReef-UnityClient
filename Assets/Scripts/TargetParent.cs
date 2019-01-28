using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetParent : MonoBehaviour {

    // Use this for initialization
    public TargetInfo targetInfo;
    public SphereCollider collider;
    Renderer renderer;

	void Start () {
        renderer = GetComponent<Renderer>();

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
        Color originalColor = renderer.material.GetColor("_Color");
        renderer.material.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(0.6f);
        renderer.material.SetColor("_Color", originalColor);
    }
}
