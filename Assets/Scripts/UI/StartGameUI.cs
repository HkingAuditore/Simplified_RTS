using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameUI : MonoBehaviour
{
    public void LaunchGame()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }
    
    public void LaunchGameOnline()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null &&
            PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(2);

        }
    }


}
