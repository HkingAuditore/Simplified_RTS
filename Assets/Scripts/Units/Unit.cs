using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Units
{
    public enum Road
    {
        Top,
        Mid,
        Bot
    }

    public interface IMilitaryUnit
    {
        int              AttackValue        { get; set; }
        float            AttackColdDownTime { get; set; }
        float            AttackRange        { get; set; }
        int              DefenceValue       { get; set; }
        float            SpeedValue         { get; set; }
        Vector3          OriginalVelocity   { get; set; }
        Player           SidePlayer         { get; set; }
        UnityEvent<Unit> AttackEvent        { get; }
        UnityEvent<IMilitaryUnit>       UnderAttackedEvent { get; }
        


        void Attack();

        Unit GetUnit();
    }


    public class Unit : MonoBehaviour
    {
        //基本属性
        [Header("基本属性")]
        // 能力属性
        [SerializeField] private int _hp;
        public                   int    defence;
        public                   bool   isUnmovable;
        [SerializeField] private float  speed = 1;
        public                   Road   road;
        public                   Player sidePlayer;

        #region 属性封装

        private bool _death = false;

        public int HP
        {
            get => _hp;
            protected set
            {
                _hp = value >= 0 ? value : 0;
                if(_hp == 0)
                {
                    UnitDeathEventHandler?.Invoke(_attacker.SidePlayer, _attacker);
                    _death = true;
                    Destroy(gameObject,2f);
                }
            }
        }

        public float Speed
        {
            get => speed;
            set
            {
                speed               = value;
                _pathFinder.maxSpeed = value;
            }
        }

        #endregion


        //生产成本
        [Header("生产成本")]
        public int costFood;
        public                  int   costWood;
        public                  int   costGold;
        
        [Header("死亡奖励")]
        public int deathReward = 10;
        private Player        _enemyPlayer;
        private IMilitaryUnit _attacker;

        [HideInInspector]
       public Rigidbody unitRigidbody;

        #region 事件

        // 目的地到达事件
        public    UnityEvent<GameObject, Transform> navStopEventHandler;
        protected UnityEvent                        StartEventHandler;
        //收到攻击
        protected UnityEvent<IMilitaryUnit>        BeAttackedEventHandler = new UnityEvent<IMilitaryUnit>();
        public    UnityEvent<Player,IMilitaryUnit> UnitDeathEventHandler = new UnityEvent<Player, IMilitaryUnit>();

        

        #endregion


        #region 寻路

        private Transform _initTarget;
        protected Transform InitTarget
        {
            get => _initTarget;
            set => _initTarget = value;
        }

        private AIDestinationSetter _destinationSetter;
        private AIPath              _pathFinder;
        private bool                _isAtTarget;
        private float               _baseTimer;
        public  bool                IsAtEnemyDoor { get; set; } = false;
 

        #endregion


        protected virtual void Awake()
        {
            _enemyPlayer = this.sidePlayer == GameManager.GameManager.GetManager.aSide
                ? GameManager.GameManager.GetManager.bSide
                : GameManager.GameManager.GetManager.aSide;
            unitRigidbody         =  GetComponent<Rigidbody>();
            _destinationSetter    =  this.GetComponent<AIDestinationSetter>();
            _pathFinder           =  this.GetComponent<AIPath>();
            UnitDeathEventHandler.AddListener((p, m) =>
                                              {
                                                  p.ChangeResource(GameResourceType.Gold, this.deathReward);
                                              }); 
        }

        public virtual void Start()
        {
            StartEventHandler?.Invoke();

            if (InitTarget != null)
                // Debug.Log("GOTO!");
                Goto(InitTarget);
        }

        public void Update()
        {
            if(this.HP > 0)
            {
                _baseTimer += Time.deltaTime;
                try
                {
                    //TODO 这里会跳出"GetRemainingDistance" can only be called on an active agent that has been placed on a NavMesh.
                    // if (!isUnmovable && navMeshAgent.remainingDistance < 0.2f && !_isAtTarget)
                    if (!isUnmovable && !_isAtTarget && _pathFinder.reachedDestination)
                    {
                        navStopEventHandler?.Invoke(gameObject, transform);
                        _isAtTarget = true;
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
            }
            
        }

        IEnumerator WaitForDeath() {
            // TODO 死亡五秒消失
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }

        #region 寻路

        // 前往目的地
        protected void Goto(Transform tr)
        {
            _destinationSetter.target = tr;
        }


        protected static float GetTwoPointDistanceOnNav(Vector3 oriPoint, Vector3 targetPoint)
        {
            return GameManager.GameManager.GetManager.seeker.StartPath(oriPoint, targetPoint).GetTotalLength();
        }

        #endregion

        #region 战斗


        public void BeAttacked(IMilitaryUnit attacker)
        {
            if (this._death)
                throw new Exception("WAS DEAD");
            var damage = attacker.AttackValue - defence > 0 ? attacker.AttackValue - defence : 1;
            HP -= damage;
            this._attacker = attacker;
            BeAttackedEventHandler?.Invoke(attacker);
        }

        #endregion
    }
}