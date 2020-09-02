using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Units;
using UnityEngine;

public class PlayerOnline_ASide : Player
{
    public NetworkPlayerManager networkPlayerManager;
    private ExitGames.Client.Photon.Hashtable _customProperties = new ExitGames.Client.Photon.Hashtable();

    private void Awake()
    {
        UpdateLocalPlayerProperties();
    }

    public override void UpdateLocalPlayerProperties()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // _customProperties["HP"] = this.HP;

            _customProperties["Wood"] = this.Wood;
            _customProperties["Gold"] = this.Gold;
            _customProperties["Food"] = this.Food;

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

    public override void AddFarmer(Resource rs)
    {
        if (FarmerCount > RoadFarmers.Sum())
        {
            RoadFarmers[(int) rs]++;
            this.networkPlayerManager.SendAddFarmer(rs);
        }
        
    }


    public override void SubtractFarmer(Resource rs)
    {
        if (RoadFarmers[(int) rs] > 0)
        {
            RoadFarmers[(int) rs]--;
            this.networkPlayerManager.SendAddFarmer(rs);
        }
    }

    public override void InstantiateUnit(int chosenUnit, Road rd, Vector3 oriPoint)
    {
        base.InstantiateUnit(chosenUnit, rd, oriPoint);
        this.networkPlayerManager.SendInstantiateUnit(chosenUnit, rd, oriPoint);
    }

    #endregion
}
