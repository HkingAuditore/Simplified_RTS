using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
   }
}
