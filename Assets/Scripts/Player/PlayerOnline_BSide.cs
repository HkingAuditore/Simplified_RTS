using System;
using Network;
using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class PlayerOnline_BSide : Player
    {
        /// <summary>
        ///     网络玩家管理
        /// </summary>
        public NetworkPlayerManager networkPlayerManager;

        private Type _playerType;

        /// <summary>
        ///     玩家用户名
        /// </summary>
        public int PlayerID { get; set; }

        public Photon.Realtime.Player PlayerPhoton { get; set; }

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

        /// <summary>
        ///     更新远程玩家信息
        /// </summary>
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

        /// <summary>
        ///     更新远程玩家
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void UpdateSingleRemotePlayerProperty(string propertyName, object value)
        {
            _playerType.GetProperty(propertyName)?.SetValue(this, value);
        }


        #region 控制

        #endregion
    }
}