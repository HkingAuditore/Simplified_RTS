using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanelUI : MonoBehaviour
{
   public GameObject pauseButton;
   public GameObject pausePanel;
   public void Pause()
   {
      Time.timeScale = 0;
      pausePanel.SetActive(true);
      pauseButton.SetActive(false);
   }

   public void Continue()
   {
      Time.timeScale = 1;
      pauseButton.SetActive(true);
      pausePanel.SetActive(false);
   }

   public void BackToMain()
   {
      //TODO Back to main
      DataTransfer.GetDataTransfer.LoadSceneInLoadingScene("Map");
   }
}
