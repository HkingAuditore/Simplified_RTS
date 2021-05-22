using UnityEngine;
using UnityEngine.Events;

namespace Units.RangedAttack
{
    /// <summary>
    ///     远程攻击单位
    /// </summary>
    public class RangedAttackUnit : Unit, IMilitaryUnit
    {
        /// <summary>
        ///     攻击力
        /// </summary>
        [Header("战斗属性")] [Space(10)] [SerializeField]
        private int attackPower;

        /// <summary>
        ///     攻击冷却时间
        /// </summary>
        [SerializeField] private float attackColdDownTime;

        /// <summary>
        ///     攻击范围
        /// </summary>
        [SerializeField] private float attackRange;

        /// <summary>
        ///     攻击初速度
        /// </summary>
        [HideInInspector] public Vector3 originalVelocity;

        /// <summary>
        ///     射击坐标
        /// </summary>
        [Header("射击属性")] [Space(10)] public Transform shootTransform;

        /// <summary>
        ///     子弹预制体
        /// </summary>
        public GameObject bulletObject;

        /// <summary>
        ///     寻敌范围
        /// </summary>
        public float findEnemyRadius = 8.8f;


        /************战斗*****************/
        /// <summary>
        ///     射击矫正角度
        /// </summary>
        public float shootAngleOffset;

        private FindEnemyTrigger _attackTrigger;
        private Unit             _enemyUnit;


        /********寻敌********/
        private float _giveUpRadius = 8f;
        private bool  _isFoundEnemy;
        private float _maxShootSpeed;

        private float _timer;

        public override void Start()
        {
            BeAttackedEventHandler.AddListener(AttackedReact);
            if (!isUnmovable)
                // TODO StartEventHandler += () => navMeshAgent.stoppingDistance = attackRange * 0.8f;

                unitRigidbody.velocity = OriginalVelocity;
            InitTarget                    = GetEnemySide();
            _maxShootSpeed                = Mathf.Sqrt(attackRange * Bullet.Gravity / Mathf.Sin(45f * 2 * Mathf.Deg2Rad));
            _attackTrigger                = transform.Find("FindEnemyRange").GetComponent<FindEnemyTrigger>();
            pathFinder.endReachedDistance = this.attackRange * .8f;

            FindEnemy();

            base.Start();
        }

        private void FixedUpdate()
        {
            // _timer += Time.deltaTime;
            //
            // // if (_enemyUnit != null)
            // //     Goto(_enemyUnit.transform);
            // // 寻敌攻击
            // if (!_isFoundEnemy || _enemyUnit == null || _isFoundEnemy && _enemyUnit.HP <= 0)
            // {
            //     //Debug.Log("FIND NEW!");
            //     FindEnemy();
            //     if (_enemyUnit != null)
            //     {
            //         Goto(_enemyUnit.transform);
            //         _isFoundEnemy = true;
            //     }
            //     else
            //     {
            //         Goto(InitTarget);
            //         _isFoundEnemy = false;
            //     }
            //
            //     // if (!isUnmovable)
            //     // TODO navMeshAgent.speed = Speed;
            // }
            // else if (_isFoundEnemy)
            // {
            //     Goto(_enemyUnit.transform);
            //     if (_timer > attackColdDownTime)
            //     {
            //         Attack();
            //         _timer = 0f;
            //     }
            // }
            _timer += Time.deltaTime;
            // if(_enemyUnit!=null && Vector3.Distance( this.navMeshAgent.destination,this._enemyUnit.transform.position) > .1f)
            // {
            //     this.Goto(_enemyUnit.transform);
            // }
            // 寻敌攻击
            if (!_isFoundEnemy || _enemyUnit == null || (_isFoundEnemy && _enemyUnit?.HP <= 0))
            {
                FindEnemy();
                if(!isUnmovable)
                    this.pathFinder.maxSpeed = this.Speed;
                if (_enemyUnit != null)
                {
                    Goto(_enemyUnit.transform);
                    _isFoundEnemy = true;
                }
                else
                {
                    Goto(InitTarget);
                    _isFoundEnemy = false;
                }
            }
            // else if (_isFoundEnemy)
            // {
            //     Goto(_enemyUnit.transform);
                // Debug.DrawLine(this.transform.position,_enemyUnit.transform.position,this.sidePlayer.gameObject.name == "APlayer" ? Color.green : Color.magenta);
                // Debug.DrawLine(this.transform.position,this.navMeshAgent.destination,this.sidePlayer.gameObject.name == "APlayer" ? Color.cyan : Color.yellow);
                if(_isFoundEnemy && _enemyUnit != null)
                {
                    if (_timer                                                              > AttackColdDownTime &&
                        Vector3.Distance(transform.position, _enemyUnit.transform.position) < AttackRange)
                    {
                        Attack();
                        pathFinder.maxSpeed = 0f;
                        _timer              = 0f;
                    }

                    if (Vector3.Distance(transform.position, _enemyUnit.transform.position) > AttackRange)
                    {
                        if(!isUnmovable)
                            this.pathFinder.maxSpeed = this.Speed;
                    }
                }
            // }

        }


