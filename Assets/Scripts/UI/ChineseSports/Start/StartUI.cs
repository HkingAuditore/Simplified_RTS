using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
   public Button       skipButton;
   public StartVideoUI startVideoUI;

   private void Start()
   {
      startVideoUI.videoEndEvent.AddListener(() => skipButton.gameObject.SetActive(false));
   }

   public void InitToMap()
   {
      DataTransfer.GetDataTransfer.LoadSceneInLoadingScene("Map");

   }

   public void SkipVideo()
   {
      startVideoUI.ForceStop();
   }
   
}
