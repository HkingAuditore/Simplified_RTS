using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Units;
using UnityEngine.Events;
using Object = System.Object;

public class Farmer : Unit
{
    public int ResouceCarried { get; private set; }

    private Transform _targerResource;
    private Transform _home;
    
    private Resource _resouceType;


    public float workTime = 5f;
    public int maxLoad = 10;
    

    public void Awake()
    {
        GenerateWorkPath();
        this.navStopEventHandler += OnNavStop;
        
        this.farmerBackEventHandler += Drop;
        this.farmerBackEventHandler += this.sidePlayer.FarmerBack;
        
        //死亡处理
        this.unitDeathEventHandler += () =>  
        {
            this.sidePlayer.FarmerCount--;
            this.sidePlayer.RoadFarmers[(int) this.road]--;
            this.sidePlayer.RoadWorkingFarmers[(int) this.road]--;
        };
        
        this.ResouceCarried = 0;
        InitTarget = _targerResource;
    }
    
    //到达目的地
    private void OnNavStop(GameObject gm,Transform tr)
    {
        // Debug.Log("To Resource:" + Vector3.Distance(tr.position,_targerResource.position));
        // Debug.Log("To Home:" + Vector3.Distance(tr.position,_home.position));
        if (Vector3.Distance(tr.position,_targerResource.position) < 0.6f)
        {
            Invoke("Work", workTime);
        }
        else if(Vector3.Distance(tr.position,_home.position) < 0.6f)
        {
            Invoke("FarmerBack", workTime);
        }
    }

    private void GenerateWorkPath()
    {
        this._home = this.sidePlayer.gameObject.transform.Find("Positions").Find(this.road.ToString() + "Ori").transform;
        // 注意资源排布要按照上中下路的顺序
        LayerMask resourceLayerMask = (1 << LayerMask.NameToLayer(LayerMask.LayerToName(this.gameObject.layer)[0] + "Side")) | (1 << LayerMask.NameToLayer("Neutrality"));
        GameObject[] sideResource = GameObject.FindGameObjectsWithTag("Resouce").Where(gb => (resourceLayerMask.value & 1 << gb.layer) > 0).ToArray();
        // Debug.Log(sideResource[0]);
        // Debug.Log(sideResource[1]);
        // Debug.Log(sideResource[2]);
        this._targerResource = sideResource[(int)this.road].transform;
        switch (road)
        {
            case Road.Top:
                this._resouceType = Resource.Food;
                break;
            case Road.Mid:
                this._resouceType = Resource.Gold;
                break;
            case Road.Bot:
                this._resouceType = Resource.Wood;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    
    //返回事件
    public UnityAction<Farmer, Transform> farmerBackEventHandler;
    
    // 工作
    private void Work()
    {
        this.ResouceCarried += maxLoad;
        this.Goto(_home);
        Debug.Log("GO HOME");
    }
    
    // 放下资源
    private void FarmerBack()
    {
        farmerBackEventHandler(this,this.transform);
    }
    
    private void Drop(Farmer farmer,Transform tr)
    {
        this.sidePlayer.ChangeResource(_resouceType,this.ResouceCarried);
        this.ResouceCarried = 0;
        this.Goto(_targerResource);
        Debug.Log("GO Resource");
        //查看是否人数已满
        
    }
}
