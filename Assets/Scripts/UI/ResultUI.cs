using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class ItemDict
{
    public Sprite itemSprite;
    public string itemName;
    public int    itemIndex;
}
public class ResultUI : MonoBehaviour
{
    [Header("Win or Lose")] public bool       isWin;
    public                         GameObject resultPanel;
    public                         GameObject winPanel;
    public                         GameObject losePanel;

    
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
    public GameObject videoManager;

    public UnityEvent onWinResultEnd = new UnityEvent();

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
                if (isWin)
                {
                    if(videoManager!=null)
                        videoManager.SetActive(true);

                    onWinResultEnd.Invoke();
                }
            }
        }
    }

    public void ShowResult()
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

    public void BackToMap()
    {
        DataTransfer.GetDataTransfer.xmlSaver.SaveData();
        DataTransfer.GetDataTransfer.LoadSceneInLoadingScene("Map");
        // SceneManager.LoadScene("Map");
    }

    public void OnNextClick()
    {
        CurState++;
    }

    public void ShowButton()
    {
        if (!isWin)
        {
            nextButton.SetActive(false);
            backButton.transform.GetChild(0).GetComponent<Text>().text = "重 整 旗 鼓";
            backButton.SetActive(true);
            return;
        }
        if (_curState > itemDicts.Count)
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
    
    


}