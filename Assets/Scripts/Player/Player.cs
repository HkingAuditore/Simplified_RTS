using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum Resource
{
    Food,
    Gold,
    Wood
}

public class ResourceRunOutException : ApplicationException
{
    public Resource resource;

    public ResourceRunOutException(Resource rs, string message = "Run Out") : base(message)
    {
        resource = rs;
    }
}

public class Player : MonoBehaviour
{
    private GameObject _gameEventHandler;
    [SerializeField] private int hp;

    public int HP
    {
        get => hp;
        set
        {
            // hp = value;
            if (value > 0)
                hp = value;
            else
                _gameEventHandler.GetComponent<ResultUI>().ShowResult(this.gameObject.name[0] == 'B');
        }
    }


    public virtual void Start()
    {
        _gameEventHandler = GameObject.Find("GameEventHandler").gameObject;
        
        this._top = this.gameObject.transform.Find("Positions").transform.Find("TopOri").transform;
        this._mid = this.gameObject.transform.Find("Positions").transform.Find("MidOri").transform;
        this._bot = this.gameObject.transform.Find("Positions").transform.Find("BotOri").transform;

        this.Food = 0;
        this.Wood = 0;
        this.Gold = 0;

        DispatchableFarmer = FarmerCount;
    }

    public UnityAction updateEventHandler;

    private void FixedUpdate()
    {
        //生产任务
        RegisterProduceFarmer();
        RegisterProduceResource();

        DispatchFarmer();
        updateEventHandler?.Invoke();
    }


    /*****资源**********/
    public int Food
    {
        get => _food;
        protected set
        {
            if (value >= 0)
            {
                _food = value;
            }
            else
            {
                throw new ResourceRunOutException(Resource.Food);
            }
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
            {
                _wood = value;
            }
            else
            {
                throw new ResourceRunOutException(Resource.Wood);
            }
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
            {
                _gold = value;
            }
            else
            {
                throw new ResourceRunOutException(Resource.Gold);
            }
            //Debug.Log("Change Gold");

            UpdateLocalPlayerProperties();
        }
    }


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

    #region 农民相关处理

    public Transform[] resourceTransform;

    #region 农民调遣

    /**********农民派遣*********/
    //农民
    public Farmer farmerPrefab;
    private int _maxFarmerNumber = 5;
    private int _farmerCount = 1;
    private int _dispatchableFarmer;
    private int[] _roadFarmers = new int[3];
    private int[] _roadWorkingFarmers = new int[3];

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

    protected Transform _top;
    protected Transform _mid;
    protected Transform _bot;
    private int _food;
    private int _wood;
    private int _gold;


    //增加一条路的农民分配
    public virtual void AddFarmer(Resource rs)
    {
        if (FarmerCount > RoadFarmers.Sum())
        {
            RoadFarmers[(int) rs]++;
        }
    }

    //减少一条路的农民分配
    public virtual void SubtractFarmer(Resource rs)
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

        //寻路设置
        LayerMask resourceLayerMask =
            (1 << NavMesh.GetAreaFromName(LayerMask.LayerToName(this.gameObject.layer)[0] + "Walkable")) |
            (1 << NavMesh.GetAreaFromName("Walkable"));

        // Debug.Log(Resources.Load("Units/" + farmerPrefab.name + LayerMask.LayerToName(this.gameObject.layer)[0]));
        
        var farmerInstantiated = (Farmer) Instantiate<Farmer>(farmerPrefab, bornPoint.position,
            farmerPrefab.transform.rotation,
            this.gameObject.transform.Find(this.gameObject.name[0] + "Units").transform);

        //TODO 动态生成材质实例，需要优化
        farmerInstantiated.gameObject.transform.Find(farmerPrefab.name).gameObject.GetComponent<SpriteRenderer>()
            .sharedMaterial = Instantiate<Material>(farmerPrefab.gameObject.transform.Find(farmerPrefab.name).gameObject
            .GetComponent<SpriteRenderer>().sharedMaterial);
        farmerInstantiated.gameObject.transform.Find(farmerPrefab.name).gameObject.GetComponent<SpriteRenderer>()
            .sharedMaterial.SetTexture(
                "_BaseMap",
                Resources.Load("Units/" + farmerPrefab.name + LayerMask.LayerToName(this.gameObject.layer)[0]) as
                    Texture
            );

