using System;
using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum GameResourceType
{
    Food,
    Gold,
    Wood
}

public class GameException : ApplicationException
{
    public GameResourceType GameResourceType;

    public GameException(GameResourceType rs, string message = "Run Out") : base(message)
    {
        GameResourceType = rs;
    }
    public GameException(string message) : base(message)
    {
        
    }
}

public class Player : MonoBehaviour
{
    [SerializeField] private int  hp;
    private                  bool _isDead = false;
    public int HP
    {
        get => hp;
        set
        {
            // hp = value;
            if (value > 0)
            {
                hp = value;
            }
            else
            {
                Time.timeScale = 0;
                if(!_isDead)
                {
                    GameManager.GameManager.GetManager.resultUI.ShowResult(gameObject.name[0] == 'B');
                    _isDead = true;
                }
                
            }
        }
    }
    

    public UnityAction updateEventHandler;



    public virtual void Start()
    {

        Top = gameObject.transform.Find("Positions").transform.Find("TopOri").transform;
        Mid = gameObject.transform.Find("Positions").transform.Find("MidOri").transform;
        Bot = gameObject.transform.Find("Positions").transform.Find("BotOri").transform;

        Food = 0;
        Wood = 0;
        Gold = 0;

        DispatchableFarmer = FarmerCount;
    }

    private void FixedUpdate()
    {
        //生产任务
        if(enableFarmer)
        {
            RegisterProduceFarmer();
            DispatchFarmer();
        } 
        RegisterProduceResource();

        
        updateEventHandler?.Invoke();
    }


    #region 资源

    public int Food
    {
        get => _food;
        protected set
        {
            if (value >= 0)
                _food = value;
            else
                throw new GameException(GameResourceType.Food);
            //Debug.Log("Change Food");

            UpdateLocalPlayerProperties();
        }
    }

    public int Wood
    {
        get => _wood;
        protected set
        {
            if (value >= 0)
                _wood = value;
            else
                throw new GameException(GameResourceType.Wood);
            //Debug.Log("Change Wood");

            UpdateLocalPlayerProperties();
        }
    }

    public int Gold
    {
        get => _gold;
        protected set
        {
            if (value >= 0)
                _gold = value;
            else
                throw new GameException(GameResourceType.Gold);
            //Debug.Log("Change Gold");

            UpdateLocalPlayerProperties();
        }
    }

