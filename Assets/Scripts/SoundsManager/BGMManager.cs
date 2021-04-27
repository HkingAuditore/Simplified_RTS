using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioClip   normalBgm;
    public AudioClip   resultBgm;
    public AudioSource audioSource;

    private void Start()
    {
        GameManager.GameManager.GetManager.winEvent.AddListener((() =>
                                                                 {
                                                                     audioSource.clip = resultBgm;
                                                                     if(DataTransfer.GetDataTransfer.isSoundsActive)
                                                                        audioSource.Play();
                                                                 }));
        GameManager.GameManager.GetManager.loseEvent.AddListener((() =>
                                                                 {
                                                                     audioSource.clip = resultBgm;
                                                                     if(DataTransfer.GetDataTransfer.isSoundsActive)
                                                                         audioSource.Play();

                                                                 }));
    }
}
