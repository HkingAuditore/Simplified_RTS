﻿using Saver;
using UnityEngine;

namespace UI
{
    public class PausePanelUI : MonoBehaviour
    {
        public GameObject pauseButton;
        public GameObject pausePanel;

        private void Start()
        {
            GameManager.GameManager.GetManager.winEvent.AddListener(() => { gameObject.SetActive(false); });
            GameManager.GameManager.GetManager.loseEvent.AddListener(() => { gameObject.SetActive(false); });
        }

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
}