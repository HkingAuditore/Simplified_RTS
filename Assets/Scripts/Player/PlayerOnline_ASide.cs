using System.Linq;
using ExitGames.Client.Photon;
using Network;
using Photon.Pun;
using Player;
using Units;
using UnityEngine;

public class PlayerOnline_ASide : Player.Player
{
    public           NetworkPlayerManager networkPlayerManager;
    private readonly Hashtable            _customProperties = new Hashtable();

    private void Awake()
    {
        UpdateLocalPlayerProperties();
    }

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

    public override void AddFarmer(GameResourceType rs)
    {
        if (FarmerCount > RoadFarmers.Sum())
        {
            RoadFarmers[(int) rs]++;
            networkPlayerManager.SendAddFarmer(rs);
        }
    }


    public override void SubtractFarmer(GameResourceType rs)
    {
        if (RoadFarmers[(int) rs] > 0)
        {
            RoadFarmers[(int) rs]--;
            networkPlayerManager.SendAddFarmer(rs);
        }
    }

    public override void InstantiateUnit(int chosenUnit, Road rd, Vector3 oriPoint)
    {
        base.InstantiateUnit(chosenUnit, rd, oriPoint);
        networkPlayerManager.SendInstantiateUnit(chosenUnit, rd, oriPoint);
    }

    #endregion
}