using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomUI : MonoBehaviourPunCallbacks
{
    private Text _playerName;

    private void Start()
    {
        _playerName = this.transform.parent.parent.parent.parent.Find("PlayerName").Find("Text").GetComponent<Text>();
    }

    public void JoinRoom()
    {
        if(!PhotonNetwork.IsConnected || PhotonNetwork.InRoom)
            return;
        PhotonNetwork.LocalPlayer.NickName = _playerName.text;
        PhotonNetwork.JoinRoom(this.gameObject.name);
        var roomText = this.transform.Find("Text").GetComponent<Text>();
        roomText.color = new Color(1f, 0.34f, 0.44f);
    }
}
