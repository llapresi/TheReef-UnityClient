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
    public int id;
}

[System.Serializable]
public class FireMessage
{
    public string type;
    public int id;
    //for later gripping functionality
    public bool held;
}

[System.Serializable]
public class CommonTypeMessage
{
    public string type;
}

[System.Serializable]
public class UserConnectMessage
{
    public string type;
    public int id;
}

[System.Serializable]
public class TargetInfoMessage
{
    public string type = "targetInfo";
    public string targetName;
    public string targetDescription;
    public int userID;
}
