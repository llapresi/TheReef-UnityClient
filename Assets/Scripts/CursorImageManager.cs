using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorImageManager : MonoBehaviour {
    //This holds the referenced images
    public List<Sprite> cursorImages;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Sprite GetNewCursorImage()
    {
        //Return the first cursor image
        Sprite imageToReturn = cursorImages[0];
        cursorImages.Remove(imageToReturn);
        return imageToReturn;
    }

    public void GiveBackCursorImage(Sprite imageGottenBack)
    {
        //Add it back to the front of the list
        cursorImages.Insert(0,imageGottenBack);
    }
}
