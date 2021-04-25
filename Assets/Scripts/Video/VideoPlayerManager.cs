using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage    rawImage;
    public ResultUI    resultUI;
    public string      videoClipName;

    private void Awake()
    {
        rawImage.enabled = false;
        resultUI?.onWinResultEnd.AddListener(Play);
        videoPlayer.clip = Resources.Load<VideoClip>("Videos/" + videoClipName);
    }

    public void Play()
    {
        videoPlayer.targetTexture.Release();
        rawImage.enabled  = true;
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
