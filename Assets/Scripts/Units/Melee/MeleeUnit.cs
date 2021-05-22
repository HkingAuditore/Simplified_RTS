using UnityEngine;
using UnityEngine.Events;

namespace Units.Melee
{
    /// <summary>
    ///     近战单位
    /// </summary>
    public class MeleeUnit : Unit, IMilitaryUnit
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
        ///     初速度
        /// </summary>
        [HideInInspector] public Vector3 originalVelocity;

        /// <summary>
        ///     寻敌范围
        /// </summary>
        public float findEnemyRadius = 2.2f;

        private FindEnemyTrigger _attackTrigger;
        private Unit             _enemyUnit;


        /********寻敌********/
        private float _giveUpRadius = 1.5f;

        private bool _isFoundEnemy;

        private float _timer;


        public override void Start()
        {
            _attackTrigger = transform.Find("FindEnemyRange").GetComponent<FindEnemyTrigger>();
            InitTarget     = GetEnemySide();
            // UnitRigidbody          = this.GetComponent<Rigidbody>();
            unitRigidbody.velocity             = OriginalVelocity;
            this.pathFinder.endReachedDistance = this.attackRange;
            // Debug.Log(this.GetComponent<Rigidbody>().velocity);
            FindEnemy();
            base.Start();
        }

        private void FixedUpdate()
        {
            _timer += Time.deltaTime;
            // if(_enemyUnit!=null && Vector3.Distance( this.navMeshAgent.destination,this._enemyUnit.transform.position) > .1f)
            // {
            //     this.Goto(_enemyUnit.transform);
            // }
            // 寻敌攻击
            if (!_isFoundEnemy || _enemyUnit == null || _isFoundEnemy && _enemyUnit?.HP <= 0)
            {
                FindEnemy();
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
                    _timer = 0f;
                }
            }
            // }

            // Debug.Log(this.navMeshAgent.destination);
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
        ///     攻击冷却时间
        /// </summary>
        public float AttackColdDownTime
        {
            get => attackColdDownTime;
            set => attackColdDownTime = value;
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
        ///     所属玩家
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


        #region 战斗

        /// <summary>
        ///     攻击
        /// </summary>
        public void Attack()
        {
            AttackEvent.Invoke(_enemyUnit);
            _enemyUnit.BeAttacked(this);
            if (_enemyUnit.HP <= 0) _isFoundEnemy = false;
        }

        private void AttackedReact(IMilitaryUnit attacker)
        {
            UnderAttackedEvent.Invoke(attacker);
            if (Vector3.Distance(transform.position, _enemyUnit.transform.position) >
                Vector3.Distance(transform.position, attacker.GetUnit().transform.position))
                _enemyUnit = attacker.GetUnit();
        }

        #endregion


        #region 寻敌

        private Transform GetEnemySide()
        {
            return LayerMask.LayerToName(gameObject.layer) == "ASide"
                ? GameManager.GameManager.GetManager.bSide.door.transform
                : GameManager.GameManager.GetManager.aSide.door.transform;
        }

        private void FindEnemy()
        {
            // var enemyLayer = LayerMask.LayerToName(this.gameObject.layer) == "ASide" ? "BSide" : "ASide";
            // Collider[] enemiesCol = new Collider[10];
            // var size = Physics.OverlapSphereNonAlloc(this.transform.position, FindEnemyRadius, enemiesCol, 1 << LayerMask.NameToLayer(enemyLayer));
            // if (size == 0) 
            //     return;
            // Array.Resize(ref enemiesCol, size);
            // try
            // {
            //     _enemyUnit = enemiesCol.Where(enemy => enemy.gameObject.tag != "Unattackable")
            //         .OrderBy(enemy => GetAgentDistanceOnNavMesh(enemy.transform.position))
            //         .ToArray()?[0].gameObject
            //         .GetComponent<Unit>();
            //
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            //     //throw;
            // }

            _enemyUnit = _attackTrigger.GetEnemyInList();
            // try
            // {
            //     Debug.Log(gameObject.name + " Find Enemy :: " + _enemyUnit.gameObject.GetComponent<Unit>().gameObject.name);
            // }
            // catch
            // {
            //     // ignored
            // }


            // Debug.Log("END FIND:" + this._enemyUnit.gameObject.name);
        }


        private float GetAgentDistanceOnNavM(Vector3 targetPoint)
        {
            return GetTwoPointDistanceOnNav(transform.position, targetPoint);
        }

        #endregion
    }
}