    public void ChangeResource(GameResourceType gameResourceType, int count)
    {
        switch (gameResourceType)
        {
            case GameResourceType.Food:
                Food += count;
                break;
            case GameResourceType.Wood:
                Wood += count;
                break;
            case GameResourceType.Gold:
                Gold += count;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameResourceType), gameResourceType, null);
        }
    }

    public bool IsEnoughForUnit(Unit unit)
    {
        return this.Gold >= unit.costGold && this.Wood >= unit.costWood && this.Food >= unit.costFood;
    }
    
    public bool IsEnoughForUnit(Unit unit,GameResourceType gameResourceType)
    {
        switch (gameResourceType)
        {
            case GameResourceType.Food:
                return this.Food >= unit.costFood;
                break;
            case GameResourceType.Gold:
                return  this.Gold >= unit.costGold;
                break;
            case GameResourceType.Wood:
                return this.Wood >= unit.costWood;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameResourceType), gameResourceType, null);
        }
    }

    #endregion

    #region 网络支持

    public virtual void UpdateLocalPlayerProperties()
    {
    }

    #endregion

    #region 农民相关处理


    #region 农民调遣

    #region 字段和属性

    [Header("农民")]
    public bool enableFarmer;
    public  Transform[] resourceTransform;
    public  Farmer      farmerPrefab;
    private int         _maxFarmerNumber = 5;
    private int         _farmerCount     = 1;
    private int         _dispatchableFarmer;
    private int[]       _roadFarmers        = new int[3];
    private int[]       _roadWorkingFarmers = new int[3];
    public  Transform   Top { get; set; }
    public  Transform   Mid { get; set; }
    public  Transform   Bot { get; set; }
    private int         _food;
    private int         _wood;
    private int         _gold;
    

    // 最大可用农民数量
    public int MaxFarmerNumber
    {
        get => _maxFarmerNumber;
        protected set
        {
            _maxFarmerNumber = value;
//            Debug.Log("Change MaxFarmerNumber");

            UpdateLocalPlayerProperties();
        }
    }

    //当前农民数量
    public int FarmerCount
    {
        get => _farmerCount;
        set
        {
            _farmerCount = value;
//            Debug.Log("Change FarmerCount");

            UpdateLocalPlayerProperties();
        }
    }

    //可调遣农民数量
    public int DispatchableFarmer
    {
        get => _dispatchableFarmer;
        protected set
        {
            _dispatchableFarmer = value;
//            Debug.Log("Change DispatchableFarmer");

            UpdateLocalPlayerProperties();
        }
    }

    //每条路农民数量

    public int[] RoadFarmers
    {
        get => _roadFarmers;
        protected set
        {
            _roadFarmers = value;
//            Debug.Log("Change RoadFarmers");

            UpdateLocalPlayerProperties();
        }
    }

    //每条路正在工作农民数量
    public int[] RoadWorkingFarmers
    {
        get => _roadWorkingFarmers;
        protected set
        {
            _roadWorkingFarmers = value;
//            Debug.Log("Change RoadWorkingFarmers");

            UpdateLocalPlayerProperties();
        }
    }

    #endregion
    

    //增加一条路的农民分配
    public virtual void AddFarmer(GameResourceType rs)
    {
        if (FarmerCount > RoadFarmers.Sum()) RoadFarmers[(int) rs]++;
    }

    //减少一条路的农民分配
    public virtual void SubtractFarmer(GameResourceType rs)
    {
        if (RoadFarmers[(int) rs] > 0)
            RoadFarmers[(int) rs]--;
    }


    // 生成农民GameObject
    private void InstantiateFarmer(Road rd)
    {
        RoadWorkingFarmers[(int) rd]++;
        Transform bornPoint;
        switch (rd)
        {
            case Road.Top:
                bornPoint = Top;
                break;
            case Road.Mid:
                bornPoint = Mid;
                break;
            case Road.Bot:
                bornPoint = Bot;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(rd), rd, null);
        }

        //寻路设置
        LayerMask resourceLayerMask =
            (1 << NavMesh.GetAreaFromName(LayerMask.LayerToName(gameObject.layer)[0] + "Walkable")) |
            (1 << NavMesh.GetAreaFromName("Walkable"));

        // Debug.Log(Resources.Load("Units/" + farmerPrefab.name + LayerMask.LayerToName(this.gameObject.layer)[0]));

        var farmerInstantiated = Instantiate(farmerPrefab, bornPoint.position,
                                             farmerPrefab.transform.rotation,
                                             gameObject.transform.Find(gameObject.name[0] + "Units").transform);

        //TODO 动态生成材质实例，需要优化
        farmerInstantiated.gameObject.transform.Find(farmerPrefab.name).gameObject.GetComponent<SpriteRenderer>()
                          .sharedMaterial = Instantiate(farmerPrefab
                                                       .gameObject.transform.Find(farmerPrefab.name).gameObject
                                                       .GetComponent<SpriteRenderer>().sharedMaterial);
        farmerInstantiated.gameObject.transform.Find(farmerPrefab.name).gameObject.GetComponent<SpriteRenderer>()
                          .sharedMaterial.SetTexture(
                                                     "_BaseMap",
                                                     Resources.Load("Units/" + farmerPrefab.name +
                                                                    LayerMask.LayerToName(gameObject.layer)[0]) as
                                                         Texture
                                                    );
        Debug.Log("TEX :" + "Units/" + farmerPrefab.name + LayerMask.LayerToName(gameObject.layer)[0]);

        // farmerInstantiated.GetComponent<NavMeshAgent>().areaMask = resourceLayerMask.value;
        farmerInstantiated.tag                                   = rd.ToString();
        farmerInstantiated.sidePlayer                            = this;


        farmerInstantiated.road           = rd;
        farmerInstantiated.ResouceCarried = 0;
        //Debug.Log(this.gameObject.name + " Farmer Road :" + rd);
        farmerInstantiated.gameObject.layer = gameObject.layer;
        DispatchableFarmer--;

        farmerInstantiated.enabled = true;
    }

    public void InstantiateFarmer(Road rd, Transform transform, int resourceCarried)
    {
        RoadWorkingFarmers[(int) rd]++;
        var bornPoint = transform;

        //寻路设置
        LayerMask resourceLayerMask =
            (1 << NavMesh.GetAreaFromName(LayerMask.LayerToName(gameObject.layer)[0] + "Walkable")) |
            (1 << NavMesh.GetAreaFromName("Walkable"));

        // Debug.Log(Resources.Load("Units/" + farmerPrefab.name + LayerMask.LayerToName(this.gameObject.layer)[0]));

        var farmerInstantiated = Instantiate(farmerPrefab, bornPoint.position,
                                             farmerPrefab.transform.rotation,
                                             gameObject.transform.Find(gameObject.name[0] + "Units").transform);

        //TODO 动态生成材质实例，需要优化
        farmerInstantiated.gameObject.transform.Find(farmerPrefab.name).gameObject.GetComponent<SpriteRenderer>()
                          .sharedMaterial = Instantiate(farmerPrefab
                                                       .gameObject.transform.Find(farmerPrefab.name).gameObject
                                                       .GetComponent<SpriteRenderer>().sharedMaterial);
        farmerInstantiated.gameObject.transform.Find(farmerPrefab.name).gameObject.GetComponent<SpriteRenderer>()
                          .sharedMaterial.SetTexture(
                                                     "_BaseMap",
                                                     Resources.Load("Units/" + farmerPrefab.name +
                                                                    LayerMask.LayerToName(gameObject.layer)[0]) as
                                                         Texture
                                                    );

        farmerInstantiated.GetComponent<NavMeshAgent>().areaMask = resourceLayerMask.value;
        farmerInstantiated.tag                                   = rd.ToString();
        farmerInstantiated.sidePlayer                            = this;

        farmerInstantiated.road = rd;
        farmerInstantiated.ResouceCarried = resourceCarried > farmerInstantiated.maxLoad[(int) rd]
            ? farmerInstantiated.maxLoad[(int) rd]
            : resourceCarried;

        //Debug.Log(this.gameObject.name + " Farmer Road :" + rd);
        farmerInstantiated.gameObject.layer = gameObject.layer;
        DispatchableFarmer--;

        farmerInstantiated.enabled = true;
    }

    // 当农民返回
    public void FarmerBack(Farmer farmer, Transform tr)
    {
        if (RoadWorkingFarmers[(int) farmer.road] > RoadFarmers[(int) farmer.road])
        {
            Destroy(farmer.gameObject);
            RoadWorkingFarmers[(int) farmer.road]--;
            DispatchableFarmer++;
        }
    }

    //空闲农民调遣
    private void DispatchFarmer()
    {
        // bool isFixed = true;
        for (var i = 0; i < 3; i++)
            if (RoadFarmers[i] > RoadWorkingFarmers[i] && DispatchableFarmer > 0)
                InstantiateFarmer((Road) i);
            // 派遣之后人数是否满足要求
            // if (RoadFarmers[i] > RoadWorkingFarmers[i]) isFixed = false;

        // if (!isFixed)
        // {
        //     updateEventHandler -= DispatchFarmer;
        // }
    }

    #endregion

    #region 农民生产

    private          bool  _isProducingFarmer;
    private readonly float _farmerProducingTime = 5f;

    private void RegisterProduceFarmer()
    {
        if (!_isProducingFarmer && FarmerCount < MaxFarmerNumber)
        {
            _isProducingFarmer = true;
            Invoke("ProduceFarmer", _farmerProducingTime);
        }
    }

    private void ProduceFarmer()
    {
        FarmerCount++;
        DispatchableFarmer++;
        _isProducingFarmer = false;
    }

    #endregion

    #region 资源生产

    public      readonly     int[]   resourceProduct = new int[]{0,0,3};
    private          bool  _isProducingResource;
    private readonly float _resourceProducingTime = 3f;

    private void RegisterProduceResource()
    {
        if (!_isProducingResource)
        {
            _isProducingResource = true;
            Invoke("ProduceResource", _resourceProducingTime);
        }
    }

    private void ProduceResource()
    {
        ChangeResource(GameResourceType.Food, resourceProduct[0]);
        ChangeResource(GameResourceType.Wood, resourceProduct[1]);
        ChangeResource(GameResourceType.Gold, resourceProduct[2]);

        _isProducingResource = false;
    }

    #endregion

    #endregion

    #region 单位调遣
    
    [Header("单位派遣")]
    [Space(10)]
    public int[] availableUnits;

    public  List<Unit> UnitsList = new List<Unit>();

    public int CountUnits(UnitType type)
    {
        return (from unit in UnitsList
                 where unit.unitType == type
                 select unit).Count();
    }
    public void SetUnits(Vector3 tr, int chosenUnit, Road rd, int number)
    {
        // Debug.Log("NUMBER:" + number);
        //TODO 人数检测
        Unit unit =  GameManager.GameManager.GetManager.unitsList[availableUnits[chosenUnit]];
        if (this.CountUnits(unit.unitType) + number > unit.playerOwnMax)
            throw new GameException("MAX UNITS!");

        for (var i = 0; i < number; i++)
        {
            //随机生成点
            var randomTr = Random.insideUnitCircle * 0.3f;
            var oriPoint = new Vector3(tr.x + randomTr.x, 0, tr.z + randomTr.y);
            // Debug.Log("SET POINT:" + oriPoint);

            InstantiateUnit(chosenUnit, rd, oriPoint);
        }
    }
    
    
    public void   SetUnits(Vector3 tr, int chosenUnit, Road rd, int number,Vector3 oriVelocity)
    {
        // Debug.Log("NUMBER:" + number);
        //TODO 人数检测
        Unit unit =  GameManager.GameManager.GetManager.unitsList[availableUnits[chosenUnit]];
        if (this.CountUnits(unit.unitType) + number > unit.playerOwnMax)
            throw new GameException("MAX UNITS!");


        for (var i = 0; i < number; i++)
        {
            //随机生成点
            var randomTr = Random.insideUnitCircle * 0.3f;
            var oriPoint = new Vector3(tr.x + randomTr.x, 0, tr.z + randomTr.y);
            // Debug.Log("SET POINT:" + oriPoint);

            InstantiateUnit(chosenUnit, rd, oriPoint, oriVelocity);
        }
    }

    //生成单个单位
    public virtual void InstantiateUnit(int chosenUnit, Road rd, Vector3 oriPoint)
    {
        var unitInstantiated = InstantiateUnitBase(chosenUnit, rd, oriPoint);
        var unit = unitInstantiated.GetComponent<Unit>();
        unit.enabled = true;
        UnitsList.Add(unit);
        unit.UnitDeathEventHandler.AddListener(((p, m) =>
                                                {
                                                    this.UnitsList.Remove(unit);
                                                }));
    }
    
    public virtual void  InstantiateUnit(int chosenUnit, Road rd, Vector3 oriPoint,Vector3 oriVelocity)
    {
        var unitInstantiated = InstantiateUnitBase(chosenUnit, rd, oriPoint);        
        try
        {
            ((IMilitaryUnit) (unitInstantiated.gameObject.GetComponent<Unit>())).OriginalVelocity = oriVelocity;
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        Unit unit = unitInstantiated.GetComponent<Unit>();
        unit.enabled = true;
        UnitsList.Add(unit);
        unit.UnitDeathEventHandler.AddListener(((p, m) =>
                                                {
                                                    this.UnitsList.Remove(unit);
                                                }));

    }


    private GameObject InstantiateUnitBase(int chosenUnit, Road rd, Vector3 oriPoint)
    {
        GameObject unitGb = GameManager.GameManager.GetManager.unitsList[ availableUnits[chosenUnit]].gameObject;
        //寻路设置
        LayerMask layerMask =
            (1 << NavMesh.GetAreaFromName(LayerMask.LayerToName(gameObject.layer)[0] + "Walkable")) |
            (1 << NavMesh.GetAreaFromName("Walkable"));

        var unitInstantiated =
            Instantiate(unitGb, oriPoint, unitGb.transform.rotation,
                        gameObject.transform.Find(gameObject.name[0] + "Units").transform);


        //TODO 动态生成材质实例，需要优化
        try
        {
            unitInstantiated.gameObject.transform.Find(unitGb.name).gameObject.GetComponent<SpriteRenderer>()
                            .sharedMaterial = Instantiate(unitGb.gameObject.transform.Find(unitGb.name).gameObject
                                                                .GetComponent<SpriteRenderer>().sharedMaterial);

            unitInstantiated.gameObject.transform.Find(unitGb.name).gameObject.GetComponent<SpriteRenderer>()
                            .sharedMaterial.SetTexture(
                                                       "_BaseMap",
                                                       Resources.Load("Units/" + unitGb.name +
                                                                      LayerMask.LayerToName(gameObject.layer)[0]) as Texture
                                                      );

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // throw;
        }

        unitInstantiated.gameObject.tag                        = rd.ToString();
        unitInstantiated.gameObject.layer                      = gameObject.layer;
        // unitInstantiated.GetComponent<NavMeshAgent>().areaMask = layerMask.value;

        unitInstantiated.gameObject.GetComponent<Unit>().road       = rd;
        unitInstantiated.gameObject.GetComponent<Unit>().sidePlayer = this;

        
        return unitInstantiated;
    }


    #endregion
}