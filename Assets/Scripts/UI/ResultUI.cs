using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public GameObject resultUI;
    public bool       hasItem;
    public Sprite     itemSprite;
    public string       itemName;

    public Text       resultText;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject resultPanel;
    public Image      itemImage;
    public Text       itemNameText;
    public GameObject itemUIGameObject;

    public void ShowResult(bool isWin)
    {
        if (isWin)
        {
            resultText.text = "胜利";
            
            if (hasItem)
            {
                itemImage.sprite  = itemSprite;
                itemNameText.text = itemName;
                itemUIGameObject.SetActive(true);
                winPanel.SetActive(true);
            }
            else
            {
                winPanel.SetActive(true);
            }
        }
        else
        {
            resultText.text = "失败";
            losePanel.SetActive(true);
 
        }
        
        resultUI.SetActive(true);
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

    public void ItemToWin()
    {
        itemUIGameObject.SetActive(false);
        resultPanel.SetActive(true);
    }
}