        /// <summary>
        ///     攻击冷却时间
        /// </summary>
        public float AttackColdDownTime
        {
            get => attackColdDownTime;
            set => attackColdDownTime = value;
        }

        /// <summary>
        ///     攻击力
        /// </summary>
        public int AttackValue
        {
            get => attackPower;
            set => attackPower = value;
        }

        /// <summary>
        ///     攻击范围
        /// </summary>
        public float AttackRange
        {
            get => attackRange;
            set => attackRange = value;
        }

        /// <summary>
        ///     防御力
        /// </summary>
        public int DefenceValue
        {
            get => defence;
            set => defence = value;
        }

        /// <summary>
        ///     速度
        /// </summary>
        public float SpeedValue
        {
            get => Speed;
            set => Speed = value;
        }

        /// <summary>
        ///     初速度
        /// </summary>
        public Vector3 OriginalVelocity
        {
            get => originalVelocity;
            set => originalVelocity = value;
        }

        /// <summary>
        ///     玩家
        /// </summary>
        public Player.Player SidePlayer
        {
            get => sidePlayer;
            set => sidePlayer = value;
        }

        /// <summary>
        ///     攻击事件
        /// </summary>
        public UnityEvent<Unit> AttackEvent { get; } = new UnityEvent<Unit>();

        /// <summary>
        ///     被攻击事件
        /// </summary>
        public UnityEvent<IMilitaryUnit> UnderAttackedEvent { get; } = new UnityEvent<IMilitaryUnit>();

        /// <summary>
        ///     获取单位
        /// </summary>
        /// <returns></returns>
        public Unit GetUnit()
        {
            return this;
        }

        #region 寻敌

        private Transform GetEnemySide()
        {
            return LayerMask.LayerToName(gameObject.layer) == "ASide"
                ? GameManager.GameManager.GetManager.bSide.door.transform
                : GameManager.GameManager.GetManager.aSide.door.transform;
        }

        private void FindEnemy()
        {
            _enemyUnit = _attackTrigger.GetEnemyInList();
        }

        private float GetAgentDistanceOnNav(Vector3 targetPoint)
        {
            return GetTwoPointDistanceOnNav(transform.position, targetPoint);
        }

        #endregion

        #region 战斗

        /// <summary>
        ///     攻击
        /// </summary>
        public void Attack()
        {
            AttackEvent.Invoke(_enemyUnit);
            // Debug.Log(this.navMeshAgent.velocity.magnitude);
            var distance = Vector3.Distance(transform.position, _enemyUnit.transform.position);
            if (distance < attackRange && (isUnmovable || unitRigidbody.velocity.magnitude < 2f))
            {
                // if (!isUnmovable)
                // {
                shootTransform.LookAt(_enemyUnit.transform);
                // }else
                // {
                //     shootTransform.LookAt(_enemyUnit.transform);
                // }                // if (!isUnmovable)
                //     TODO navMeshAgent.speed = 0f;

                //射击角度 0~45
                //两者距离为0时，出射角度为0,；距离到attackrange时，出射角度为45
                //生成原因，需要再求一次余角
                var shootAngle = 90 - Mathf.Clamp01(distance / attackRange) * 45 * 0.65f + shootAngleOffset;

                var shootRotation = new Vector3(0f, shootTransform.eulerAngles.y - 90f, shootAngle);

                bulletObject.gameObject.layer = gameObject.layer;


                var bullet =
                    Instantiate(bulletObject, shootTransform.position, Quaternion.Euler(shootRotation),
                                gameObject.transform.parent.parent
                                          .Find(gameObject.transform.parent.gameObject.name[0] + "Item").transform);

                var expectSpeed = Mathf.Clamp(
                                              Mathf.Sqrt(
                                                         Vector3.Distance(shootTransform.position,
                                                                          _enemyUnit.transform.position) * 1.2f *
                                                         Bullet.Gravity / Mathf.Sin(shootRotation.z * 2 * Mathf.Deg2Rad)),
                                              0,
                                              _maxShootSpeed);
                if (double.IsNaN(expectSpeed)) expectSpeed = 0.01f;
                var bulletComponent                        = bullet.GetComponent<Bullet>();
                bulletComponent.initSpeed = expectSpeed;
                bulletComponent.SetShooter(this);
                bulletComponent.enabled = true;
            }
        }

        private void AttackedReact(IMilitaryUnit attacker)
        {
            UnderAttackedEvent.Invoke(attacker);
            try
            {
                if (attacker != null)
                    if (Vector3.Distance(transform.position, _enemyUnit.transform.position) >
                        Vector3.Distance(transform.position, attacker.GetUnit().transform.position))
                        _enemyUnit = attacker.GetUnit();
            }
            catch
            {
                // ignored
            }
        }

        #endregion
    }
}