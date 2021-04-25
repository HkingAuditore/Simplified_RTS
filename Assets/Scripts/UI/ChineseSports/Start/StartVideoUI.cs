using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class StartVideoUI : MonoBehaviour
{
    public VideoClip   formerVideo;
    public VideoClip   loopVideo;
    public VideoPlayer videoPlayer;
    public GameObject[]  startUIElements;

    private double _videoTime;

    public void Start()
    {
        _videoTime       = formerVideo.frameCount / formerVideo.frameRate;
        videoPlayer.clip = formerVideo;
        videoPlayer.Play();
        StartCoroutine(WaitForVideoEnd());
    }

    IEnumerator WaitForVideoEnd()
    {
        yield return new WaitForSeconds((float)_videoTime);
        videoPlayer.Stop();
        videoPlayer.clip      = loopVideo;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
        foreach (GameObject startUIElement in startUIElements)
        {
            startUIElement.SetActive(true);
        }
    }
}
