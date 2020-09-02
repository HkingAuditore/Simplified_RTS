using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkLobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject lobbyUI;
    public Transform lobbyPanelContent;
    public GameObject roomLine;
    public Text lobbyPingText;
    public Text playerName;
    public Text pingText;

    public GameObject roomUI;
    public GameObject playerA;
    public GameObject playerB;

    public Text roomPingText;

    private Text _playerANameText;
    private Text _playerBNameText;

    private void Awake()
    {
        _playerANameText = playerA.transform.Find("PlayerName").GetComponent<Text>();
        _playerBNameText = playerB.transform.Find("PlayerName").GetComponent<Text>();

        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 5;
        
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            roomPingText.text = "PING:" + PhotonNetwork.GetPing().ToString();
        }
        else
        {
            pingText.text = "PING:" + PhotonNetwork.GetPing().ToString();
        }
    }


    #region 连接

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();

        Debug.Log("VAR");
        // PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions() {MaxPlayers = 2}, default);
    }

    #endregion

    #region 大厅

    private List<RoomInfo> _roomList = new List<RoomInfo>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        //Debug.Log("IN!");
        UpdateRoomList(roomList);
    }

    //更新房间列表
    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            Debug.Log(roomInfo.Name);
            if (roomInfo.RemovedFromList)
            {
                Destroy(lobbyPanelContent.transform.Find(roomInfo.Name).gameObject);
            }
            else if (lobbyPanelContent.transform.Find(roomInfo.Name) != null)
            {
                continue;
            }
            else
            {
                _roomList.Add(roomInfo);
                GameObject lineRoom = Instantiate(roomLine, lobbyPanelContent);
                if (lineRoom != null)
                {
                    lineRoom.name = roomInfo.Name;
                    var roomText = lineRoom.transform.Find("Text").GetComponent<Text>();
                    roomText.text = roomInfo.Name;
                    // if (PhotonNetwork.CurrentRoom.Name == lineRoom.name)
                    // {
                    //     roomText.color = new Color(1f, 0.34f, 0.44f);
                    // }
                }
            }
        }
    }


    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.InRoom)
            return;
        PhotonNetwork.LocalPlayer.NickName = playerName.text;
        string now = DateTime.Now.ToString("T");
        PhotonNetwork.CreateRoom("[" + PhotonNetwork.LocalPlayer.NickName + "]" + now,
            new RoomOptions() {MaxPlayers = 2, PublishUserId = true, BroadcastPropsChangeToAll = true},
            TypedLobby.Default);

        GameObject lineRoom = Instantiate(roomLine, lobbyPanelContent);

        if (lineRoom != null)
        {
            lineRoom.name = "[" + PhotonNetwork.LocalPlayer.NickName + "]" + now;
            var roomText = lineRoom.transform.Find("Text").GetComponent<Text>();
            roomText.text = "[" + PhotonNetwork.LocalPlayer.NickName + "]" + now;
            // roomText.color = new Color(1f, 0.34f, 0.44f);
        }

        Debug.Log("CREATE!");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("ROOM CREATED!");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("FAILED TO CREATE ROOM!");
    }

    #endregion

    #region 房间

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        lobbyUI.SetActive(false);
        roomUI.SetActive(true);

        ShowPlayerInfo();

        // if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        // {
        //     SceneManager.LoadScene(sceneBuildIndex: 1);
        // }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        _playerBNameText.text = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        _playerBNameText.text = "待加入";
    }

    public void ShowPlayerInfo()
    {
        var playersArray = PhotonNetwork.CurrentRoom.Players.Values.ToArray();
        _playerANameText.text = playersArray[0].NickName;
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            _playerBNameText.text = playersArray[1].NickName;
    }

    public void QuitRoom()
    {
        PhotonNetwork.LeaveRoom();
        lobbyUI.SetActive(true);
        roomUI.SetActive(false);
    }

    #endregion
}