        farmerInstantiated.GetComponent<NavMeshAgent>().areaMask = resourceLayerMask.value;
        farmerInstantiated.tag = rd.ToString();
        farmerInstantiated.sidePlayer = this;


        farmerInstantiated.road = rd;
        //Debug.Log(this.gameObject.name + " Farmer Road :" + rd);
        farmerInstantiated.gameObject.layer = this.gameObject.layer;
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
        for (int i = 0; i < 3; i++)
        {
            if (RoadFarmers[i] > RoadWorkingFarmers[i] && DispatchableFarmer > 0)
            {
                InstantiateFarmer((Road) i);
                // 派遣之后人数是否满足要求
                // if (RoadFarmers[i] > RoadWorkingFarmers[i]) isFixed = false;
            }
        }

        // if (!isFixed)
        // {
        //     updateEventHandler -= DispatchFarmer;
        // }
    }

    #endregion

    #region 农民生产

    private bool _isProducingFarmer = false;
    private float _farmerProducingTime = 5f;

    private void RegisterProduceFarmer()
    {
        if (!_isProducingFarmer && this.FarmerCount < this.MaxFarmerNumber)
        {
            _isProducingFarmer = true;
            Invoke("ProduceFarmer", _farmerProducingTime);
        }
    }

    private void ProduceFarmer()
    {
        this.FarmerCount++;
        this.DispatchableFarmer++;
        _isProducingFarmer = false;
    }

    #endregion

    #region 资源生产

    private bool _isProducingResource = false;
    private float _resourceProducingTime = 3f;

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
        this.ChangeResource(Resource.Food, 1);
        this.ChangeResource(Resource.Wood, 1);

        _isProducingResource = false;
    }

    #endregion

    #endregion

    #region 单位调遣

    public GameObject[] availableUnits;
    public void SetUnits(Vector3 tr, int chosenUnit, Road rd, int number)
    {
        // Debug.Log("NUMBER:" + number);


        for (int i = 0; i < number; i++)
        {
            //随机生成点
            Vector2 randomTr = Random.insideUnitCircle * 0.1f;
            var oriPoint = new Vector3(tr.x + randomTr.x, 0, tr.z + randomTr.y);
            // Debug.Log("SET POINT:" + oriPoint);
            
            InstantiateUnit(chosenUnit, rd, oriPoint);
        }
    }

    //生成单个单位
    public virtual void InstantiateUnit(int chosenUnit, Road rd, Vector3 oriPoint)
    {
        GameObject unitGb = availableUnits[chosenUnit];
        //寻路设置
        LayerMask layerMask =
            (1 << NavMesh.GetAreaFromName(LayerMask.LayerToName(this.gameObject.layer)[0] + "Walkable")) |
            (1 << NavMesh.GetAreaFromName("Walkable"));

        var unitInstantiated =
            Instantiate(unitGb, oriPoint, unitGb.transform.rotation,
                this.gameObject.transform.Find(this.gameObject.name[0] + "Units").transform);


        //TODO 动态生成材质实例，需要优化
        unitInstantiated.gameObject.transform.Find(unitGb.name).gameObject.GetComponent<SpriteRenderer>()
            .sharedMaterial = Instantiate<Material>(unitGb.gameObject.transform.Find(unitGb.name).gameObject
            .GetComponent<SpriteRenderer>().sharedMaterial);

        unitInstantiated.gameObject.transform.Find(unitGb.name).gameObject.GetComponent<SpriteRenderer>()
            .sharedMaterial.SetTexture(
                "_BaseMap",
                Resources.Load("Units/" + unitGb.name + LayerMask.LayerToName(this.gameObject.layer)[0]) as Texture
            );

        unitInstantiated.gameObject.tag = rd.ToString();
        unitInstantiated.gameObject.layer = this.gameObject.layer;
        unitInstantiated.GetComponent<NavMeshAgent>().areaMask = layerMask.value;

        unitInstantiated.gameObject.GetComponent<Unit>().road = rd;
        unitInstantiated.gameObject.GetComponent<Unit>().sidePlayer = this;

        unitInstantiated.GetComponent<Unit>().enabled = true;
    }

    #endregion

    #region 网络支持

    public virtual void UpdateLocalPlayerProperties()
    {
    }

    #endregion
}