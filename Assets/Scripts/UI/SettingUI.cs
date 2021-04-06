using System.Collections;
using System.Collections.Generic;
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
        _isActive                                 = !_isActive;
        sounds.sprite                             = _isActive ? enableSounds : disableSounds;
        Transformer.getTransformer.isSoundsActive = _isActive;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
