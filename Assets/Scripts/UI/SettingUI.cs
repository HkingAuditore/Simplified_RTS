﻿using Saver;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public Sprite disableSounds;
    public Sprite enableSounds;
    public Image  sounds;

    private bool _isActive;

    public void SetSounds()
    {
        _isActive                                   = !_isActive;
        sounds.sprite                               = _isActive ? enableSounds : disableSounds;
        DataTransfer.GetDataTransfer.isSoundsActive = _isActive;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}