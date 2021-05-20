using System;
using Pathfinding;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Units
{
    /// <summary>
    ///     路
    /// </summary>
    public enum Road
    {
        /// <summary>
        ///     上路
        /// </summary>
        Top,

        /// <summary>
        ///     中路
        /// </summary>
        Mid,

        /// <summary>
        ///     下路
        /// </summary>
        Bot
    }

    /// <summary>
    ///     单位类型
    /// </summary>
    public enum UnitType
    {
        /// <summary>
        ///     灯笼男
        /// </summary>
        LanternBoy,

        /// <summary>
        ///     弓箭手
        /// </summary>
        Archer,

        /// <summary>
        ///     风筝女
        /// </summary>
        KiteGirl,

        /// <summary>
        ///     普通敌人
        /// </summary>
        Enemy0
    }

    /// <summary>
    ///     攻击性单位接口
    /// </summary>
    public interface IMilitaryUnit
    {
        /// <summary>
        ///     攻击力
        /// </summary>
        int AttackValue { get; set; }

        /// <summary>
        ///     攻击冷却时间
        /// </summary>
        float AttackColdDownTime { get; set; }

        /// <summary>
        ///     攻击范围
        /// </summary>
        float AttackRange { get; set; }

        /// <summary>
        ///     防御力
        /// </summary>
        int DefenceValue { get; set; }

        /// <summary>
        ///     速度
        /// </summary>
        float SpeedValue { get; set; }

        /// <summary>
        ///     初速度
        /// </summary>
        Vector3 OriginalVelocity { get; set; }

        /// <summary>
        ///     所属玩家
        /// </summary>
        Player.Player SidePlayer { get; set; }

        /// <summary>
        ///     攻击事件
        /// </summary>
        UnityEvent<Unit> AttackEvent { get; }

        /// <summary>
        ///     被攻击事件
        /// </summary>
        UnityEvent<IMilitaryUnit> UnderAttackedEvent { get; }


        /// <summary>
        ///     攻击
        /// </summary>
        void Attack();

        /// <summary>
        ///     获取单位
        /// </summary>
        /// <returns></returns>
        Unit GetUnit();
    }

    /// <summary>
    ///     单位
    /// </summary>
    public class Unit : MonoBehaviour
    {
        //基本属性
        [Header("基本属性")]
        // 能力属性
        [SerializeField]
        private int _hp;

        /// <summary>
        ///     防御力
        /// </summary>
        public int defence;

        /// <summary>
        ///     是否不可移动
        /// </summary>
        public bool isUnmovable;

        /// <summary>
        ///     玩家最大拥有数量
        /// </summary>
        public int playerOwnMax = 5;

        /// <summary>
        ///     单位刚体
        /// </summary>
        public Rigidbody unitRigidbody;

        [SerializeField] private float speed = 1;

        /// <summary>
        ///     路
        /// </summary>
        public Road road;

        /// <summary>
        ///     所属玩家
        /// </summary>
        public Player.Player sidePlayer;

        /// <summary>
        ///     单位类型
        /// </summary>
        public UnitType unitType;


        /// <summary>
        ///     生产成本（食物）
        /// </summary>
        [Header("生产成本")] public int costFood;

        /// <summary>
        ///     生产成本（木材）
        /// </summary>
        public int costWood;

        /// <summary>
        ///     生产成本（黄金）
        /// </summary>
        public int costGold;

        public bool isDieImmediately = false;

        /// <summary>
        ///     击杀奖励
        /// </summary>
        [Header("死亡奖励")] public int deathReward = 10;

        private protected IMilitaryUnit _attacker;
        private           Player.Player _enemyPlayer;


        protected virtual void Awake()
        {
            _enemyPlayer = sidePlayer == GameManager.GameManager.GetManager.aSide
                ? GameManager.GameManager.GetManager.bSide
                : GameManager.GameManager.GetManager.aSide;
            unitRigidbody      = GetComponent<Rigidbody>();
            _destinationSetter = GetComponent<AIDestinationSetter>();
            _pathFinder        = GetComponent<AIPath>();
            UnitDeathEventHandler.AddListener((p, m) => { p.ChangeResource(GameResourceType.Gold, deathReward); });
        }

        public virtual void Start()
        {
            StartEventHandler?.Invoke();
            if(!isUnmovable)
                this._pathFinder.maxSpeed = this.speed;

            if (InitTarget != null)
                // Debug.Log("GOTO!");
                Goto(InitTarget);
        }

        public virtual void Update()
        {
            if (HP > 0)
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

        #region 战斗

        /// <summary>
        ///     被攻击
        /// </summary>
        /// <param name="attacker"></param>
        /// <exception cref="Exception"></exception>
        public virtual void BeAttacked(IMilitaryUnit attacker)
        {
            if (Death)
                throw new Exception("WAS DEAD");
            var damage = attacker.AttackValue - defence > 0 ? attacker.AttackValue - defence : 1;
            HP        -= damage;
            _attacker =  attacker;
            BeAttackedEventHandler?.Invoke(attacker);
        }

        #endregion

        #region 属性封装

        private protected bool Death;

        /// <summary>
        ///     体力值
        /// </summary>
        public int HP
        {
            get => _hp;
            protected set
            {
                _hp = value >= 0 ? value : 0;
                if (_hp == 0)
                {
                    try
                    {
                        // Debug.Log("UnitDeathEventHandler");
                        // Debug.Log(_attacker.SidePlayer);
                        // Debug.Log(_attacker);
                        UnitDeathEventHandler?.Invoke(_attacker.SidePlayer, _attacker);
                        Debug.Log("UnitDeathEventHandler finished");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    Death = true;
                    Destroy(gameObject, isDieImmediately ? 0 : 3f);
                }
            }
        }

        /// <summary>
        ///     速度
        /// </summary>
        public float Speed
        {
            get => speed;
            set
            {
                speed                = value;
                _pathFinder.maxSpeed = value;
            }
        }

        #endregion


        #region 事件

        /// <summary>
        ///     目的地到达事件
        /// </summary>
        public UnityEvent<GameObject, Transform> navStopEventHandler;

        /// <summary>
        ///     开始事件
        /// </summary>
        protected UnityEvent StartEventHandler;

        /// <summary>
        ///     受到攻击事件
        /// </summary>
        protected UnityEvent<IMilitaryUnit> BeAttackedEventHandler = new UnityEvent<IMilitaryUnit>();

        /// <summary>
        ///     死亡事件
        /// </summary>
        public UnityEvent<Player.Player, IMilitaryUnit> UnitDeathEventHandler = new UnityEvent<Player.Player, IMilitaryUnit>();

        #endregion


        #region 寻路

        /// <summary>
        ///     初始目标
        /// </summary>
        protected Transform InitTarget { get; set; }

        private AIDestinationSetter _destinationSetter;
        private AIPath              _pathFinder;
        private bool                _isAtTarget;
        private float               _baseTimer;

        /// <summary>
        ///     是否在敌人家门口
        /// </summary>
        public bool IsAtEnemyDoor { get; set; } = false;

        #endregion

        // IEnumerator WaitForDeath() {
        //     // TODO 死亡五秒消失
        //     yield return new WaitForSeconds(2);
        //     Destroy(gameObject);
        // }

        #region 寻路

        /// <summary>
        ///     前往目的地
        /// </summary>
        /// <param name="tr"></param>
        protected void Goto(Transform tr)
        {
            _destinationSetter.target = tr;
        }


        protected static float GetTwoPointDistanceOnNav(Vector3 oriPoint, Vector3 targetPoint)
        {
            return GameManager.GameManager.GetManager.seeker.StartPath(oriPoint, targetPoint).GetTotalLength();
        }

        #endregion
    }
}