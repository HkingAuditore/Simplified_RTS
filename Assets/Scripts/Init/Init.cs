using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    void Start()
    {
        DataTransfer.GetDataTransfer.LoadSceneInLoadingScene("Start");
    }

}
