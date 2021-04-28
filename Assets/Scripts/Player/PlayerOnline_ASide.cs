using System.Linq;
using ExitGames.Client.Photon;
using Network;
using Photon.Pun;
using Units;
using UnityEngine;

namespace Player
{
    public class PlayerOnline_ASide : Player
    {
        /// <summary>
        ///     网络玩家管理
        /// </summary>
        public NetworkPlayerManager networkPlayerManager;

        private readonly Hashtable _customProperties = new Hashtable();

        private void Awake()
        {
            UpdateLocalPlayerProperties();
        }

        /// <summary>
        ///     更新本地玩家属性
        /// </summary>
        public override void UpdateLocalPlayerProperties()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                // _customProperties["HP"] = this.HP;

                _customProperties["Wood"] = Wood;
                _customProperties["Gold"] = Gold;
                _customProperties["Food"] = Food;

                //  _customProperties["FarmerCount"] = this.FarmerCount;
                //  _customProperties["MaxFarmerNumber"] = this.MaxFarmerNumber;
                //  _customProperties["RoadFarmers"] = this.RoadFarmers;
                // _customProperties["RoadWorkingFarmers"] = this.RoadWorkingFarmers;
                //  _customProperties["DispatchableFarmer"] = this.DispatchableFarmer;

                PhotonNetwork.SetPlayerCustomProperties(_customProperties);
                Debug.Log("Update Properties!");
            }
        }

        #region 控制信息传输

        /// <summary>
        ///     增加农民
        /// </summary>
        /// <param name="rs"></param>
        public override void AddFarmer(GameResourceType rs)
        {
            if (FarmerCount > RoadFarmers.Sum())
            {
                RoadFarmers[(int) rs]++;
                networkPlayerManager.SendAddFarmer(rs);
            }
        }

        /// <summary>
        ///     减少农民
        /// </summary>
        /// <param name="rs"></param>
        public override void SubtractFarmer(GameResourceType rs)
        {
            if (RoadFarmers[(int) rs] > 0)
            {
                RoadFarmers[(int) rs]--;
                networkPlayerManager.SendAddFarmer(rs);
            }
        }

        /// <summary>
        ///     生成单位
        /// </summary>
        /// <param name="chosenUnit"></param>
        /// <param name="rd"></param>
        /// <param name="oriPoint"></param>
        public override void InstantiateUnit(int chosenUnit, Road rd, Vector3 oriPoint)
        {
            base.InstantiateUnit(chosenUnit, rd, oriPoint);
            networkPlayerManager.SendInstantiateUnit(chosenUnit, rd, oriPoint);
        }

        #endregion
    }
}