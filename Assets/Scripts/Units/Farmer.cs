using System;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Units
{
    /// <summary>
    ///     农民
    /// </summary>
    public class Farmer : Unit
    {
        /// <summary>
        ///     工作时长
        /// </summary>
        public float workTime = 5f;

        /// <summary>
        ///     资源携带量
        /// </summary>
        public int[] maxLoad = {10, 5, 10};

        protected Transform _home;

        private GameResourceType _resouceType;

        protected Transform _targerResource;


        /// <summary>
        ///     返回事件
        /// </summary>
        public UnityAction<Farmer, Transform> farmerBackEventHandler;

        /// <summary>
        ///     携带的资源数量
        /// </summary>
        public int ResouceCarried { get; protected internal set; }


        public override void Start()
        {
            GenerateWorkPath();
            navStopEventHandler.AddListener(OnNavStop);
            ;

            farmerBackEventHandler += Drop;
            farmerBackEventHandler += sidePlayer.FarmerBack;

            //死亡处理
            UnitDeathEventHandler.AddListener((p, m) =>
                                              {
                                                  sidePlayer.FarmerCount--;
                                                  sidePlayer.RoadFarmers[(int) road]--;
                                                  sidePlayer.RoadWorkingFarmers[(int) road]--;
                                              });


            if (ResouceCarried > 0)
            {
                InitTarget = _home;
            }
            else
            {
                ResouceCarried = 0;
                InitTarget     = _targerResource;
            }


            base.Start();
        }

        /// <summary>
        ///     到达目的地
        /// </summary>
        /// <param name="gm"></param>
        /// <param name="tr"></param>
        protected void OnNavStop(GameObject gm, Transform tr)
        {
            // Debug.Log("To Resource:" + Vector3.Distance(tr.position,_targerResource.position));
            // Debug.Log("To Home:" + Vector3.Distance(tr.position,_home.position));
            if (Vector3.Distance(tr.position, _targerResource.position) < 0.6f)
                Invoke("Work",                                                                  workTime);
            else if (Vector3.Distance(tr.position, _home.position) < 0.6f) Invoke("FarmerBack", workTime);
        }

        /// <summary>
        ///     生成工作路径
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected void GenerateWorkPath()
        {
            _home = sidePlayer.gameObject.transform.Find("Positions").Find(road + "Ori").transform;
            // 注意资源排布要按照上中下路的顺序
            LayerMask resourceLayerMask =
                (1 << LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer)[0] + "Side")) |
                (1 << LayerMask.NameToLayer("Neutrality"));
            // GameObject[] sideResource = GameObject.FindGameObjectsWithTag("Resouce").Where(gb => (resourceLayerMask.value & 1 << gb.layer) > 0).ToArray();
            var sideResource = sidePlayer.resourceTransform;
            // Debug.Log(sideResource[0]);
            // Debug.Log(sideResource[1]);
            // Debug.Log(sideResource[2]);
            _targerResource = sideResource[(int) road];
            switch (road)
            {
                case Road.Top:
                    //this._targerResource = sideResource[0].transform;
                    _resouceType = GameResourceType.Food;
                    break;
                case Road.Mid:
                    //this._targerResource = sideResource[1].transform;
                    _resouceType = GameResourceType.Gold;
                    break;
                case Road.Bot:
                    //this._targerResource = sideResource[2].transform;
                    _resouceType = GameResourceType.Wood;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     /工作
        /// </summary>
        protected virtual void Work()
        {
            ResouceCarried += maxLoad[(int) road];
            Goto(_home);
            Debug.Log("GO HOME");
        }

        /// <summary>
        ///     农民返回
        /// </summary>
        private void FarmerBack()
        {
            farmerBackEventHandler(this, transform);
        }

        /// <summary>
        ///     放下资源
        /// </summary>
        /// <param name="farmer"></param>
        /// <param name="tr"></param>
        protected void Drop(Farmer farmer, Transform tr)
        {
            sidePlayer.ChangeResource(_resouceType, ResouceCarried);
            ResouceCarried = 0;
            Goto(_targerResource);
            Debug.Log("GO Resource");
            //查看是否人数已满
        }
    }
}