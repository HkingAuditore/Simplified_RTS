using System;
using System.Linq;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAI : MonoBehaviour
{
    private Unit[] aiAvailableUnits;
    public  Player aiPlayer;
    [Header("AI相关")] [Space(15)]
    public bool[]     enableRoads = new bool[3];
    public                              float      aiRestTime  = 5f;
    private                             int        _botEnemy;
    private readonly                    int        _botSend    = 0;

    private LayerMask _enemyLayer;

    protected        bool _isColdDown;
    private          int  _midEnemy;
    private readonly int  _midSend = 0;
    private          int  _topEnemy;

    private readonly int _topSend = 0;


    public void Awake()
    {
        _enemyLayer =
            LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer) == "ASide" ? "BSide" : "ASide");
        aiAvailableUnits = (from availableUnit in aiPlayer.availableUnits
                            select availableUnit.GetComponent<Unit>()).ToArray();
    }

    protected void Update()
    {
        if (!_isColdDown)
            AIGaming();
    }

    protected virtual void AIGaming()
    {
        CountEnemy();
        DispatchUnits(1);
        if (aiPlayer.enableFarmer)
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

    protected void DispatchUnits()
    {
        //TODO mid
        var sendRoad = Road.Mid;

        var sendArray = new int[aiAvailableUnits.Length];
        // int maxSendIndex = 0;
        for (var i = 0; i < sendArray.Length; i++) sendArray[i] = GetUnitMaxSend(aiAvailableUnits[i]);

        Unit sendUnit;
        var  probability = Random.Range(0f, 1f);
        if (probability < 0.5f)
            sendUnit = aiAvailableUnits[Array.IndexOf(sendArray, sendArray.Max())];
        else
            sendUnit = aiAvailableUnits[Random.Range(0, aiAvailableUnits.Length)];


        //消耗木材、食物资源计算
        var foodAndWoodSend = aiPlayer.Food / sendUnit.costFood > aiPlayer.Wood / sendUnit.costWood
                                  ? aiPlayer.Wood / sendUnit.costWood
                                  : aiPlayer.Food / sendUnit.costFood;
        //派遣数量随机
        var foodAndWoodSendNumber = Random.Range((int) Mathf.Ceil(foodAndWoodSend * 0.5f), foodAndWoodSend);

        //消耗金矿资源计算
        var goldSend = aiPlayer.Gold / sendUnit.costGold;
        //派遣数量随机
        var goldSendNumber = Random.Range((int) Mathf.Ceil(goldSend * 0.5f), goldSend);


        var costFood = foodAndWoodSendNumber * sendUnit.costFood;
        var costWood = foodAndWoodSendNumber * sendUnit.costWood;
        var costGold = goldSendNumber        * sendUnit.costGold;


        aiPlayer.ChangeResource(GameResourceType.Food, -costFood);
        aiPlayer.ChangeResource(GameResourceType.Wood, -costWood);
        aiPlayer.ChangeResource(GameResourceType.Gold, -costGold);

        aiPlayer.SetUnits(RoadToTransform(sendRoad).position, Array.IndexOf(aiAvailableUnits, sendUnit), sendRoad,
                         goldSendNumber + foodAndWoodSendNumber);
    }
    protected void DispatchUnits(int num)
    {
        //TODO mid
        var sendRoad = Road.Mid;

        var sendArray = new int[aiAvailableUnits.Length];
        // int maxSendIndex = 0;
        for (var i = 0; i < sendArray.Length; i++) sendArray[i] = GetUnitMaxSend(aiAvailableUnits[i]);

        Unit sendUnit;
        var  probability = Random.Range(0f, 1f);
        if (probability < 0.5f)
            sendUnit = aiAvailableUnits[Array.IndexOf(sendArray, sendArray.Max())];
        else
            sendUnit = aiAvailableUnits[Random.Range(0, aiAvailableUnits.Length)];


        //消耗木材、食物资源计算
        var foodAndWoodSend = aiPlayer.Food / sendUnit.costFood > aiPlayer.Wood / sendUnit.costWood
                                  ? aiPlayer.Wood / sendUnit.costWood
                                  : aiPlayer.Food / sendUnit.costFood;
        //派遣数量随机
        var foodAndWoodSendNumber = Random.Range((int) Mathf.Ceil(foodAndWoodSend * 0.5f), foodAndWoodSend);
        foodAndWoodSendNumber = foodAndWoodSendNumber > num ? num : foodAndWoodSend;

        //消耗金矿资源计算
        var goldSend = aiPlayer.Gold / sendUnit.costGold;
        //派遣数量随机
        var goldSendNumber = Random.Range((int) Mathf.Ceil(goldSend * 0.5f), goldSend);
        goldSendNumber = goldSendNumber > num ? num : goldSendNumber;


        var costFood = foodAndWoodSendNumber * sendUnit.costFood;
        var costWood = foodAndWoodSendNumber * sendUnit.costWood;
        var costGold = goldSendNumber        * sendUnit.costGold;


        aiPlayer.ChangeResource(GameResourceType.Food, -costFood);
        aiPlayer.ChangeResource(GameResourceType.Wood, -costWood);
        aiPlayer.ChangeResource(GameResourceType.Gold, -costGold);

        aiPlayer.SetUnits(RoadToTransform(sendRoad).position, Array.IndexOf(aiAvailableUnits, sendUnit), sendRoad,
                         goldSendNumber + foodAndWoodSendNumber);
    }

    private void RestEnd()
    {
        _isColdDown = false;
    }

    private void AIDispatchFarmers()
    {
        if (aiPlayer.DispatchableFarmer > 0)
        {
            var leastFarmerResource = (GameResourceType) Array.IndexOf(aiPlayer.RoadFarmers, aiPlayer.RoadFarmers.Min());
            aiPlayer.AddFarmer(leastFarmerResource);
        }
    }

    //寻找敌我双方人数差值最大的一条路
    private Road FindBiggestDisadvantageRoad()
    {
        var power = Random.Range(0f, 1f);
        if (power < 0.5f)
        {
            if (power < 0.15f) return Road.Top;
            if (power > 0.35f) return Road.Bot;
            return Road.Mid;
        }

        int[] tmp = {_topEnemy - _topSend, _midEnemy - _midSend, _botEnemy - _botSend};
        if (_midEnemy - _midSend == tmp.Max())
            return Road.Mid;
        if (_topEnemy - _topSend == tmp.Max())
            return Road.Top;

        return Road.Bot;
    }

    private Transform RoadToTransform(Road road)
    {
        switch (road)
        {
            case Road.Top:
                return aiPlayer.Top;
            case Road.Mid:
                return aiPlayer.Mid;
            case Road.Bot:
                return aiPlayer.Bot;
            default:
                throw new ArgumentOutOfRangeException(nameof(road), road, null);
        }
    }

    private int GetUnitMaxSend(Unit unit)
    {
        var maxFoodAndWoodCost = aiPlayer.Food / unit.costFood > aiPlayer.Wood / unit.costWood
                                     ? aiPlayer.Wood / unit.costWood
                                     : aiPlayer.Food / unit.costFood;
        var maxGoldCost = aiPlayer.Gold / unit.costGold;

        return maxFoodAndWoodCost + maxGoldCost;
    }
}