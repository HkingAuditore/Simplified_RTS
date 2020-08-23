using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkLobbyManager :MonoBehaviourPunCallbacks
{
    // private TypedLobby _customLobby = new TypedLobby("customLobby", LobbyType.Default);
    // private Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();

    public Transform panelContent;
    public GameObject roomLine;
    public Text pingText;
    private void Awake()
    {
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();

        Debug.Log("VAR");
        // PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions() {MaxPlayers = 2}, default);
        
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            SceneManager.LoadScene(sceneBuildIndex: 1);
        }
    }

    private List<RoomInfo> _roomList = new List<RoomInfo>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log("IN!");
        foreach (RoomInfo roomInfo in roomList)
        {
            Debug.Log(roomInfo.Name);
            if (roomInfo.RemovedFromList)
            {
                Destroy(panelContent.transform.Find(roomInfo.Name).gameObject);
            }
            else
            {
                _roomList.Add(roomInfo);
                GameObject lineRoom = Instantiate(roomLine, panelContent);
                if (lineRoom != null)
                {
                    lineRoom.name = roomInfo.Name;
                    var roomText = lineRoom.transform.Find("Text").GetComponent<Text>();
                    roomText.text = roomInfo.Name;
                    if (PhotonNetwork.CurrentRoom.Name == lineRoom.name)
                    {
                        roomText.color = new Color(1f, 0.34f, 0.44f);
                    }
                }
            }
        }
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.InRoom)
            return;
        string now = DateTime.Now.ToString("O");
        PhotonNetwork.JoinOrCreateRoom(now, new RoomOptions() {MaxPlayers = 2}, TypedLobby.Default);
        
        GameObject lineRoom = Instantiate(roomLine, panelContent);
        
        if (lineRoom != null)
        {
            lineRoom.name = now;
            var roomText = lineRoom.transform.Find("Text").GetComponent<Text>();
            roomText.text = now;
            roomText.color = new Color(1f, 0.34f, 0.44f);
        }

        Debug.Log("CREATE!");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
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

    private void Update()
    {
        pingText.text = "PING:" + PhotonNetwork.GetPing().ToString();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            SceneManager.LoadScene(sceneBuildIndex: 1);
        }
    }
}
