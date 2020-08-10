using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;
using UnityEngine.Events;

public enum Resource
{
    Food,Gold,Wood
}

public class Player : MonoBehaviour
{
    
    private void Start()
    {
        this._top = this.gameObject.transform.Find("Positions").transform.Find("TopOri").transform;
        this._mid = this.gameObject.transform.Find("Positions").transform.Find("MidOri").transform;
        this._bot = this.gameObject.transform.Find("Positions").transform.Find("BotOri").transform;

        this.Food = 10;
        this.Wood = 10;
        this.Gold = 0;
        this.HP = 100;
    }

    public UnityAction onUpdate;
    private void Update()
    {
        onUpdate?.Invoke();
    }


    /*****资源**********/
    public int Food { get; private set; }
    public int Wood { get; private set; }
    public int Gold { get; private set; }
    public int HP { get; private set; }
    public void ChangeResource(Resource resource, int count)
    {
        switch (resource)
        {
            case Resource.Food:
                this.Food += count;
                break;
            case Resource.Wood:
                this.Wood += count;
                break;
            case Resource.Gold:
                this.Gold += count;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resource), resource, null);
        }
    }
    
    /**********农民派遣*********/
    //农民
    public Farmer farmerPrefab;
    private int _farmerCount = 3;
    private int _dispatchableFarmer;

    private int[] _roadFarmers = new int[3];
    private int[] _roadWorkingFarmers = new int[3];
    
    private Transform _top;
    private Transform _mid;
    private Transform _bot;

    //增加一条路的农民分配
    public void AddFarmer(Resource rs)
    {
        if (_farmerCount > _roadFarmers.Sum())
        {
            _roadFarmers[(int) rs]++;
            if (onUpdate == null || Array.IndexOf(onUpdate.GetInvocationList(), (UnityAction)DispatchFarmer) == -1)
            {
                onUpdate += DispatchFarmer;
            }

        }
    }
    //减少一条路的农民分配
    public void SubtractFarmer(Resource rs)
    {
        if(_roadFarmers[(int) rs] > 0)
            _roadFarmers[(int) rs]--;
    }


    // 生成农民
    private void InstantiateFarmer(Road rd)
    {
        _roadWorkingFarmers[(int) rd]++;
        Transform bornPoint;
        switch (rd)
        {
            case Road.Top:
                bornPoint = _top;
               break;
            case Road.Mid:
                bornPoint = _mid;
                break;
            case Road.Bot:
                bornPoint = _bot;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(rd), rd, null);
        }
        farmerPrefab.road = rd;
        farmerPrefab.gameObject.layer = this.gameObject.layer;
        farmerPrefab.sidePlayer = this;
        var farmerInstantiated = (Farmer) Instantiate<Farmer>(farmerPrefab, bornPoint.position, farmerPrefab.transform.rotation, this.gameObject.transform.Find(this.gameObject.name[0] + "Units").transform);
    }

    // 当农民返回
    public void FarmerBack(Farmer farmer,Transform tr)
    {
        if (_roadWorkingFarmers[(int) farmer.road] > _roadFarmers[(int) farmer.road])
        {
            Destroy(farmer.gameObject);
            _roadWorkingFarmers[(int) farmer.road]--;
            _dispatchableFarmer++;
        }
    }
    
    //空闲农民调遣
    private void DispatchFarmer()
    {
        for (int i = 0; i < 3; i++)
        {
            if (_roadFarmers[i] > _roadWorkingFarmers[i])
            {
                InstantiateFarmer((Road)i);
                _dispatchableFarmer--;
            }
        }

        if (_dispatchableFarmer == 0)
        {
            onUpdate -= DispatchFarmer;
        }
        
    }
    
}
