using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
   public void InitToMap()
   {
      DataTransfer.GetDataTransfer.LoadSceneInLoadingScene("Map");

   }
}
