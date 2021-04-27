using System;
using Network;
using Photon.Pun;
using UnityEngine;

public class PlayerOnline_BSide : Player.Player
{
    public  NetworkPlayerManager   networkPlayerManager;
    private Type                   _playerType;
    public  int                    PlayerID     { get; set; }
    public  Photon.Realtime.Player PlayerPhoton { get; set; }

    private void Awake()
    {
        _playerType = GetType();
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
                if (!player.Value.IsLocal)
                {
                    PlayerID     = player.Key;
                    PlayerPhoton = player.Value;
                    break;
                }
    }

    protected override void Start()
    {
        base.Start();
        UpdateRemotePlayerProperties();
    }

    public void UpdateRemotePlayerProperties()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            Debug.Log("Receive Properties!");

            // 同步数据
            // if (PlayerPhoton.CustomProperties.ContainsKey("HP"))
            //     this.HP = (int) PlayerPhoton.CustomProperties["HP"];

            if (PlayerPhoton.CustomProperties.ContainsKey("Wood"))
                Wood = (int) PlayerPhoton.CustomProperties["Wood"];
            if (PlayerPhoton.CustomProperties.ContainsKey("Gold"))
                Gold = (int) PlayerPhoton.CustomProperties["Gold"];
            if (PlayerPhoton.CustomProperties.ContainsKey("Food"))
                Food = (int) PlayerPhoton.CustomProperties["Food"];

            // if (PlayerPhoton.CustomProperties.ContainsKey("FarmerCount"))
            //     this.FarmerCount = (int) PlayerPhoton.CustomProperties["FarmerCount"];
            // if (PlayerPhoton.CustomProperties.ContainsKey("MaxFarmerNumber"))
            //     this.MaxFarmerNumber = (int) PlayerPhoton.CustomProperties["MaxFarmerNumber"];
            // if (PlayerPhoton.CustomProperties.ContainsKey("RoadFarmers"))
            //     this.RoadFarmers = (int[]) PlayerPhoton.CustomProperties["RoadFarmers"];
            // if (PlayerPhoton.CustomProperties.ContainsKey("RoadWorkingFarmers"))
            //     this.RoadWorkingFarmers = (int[]) PlayerPhoton.CustomProperties["RoadWorkingFarmers"];
            // if (PlayerPhoton.CustomProperties.ContainsKey("DispatchableFarmer"))
            //     this.DispatchableFarmer = (int) PlayerPhoton.CustomProperties["DispatchableFarmer"];
        }
    }

    public void UpdateSingleRemotePlayerProperty(string propertyName, object value)
    {
        _playerType.GetProperty(propertyName)?.SetValue(this, value);
    }


    #region 控制

    #endregion
}