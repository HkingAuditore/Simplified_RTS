using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public Button levelButton;
    public Image  cloud;
    public string battleSceneName;
    
    [SerializeField]
    private bool   _isRevealed;

    public bool IsRevealed
    {
        get => _isRevealed;
        set
        {
            _isRevealed = value;
            SetCloud();
        }
    }

    [ContextMenu("SetCloud")]
    public void SetCloud()
    {
        levelButton.interactable = IsRevealed;
        cloud.gameObject.SetActive(!IsRevealed);
    }

    private void Start()
    {
        SetCloud();
    }
    
    
}
