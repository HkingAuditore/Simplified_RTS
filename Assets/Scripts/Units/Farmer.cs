using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
        this.onNavStop += OnNavStop;
        
        ResouceCarried = 0;
        InitTarget = _targerResource;
    }
    
    //到达目的地
    private void OnNavStop(GameObject gm,Transform tr)
    {
        if (Vector3.Distance(tr.position,_targerResource.position) < 0.3f)
        {
            Invoke("Work", workTime);
        }
        else
        {
            Invoke("Drop", workTime);
        }
    }

    private void GenerateWorkPath()
    {
        this._home = this.sidePlayer.gameObject.transform.Find("Positions").Find(this.road.ToString() + "Ori").transform;
        // 注意资源排布要按照上中下路的顺序
        // Debug.Log(Road.Top.ToString() +":"+ GameObject.FindGameObjectsWithTag("Resouce")[(int)Road.Top].name);
        // Debug.Log(Road.Mid.ToString() +":"+ GameObject.FindGameObjectsWithTag("Resouce")[(int)Road.Mid].name);
        // Debug.Log(Road.Bot.ToString() +":"+ GameObject.FindGameObjectsWithTag("Resouce")[(int)Road.Bot].name);
        this._targerResource = GameObject.FindGameObjectsWithTag("Resouce")[(int)this.road + 1].transform;
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
    
    
    // 工作
    private void Work()
    {
        this.ResouceCarried += maxLoad;
        this.Goto(_home);
        Debug.Log("GO HOME");
    }
    
    // 放下资源
    private void Drop()
    {
        this.sidePlayer.ChangeResource(_resouceType,this.ResouceCarried);
        this.ResouceCarried = 0;
        this.Goto(_targerResource);
        Debug.Log("GO Resource");
    }
}
