using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomUI : MonoBehaviourPunCallbacks
{
    public void JoinRoom()
    {
        if(!PhotonNetwork.IsConnected || PhotonNetwork.InRoom)
            return;
        PhotonNetwork.JoinRoom(this.gameObject.name);
        var roomText = this.transform.Find("Text").GetComponent<Text>();
        roomText.color = new Color(1f, 0.34f, 0.44f);
    }
}
