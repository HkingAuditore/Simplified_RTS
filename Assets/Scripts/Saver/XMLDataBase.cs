using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class XMLDataBase
{
    public static string DataName = "UserData";
    public static string XMLPath
    {
        get => Application.dataPath + "/Data/";
    }
}
