using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public GameObject resultUI;
    
    public void ShowResult(bool isWin)
    {
        resultUI.transform.Find("Text").GetComponent<Text>().text = isWin ? "你胜利了！" : "你败北了！";
        resultUI.SetActive(true);
    }

    public void BackToLobby()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LoadLevel(0);
            PhotonNetwork.LeaveRoom();
        }else if(PhotonNetwork.CurrentRoom == null)
            SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}
