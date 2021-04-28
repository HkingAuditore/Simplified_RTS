using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    /// <summary>
    ///     网络对战管理
    /// </summary>
    public class NetworkLobbyManager : MonoBehaviourPunCallbacks
    {
        /// <summary>
        ///     大厅UI
        /// </summary>
        public GameObject lobbyUI;

        /// <summary>
        ///     大厅卷动栏目
        /// </summary>
        public Transform lobbyPanelContent;

        /// <summary>
        ///     房间行
        /// </summary>
        public GameObject roomLine;

        /// <summary>
        ///     大厅加载延迟Text
        /// </summary>
        public Text lobbyPingText;

        /// <summary>
        ///     玩家名
        /// </summary>
        public Text playerName;

        /// <summary>
        ///     玩家延迟
        /// </summary>
        public Text pingText;

        /// <summary>
        ///     房间UI
        /// </summary>
        public GameObject roomUI;

        /// <summary>
        ///     玩家A
        /// </summary>
        public GameObject playerA;

        /// <summary>
        ///     玩家B
        /// </summary>
        public GameObject playerB;

        /// <summary>
        ///     房间延迟
        /// </summary>
        public Text roomPingText;

        private Text _playerANameText;
        private Text _playerBNameText;

        private void Awake()
        {
            _playerANameText = playerA.transform.Find("PlayerName").GetComponent<Text>();
            _playerBNameText = playerB.transform.Find("PlayerName").GetComponent<Text>();

            PhotonNetwork.GameVersion            = "1";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.SendRate               = 20;
            PhotonNetwork.SerializationRate      = 5;

            PhotonNetwork.ConnectUsingSettings();
        }

        private void Update()
        {
            if (PhotonNetwork.InRoom)
                roomPingText.text = "PING:" + PhotonNetwork.GetPing();
            else
                pingText.text = "PING:" + PhotonNetwork.GetPing();
        }


        #region 连接

        /// <summary>
        ///     连接至主服务器
        /// </summary>
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            PhotonNetwork.JoinLobby(new TypedLobby("TestLobby", default));

            Debug.Log("VAR");
            // PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions() {MaxPlayers = 2}, default);
        }

        #endregion

        #region 大厅

        private readonly List<RoomInfo> _roomList = new List<RoomInfo>();

        /// <summary>
        ///     房间列表更新
        /// </summary>
        /// <param name="roomList"></param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
            //Debug.Log("IN!");
            UpdateRoomList(roomList);
        }

        //更新房间列表
        private void UpdateRoomList(List<RoomInfo> roomList)
        {
            foreach (var roomInfo in roomList)
            {
                Debug.Log(roomInfo.Name);
                if (roomInfo.RemovedFromList)
                {
                    Destroy(lobbyPanelContent.transform.Find(roomInfo.Name).gameObject);
                }
                else if (lobbyPanelContent.transform.Find(roomInfo.Name) != null)
                {
                }
                else
                {
                    _roomList.Add(roomInfo);
                    var lineRoom = Instantiate(roomLine, lobbyPanelContent);
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

        /// <summary>
        ///     创建房间
        /// </summary>
        public void CreateRoom()
        {
            if (!PhotonNetwork.IsConnected || PhotonNetwork.InRoom)
                return;
            PhotonNetwork.LocalPlayer.NickName = playerName.text;
            var now = DateTime.Now.ToString("T");
            PhotonNetwork.CreateRoom("[" + PhotonNetwork.LocalPlayer.NickName + "]" + now,
                                     new RoomOptions
                                     {MaxPlayers = 2, PublishUserId = true, BroadcastPropsChangeToAll = true},
                                     TypedLobby.Default);

            var lineRoom = Instantiate(roomLine, lobbyPanelContent);

            if (lineRoom != null)
            {
                lineRoom.name = "[" + PhotonNetwork.LocalPlayer.NickName + "]" + now;
                var roomText = lineRoom.transform.Find("Text").GetComponent<Text>();
                roomText.text = "[" + PhotonNetwork.LocalPlayer.NickName + "]" + now;
                // roomText.color = new Color(1f, 0.34f, 0.44f);
            }

            Debug.Log("CREATE!");
        }

        /// <summary>
        ///     创建房间
        /// </summary>
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("ROOM CREATED!");
        }

        /// <summary>
        ///     创建房间失败
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log("FAILED TO CREATE ROOM!");
        }

        #endregion

        #region 房间

        /// <summary>
        ///     加入房间
        /// </summary>
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

        /// <summary>
        ///     其他玩家进入房间
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            _playerBNameText.text = newPlayer.NickName;
        }

        /// <summary>
        ///     其他玩家离开房间
        /// </summary>
        /// <param name="otherPlayer"></param>
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            _playerBNameText.text = "待加入";
        }

        /// <summary>
        ///     展示玩家信息
        /// </summary>
        public void ShowPlayerInfo()
        {
            var playersArray = PhotonNetwork.CurrentRoom.Players.Values.ToArray();
            _playerANameText.text = playersArray[0].NickName;
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
                _playerBNameText.text = playersArray[1].NickName;
        }

        /// <summary>
        ///     离开房间
        /// </summary>
        public void QuitRoom()
        {
            PhotonNetwork.LeaveRoom();
            lobbyUI.SetActive(true);
            roomUI.SetActive(false);
        }

        #endregion
    }
}