using ExitGames.Client.Photon;
using Photon.Pun;
using Player;
using Units;
using UnityEngine;

namespace Network
{
    /// <summary>
    ///     网络玩家管理
    /// </summary>
    public class NetworkPlayerManager : MonoBehaviourPunCallbacks
    {
        /// <summary>
        ///     线上玩家A
        /// </summary>
        public PlayerOnline_ASide playerASideOnline;

        /// <summary>
        ///     线上玩家B
        /// </summary>
        public PlayerOnline_BSide playerBSideOnline;

        /// <summary>
        ///     玩家数值更新
        /// </summary>
        /// <param name="targetPlayer">玩家</param>
        /// <param name="changedProps">改变数值组</param>
        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            Debug.Log("Properties Changed!");

            if (targetPlayer != null && Equals(targetPlayer, playerBSideOnline.PlayerPhoton))
                // 需要优化
                playerBSideOnline.UpdateRemotePlayerProperties();
        }

        #region 操作命令传输

        #region 农民调遣

        /// <summary>
        ///     A发送过来，传输给另一个端的B
        ///     添加农民
        /// </summary>
        /// <param name="rs"></param>
        public void SendAddFarmer(GameResourceType rs)
        {
            // Debug.Log("Call PRC!");
            photonView.RPC("RPC_AddFarmer", RpcTarget.Others, rs);
        }

        [PunRPC]
        public void RPC_AddFarmer(GameResourceType rs)
        {
            // Debug.Log("On PRC!");
            playerBSideOnline.AddFarmer(rs);
        }

        public void SendSubtractFarmer(GameResourceType rs)
        {
            photonView.RPC("RPC_SubtractFarmer", RpcTarget.Others, rs);
        }

        [PunRPC]
        public void RPC_SubtractFarmer(GameResourceType rs)
        {
            playerBSideOnline.SubtractFarmer(rs);
        }

        #endregion

        #region 单位调遣

        /// <summary>
        ///     派遣单位
        /// </summary>
        /// <param name="chosenUnit"></param>
        /// <param name="rd"></param>
        /// <param name="oriPoint"></param>
        public void SendInstantiateUnit(int chosenUnit, Road rd, Vector3 oriPoint)
        {
            oriPoint.x *= -1;
            photonView.RPC("RPC_SendInstantiateUnit", RpcTarget.Others, chosenUnit, rd, oriPoint);
        }

        [PunRPC]
        public void RPC_SendInstantiateUnit(int chosenUnit, Road rd, Vector3 oriPoint)
        {
            playerBSideOnline.InstantiateUnit(chosenUnit, rd, oriPoint);
        }

        #endregion

        #endregion
    }
}