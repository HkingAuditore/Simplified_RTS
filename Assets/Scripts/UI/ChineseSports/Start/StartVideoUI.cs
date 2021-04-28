using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace UI.ChineseSports.Start
{
    public class StartVideoUI : MonoBehaviour
    {
        public  VideoClip    formerVideo;
        public  VideoClip    loopVideo;
        public  VideoPlayer  videoPlayer;
        public  GameObject[] startUIElements;
        public  UnityEvent   videoEndEvent = new UnityEvent();
        private double       _videoTime;

        public void Start()
        {
            _videoTime       = formerVideo.frameCount / formerVideo.frameRate;
            videoPlayer.clip = formerVideo;
            videoPlayer.Play();
            StartCoroutine(WaitForVideoEnd());
        }

        private IEnumerator WaitForVideoEnd()
        {
            yield return new WaitForSeconds((float) _videoTime);
            videoPlayer.Stop();
            videoPlayer.clip      = loopVideo;
            videoPlayer.isLooping = true;
            videoPlayer.Play();
            foreach (var startUIElement in startUIElements) startUIElement.SetActive(true);
            videoEndEvent.Invoke();
        }

        public void ForceStop()
        {
            StopCoroutine(WaitForVideoEnd());
            videoPlayer.Stop();
            videoPlayer.clip      = loopVideo;
            videoPlayer.isLooping = true;
            videoPlayer.Play();
            foreach (var startUIElement in startUIElements) startUIElement.SetActive(true);
            videoEndEvent.Invoke();
        }
    }
}