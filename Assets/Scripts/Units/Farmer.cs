using System;
using UnityEngine;
using UnityEngine.Events;

namespace Units
{
    public class Farmer : Unit
    {
        public    float     workTime = 5f;
        public    int[]     maxLoad  = {10, 5, 10};
        protected Transform _home;

        private Resource _resouceType;

        protected Transform _targerResource;


        //返回事件
        public UnityAction<Farmer, Transform> farmerBackEventHandler;
        public int                            ResouceCarried { get; protected internal set; }


        public override void Start()
        {
            GenerateWorkPath();
            navStopEventHandler += OnNavStop;

            farmerBackEventHandler += Drop;
            farmerBackEventHandler += sidePlayer.FarmerBack;

            //死亡处理
            UnitDeathEventHandler += (p, m) =>
                                     {
                                         sidePlayer.FarmerCount--;
                                         sidePlayer.RoadFarmers[(int) road]--;
                                         sidePlayer.RoadWorkingFarmers[(int) road]--;
                                     };


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

        //到达目的地
        protected void OnNavStop(GameObject gm, Transform tr)
        {
            // Debug.Log("To Resource:" + Vector3.Distance(tr.position,_targerResource.position));
            // Debug.Log("To Home:" + Vector3.Distance(tr.position,_home.position));
            if (Vector3.Distance(tr.position, _targerResource.position) < 0.6f)
                Invoke("Work",                                                                  workTime);
            else if (Vector3.Distance(tr.position, _home.position) < 0.6f) Invoke("FarmerBack", workTime);
        }

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
                    _resouceType = Resource.Food;
                    break;
                case Road.Mid:
                    //this._targerResource = sideResource[1].transform;
                    _resouceType = Resource.Gold;
                    break;
                case Road.Bot:
                    //this._targerResource = sideResource[2].transform;
                    _resouceType = Resource.Wood;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // 工作
        protected virtual void Work()
        {
            ResouceCarried += maxLoad[(int) road];
            Goto(_home);
            Debug.Log("GO HOME");
        }

        // 放下资源
        private void FarmerBack()
        {
            farmerBackEventHandler(this, transform);
        }

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