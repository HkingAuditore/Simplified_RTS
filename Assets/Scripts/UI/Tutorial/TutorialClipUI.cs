using System;
using System.Collections;
using Saver;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Tutorial
{
    public class TutorialClipUI : MonoBehaviour
    {
        public GameObject awakeGameObject;
        public bool       isWaitForAwakeGameObjectDisabled = false;
        public UnityEvent tutorialStartEvent               = new UnityEvent();
        public float      clickEventDelay                  = 0;
        public UnityEvent clickEvent                       = new UnityEvent();
        public UnityEvent closeEvent                       = new UnityEvent();
        public int        tutorialIndex;
        public float      maxStayTime = 60f;
        public GameObject tutorialPanel;

        private void Start()
        {
            DataTransfer.GetDataTransfer.tutorialManager.RegisterTutorialClip(this);
        }

        public void OnClick()
        {
            if(clickEventDelay < .001f)
                clickEvent.Invoke();
            else
            {
                StartCoroutine(WaitForClickDelayTime());
            }
        }

        private IEnumerator WaitForStayTime()
        {
            yield return new WaitForSeconds(maxStayTime);
            FinishTutorialClip();
        }
        private IEnumerator WaitForClickDelayTime()
        {
            yield return new WaitForSeconds(clickEventDelay);
            clickEvent.Invoke();
        }

        public void ShowTutorialClip()
        {
            tutorialPanel.SetActive(true);
            tutorialStartEvent.Invoke();
            StartCoroutine(WaitForStayTime());
        }

        public void FinishTutorialClip()
        {
            closeEvent.Invoke();
            try
            {
                StopCoroutine(WaitForStayTime());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            DataTransfer.GetDataTransfer.tutorialManager.GoToNextClip();
            gameObject.SetActive(false);
        }

        public void ChangeTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }
    }
}