using Saver;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChineseSports.Start
{
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
}