using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RotationMessage
{
    public string type;
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class CommonTypeMessage
{
    public string type;
}

[System.Serializable]
public class TargetInfoMessage
{
    public string type = "targetInfo";
    public string targetName;
    public string targetDescription;
}
