﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Units;
using UnityEngine.Events;
using Object = System.Object;

public class Farmer : Unit
{
    public int ResouceCarried { get; protected internal set; }

    protected Transform _targerResource;
    protected Transform _home;
    
    private Resource _resouceType;


    public float workTime = 5f;
    public int[] maxLoad = new []{10,5,10};
    

    public override void Start()
    {

        GenerateWorkPath();
        this.navStopEventHandler += OnNavStop;
        
        this.farmerBackEventHandler += Drop;
        this.farmerBackEventHandler += this.sidePlayer.FarmerBack;
        
        //死亡处理
        this.UnitDeathEventHandler += () =>  
        {
            this.sidePlayer.FarmerCount--;
            this.sidePlayer.RoadFarmers[(int) this.road]--;
            this.sidePlayer.RoadWorkingFarmers[(int) this.road]--;
        };
        
        
        if(this.ResouceCarried > 0)
        {
            InitTarget = _home;
        }else {
            this.ResouceCarried = 0;
            InitTarget = _targerResource;
        }
        
        
        base.Start();

    }
    
    //到达目的地
    protected void OnNavStop(GameObject gm,Transform tr)
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

    protected void GenerateWorkPath()
    {
        this._home = this.sidePlayer.gameObject.transform.Find("Positions").Find(this.road.ToString() + "Ori").transform;
        // 注意资源排布要按照上中下路的顺序
        LayerMask resourceLayerMask = (1 << LayerMask.NameToLayer(LayerMask.LayerToName(this.gameObject.layer)[0] + "Side")) | (1 << LayerMask.NameToLayer("Neutrality"));
       // GameObject[] sideResource = GameObject.FindGameObjectsWithTag("Resouce").Where(gb => (resourceLayerMask.value & 1 << gb.layer) > 0).ToArray();
       Transform[] sideResource = this.sidePlayer.resourceTransform;
        // Debug.Log(sideResource[0]);
        // Debug.Log(sideResource[1]);
        // Debug.Log(sideResource[2]);
        this._targerResource = sideResource[(int)this.road];
        switch (road)
        {
            case Road.Top:
                //this._targerResource = sideResource[0].transform;
                this._resouceType = Resource.Food;
                break;
            case Road.Mid:
                //this._targerResource = sideResource[1].transform;
                this._resouceType = Resource.Gold;
                break;
            case Road.Bot:
                //this._targerResource = sideResource[2].transform;
                this._resouceType = Resource.Wood;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    
    //返回事件
    public UnityAction<Farmer, Transform> farmerBackEventHandler;
    
    // 工作
    protected virtual void Work()
    {
        this.ResouceCarried += maxLoad[(int)road];
        this.Goto(_home);
        Debug.Log("GO HOME");
    }
    
    // 放下资源
    private void FarmerBack()
    {
        farmerBackEventHandler(this,this.transform);
    }

    protected void Drop(Farmer farmer,Transform tr)
    {
        this.sidePlayer.ChangeResource(_resouceType,this.ResouceCarried);
        this.ResouceCarried = 0;
        this.Goto(_targerResource);
        Debug.Log("GO Resource");
        //查看是否人数已满
        
    }
}
