using System.Collections;
using Controllers.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChineseSports.Main
{
    public class ShareUI : MonoBehaviour
    {
        public Image            screenShotImage;
        public CameraScreenShot cameraScreenShot;
        public GameObject       panel;


        private Sprite CreatSprite(Texture2D tex)
        {
            return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        private void ShowScreenShot()
        {
            cameraScreenShot.StartCoroutine(cameraScreenShot.GetCaptureScreen());
            StartCoroutine(WaitForScreenShot());
        }

        public void ClosePanel()
        {
            panel.SetActive(false);
        }

        private IEnumerator WaitForScreenShot()
        {
            yield return new WaitUntil(() => cameraScreenShot.isShotDone);
            screenShotImage.sprite = CreatSprite(cameraScreenShot.screenShot);
            panel.SetActive(true);
        }

        public void OnShotClick()
        {
            ShowScreenShot();
        }
    }
}