using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAI : Player
{
    private int _topEnemy = 0;
    private int _midEnemy = 0;
    private int _botEnemy = 0;

    private int _topSend = 0;
    private int _midSend = 0;
    private int _botSend = 0;

    public Unit[] aiAvailableUnits;

    private LayerMask _enemyLayer;
    private Collider[] _enemiesCol = new Collider[100];
    
    public float aiRestTime = 5f;


    public void Awake()
    {
        _enemyLayer =
            LayerMask.NameToLayer(LayerMask.LayerToName(this.gameObject.layer) == "ASide" ? "BSide" : "ASide");
    }

    protected void Update()
    {
        if (!_isColdDown)
            AIGaming();
    }

    protected virtual void AIGaming()
    {
        CountEnemy();
        DispatchUnits();
        AIDispatchFarmers();
        _isColdDown = true;
        Invoke("RestEnd", aiRestTime);
    }

    protected void CountEnemy()
    {
        _topEnemy = GameObject.FindGameObjectsWithTag("Top").Count(gb => (_enemyLayer.value & (1 << gb.layer)) > 0);
        _midEnemy = GameObject.FindGameObjectsWithTag("Mid").Count(gb => (_enemyLayer.value & (1 << gb.layer)) > 0);
        _botEnemy = GameObject.FindGameObjectsWithTag("Bot").Count(gb => (_enemyLayer.value & (1 << gb.layer)) > 0);
    }

    protected bool _isColdDown = false;

    protected void DispatchUnits()
    {
        Road sendRoad = FindBiggestDisadvantageRoad();

        int[] sendArray = new int[aiAvailableUnits.Length];
        // int maxSendIndex = 0;
        for (int i = 0; i < sendArray.Length; i++)
        {
            sendArray[i] = GetUnitMaxSend(aiAvailableUnits[i]);
        }

        Unit sendUnit;
        float probability = Random.Range(0f, 1f);
        if (probability < 0.5f)
        {
            sendUnit = aiAvailableUnits[Array.IndexOf(sendArray, sendArray.Max())];
        }
        else
        {
            sendUnit = aiAvailableUnits[(int) Random.Range(0, aiAvailableUnits.Length)];
        }



        //消耗木材、食物资源计算
        int foodAndWoodSend = (this.Food / sendUnit.costFood) > (this.Wood / sendUnit.costWood)
            ? (this.Wood / sendUnit.costWood)
            : (this.Food / sendUnit.costFood);
        //派遣数量随机
        int foodAndWoodSendNumber = Random.Range((int) Mathf.Ceil(foodAndWoodSend * 0.5f),foodAndWoodSend);

        //消耗金矿资源计算
        int goldSend = this.Gold / sendUnit.costGold;
        //派遣数量随机
        int goldSendNumber = Random.Range((int) Mathf.Ceil(goldSend * 0.5f),goldSend);


        int costFood = foodAndWoodSendNumber * sendUnit.costFood;
        int costWood = foodAndWoodSendNumber * sendUnit.costWood;
        int costGold = goldSendNumber * sendUnit.costGold;
        


        this.ChangeResource(Resource.Food, -costFood);
        this.ChangeResource(Resource.Wood, -costWood);
        this.ChangeResource(Resource.Gold, -costGold);

        this.SetUnits(this.RoadToTransform(sendRoad).position, Array.IndexOf(aiAvailableUnits,sendUnit), sendRoad, goldSendNumber + foodAndWoodSendNumber);
    }

    private void RestEnd()
    {
        _isColdDown = false;
    }

    private void AIDispatchFarmers()
    {
        if (this.DispatchableFarmer > 0)
        {
            Resource leastFarmerResource = (Resource) Array.IndexOf(RoadFarmers, RoadFarmers.Min());
            this.AddFarmer(leastFarmerResource);
        }
    }

    //寻找敌我双方人数差值最大的一条路
    private Road FindBiggestDisadvantageRoad()
    {
        float power = Random.Range(0f, 1f);
        if (power < 0.5f)
        {
            if (power < 0.15f) return Road.Top;
            if (power > 0.35f) return Road.Bot;
            return Road.Mid;
        }

        int[] tmp = {_topEnemy - _topSend, _midEnemy - _midSend, _botEnemy - _botSend};
        if ((_midEnemy - _midSend) == tmp.Max())
            return Road.Mid;
        if ((_topEnemy - _topSend) == tmp.Max())
            return Road.Top;

        return Road.Bot;
    }

    private Transform RoadToTransform(Road road)
    {
        switch (road)
        {
            case Road.Top:
                return this._top;
            case Road.Mid:
                return this._mid;
            case Road.Bot:
                return this._bot;
            default:
                throw new ArgumentOutOfRangeException(nameof(road), road, null);
        }
    }

    private int GetUnitMaxSend(Unit unit)
    {
        int maxFoodAndWoodCost = (this.Food / unit.costFood) > (this.Wood / unit.costWood)
            ? (this.Wood / unit.costWood)
            : (this.Food / unit.costFood);
        int maxGoldCost = this.Gold / unit.costGold;

        return maxFoodAndWoodCost + maxGoldCost;
    }
}