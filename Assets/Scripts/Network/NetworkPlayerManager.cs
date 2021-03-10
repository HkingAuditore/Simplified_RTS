using ExitGames.Client.Photon;
using Photon.Pun;
using Units;
using UnityEngine;

public class NetworkPlayerManager : MonoBehaviourPunCallbacks
{
    public PlayerOnline_ASide playerASideOnline;
    public PlayerOnline_BSide playerBSideOnline;

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

    //A发送过来，传输给另一个端的B
    public void SendAddFarmer(Resource rs)
    {
        // Debug.Log("Call PRC!");
        photonView.RPC("RPC_AddFarmer", RpcTarget.Others, rs);
    }

    [PunRPC]
    public void RPC_AddFarmer(Resource rs)
    {
        // Debug.Log("On PRC!");
        playerBSideOnline.AddFarmer(rs);
    }

    public void SendSubtractFarmer(Resource rs)
    {
        photonView.RPC("RPC_SubtractFarmer", RpcTarget.Others, rs);
    }

    [PunRPC]
    public void RPC_SubtractFarmer(Resource rs)
    {
        playerBSideOnline.SubtractFarmer(rs);
    }

    #endregion

    #region 单位调遣

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