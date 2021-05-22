using System;
using UI.ChineseSports.Battle;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Video
{
    /// <summary>
    ///     视频播放管理
    /// </summary>
    public class VideoPlayerManager : MonoBehaviour
    {
        public  VideoPlayer videoPlayer;
        public  RawImage    rawImage;
        public  ResultUI    resultUI;
        [SerializeField]private string      _videoClipName;

        public string VideoClipName
        {
            get => _videoClipName;
            set
            {
                _videoClipName   = value;
                videoPlayer.clip = Resources.Load<VideoClip>("Videos/" + VideoClipName);
            }
        }

        private void Awake()
        {
            rawImage.enabled = false;
            resultUI?.onWinResultEnd.AddListener(Play);
            videoPlayer.clip = Resources.Load<VideoClip>("Videos/" + VideoClipName);
        }

        public void Play()
        {
            videoPlayer.targetTexture.Release();
            rawImage.enabled = true;
            videoPlayer.Play();
        }

        public void Pause()
        {
            videoPlayer.Pause();
        }

        public void Stop()
        {
            videoPlayer.Stop();
            videoPlayer.frame = 0;
            videoPlayer.time  = 0;
            rawImage.enabled  = false;
            videoPlayer.targetTexture.Release();
        }
    }
}