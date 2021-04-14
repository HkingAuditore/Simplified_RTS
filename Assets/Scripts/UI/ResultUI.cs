using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class ItemDict
{
    public Sprite itemSprite;
    public string itemName;
}
public class ResultUI : MonoBehaviour
{
    [Header("Win or Lose")]
    public GameObject resultPanel;
    public GameObject winPanel;
    public GameObject losePanel;

    
    [Header("Item")]
    [SerializeField]public List<ItemDict> itemDicts = new List<ItemDict>();
    public Image      itemImage;
    public Text       itemNameText;
    public GameObject itemPanel;

    private int _curState = 0;

    [Header("Global")]
    public GameObject root;
    public GameObject nextButton;
    public GameObject backButton;

    public int CurState
    {
        get => _curState;
        set
        {
            _curState = value > 0 ? value : 0;
            if (_curState <= itemDicts.Count)
            {
                ShowCurItem();
                ShowButton();
            }
            else
            {
                _curState = itemDicts.Count;
            }
        }
    }

    public void ShowResult(bool isWin)
    {
        root.SetActive(true);
        if (isWin)
        {
            winPanel.SetActive(true);
            resultPanel.SetActive(true);

        }
        else
        {
            losePanel.SetActive(true);
            resultPanel.SetActive(true);
            
        }
        ShowButton();
        root.SetActive(true);
    }

    private void ShowCurItem()
    {
        winPanel.SetActive(false);
        itemPanel.SetActive(true);
        itemImage.sprite  = itemDicts[CurState-1].itemSprite;
        itemNameText.text = itemDicts[CurState-1].itemName;
        

    }

    public void BackToLobby()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LoadLevel(0);
            PhotonNetwork.LeaveRoom();
        }
        else if (PhotonNetwork.CurrentRoom == null)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void OnNextClick()
    {
        CurState++;
    }

    public void ShowButton()
    {
        if (_curState == itemDicts.Count)
        {
            backButton.SetActive(true);
            nextButton.SetActive(false);

        }
        else
        {
            nextButton.SetActive(true);
            backButton.SetActive(false);

        }
    }

    public void Back()
    {
        
    }
    


}