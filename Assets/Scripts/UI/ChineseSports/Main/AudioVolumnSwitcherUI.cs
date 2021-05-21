using Saver;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChineseSports.Main
{
    public class AudioVolumnSwitcherUI : MonoBehaviour
    {
        public Sprite audioOn;
        public Sprite audioOff;
        public Image  audioImage;

        private void OnEnable()
        {
            audioImage.sprite = DataTransfer.GetDataTransfer.isSoundsActive ? audioOn : audioOff;
        }

        public void OnClick()
        {
            DataTransfer.GetDataTransfer.isSoundsActive = !DataTransfer.GetDataTransfer.isSoundsActive;
            audioImage.sprite                           = DataTransfer.GetDataTransfer.isSoundsActive ? audioOn : audioOff;
        }
    }
}
