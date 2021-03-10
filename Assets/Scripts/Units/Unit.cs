using System;
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
        int   AttackValue        { get; set; }
        float AttackColdDownTime { get; set; }
        float AttackRange        { get; set; }
        int   DefenceValue       { get; set; }
        float SpeedValue         { get; set; }
        Vector3 OriginalVelocity   { get; set; }
        

        void Attack();

        Unit GetUnit();
    }


    public class Unit : MonoBehaviour
    {
        //基本属性

        public Road   road;
        public Player sidePlayer;
        public bool   isUnmovable;

        // 能力属性
        [SerializeField] private int _hp;
        public                   int defence;

        [SerializeField] private float speed = 1;


        //生产成本
        public int costFood;
        public int costWood;
        public int costGold;

        private float     _baseTimer;
        protected Rigidbody UnitRigidbody;


        /****总****/
        protected UnityAction StartEventHandler;

        #region 事件触发

        public UnityAction UnitDeathEventHandler;

        #endregion

        public int HP
        {
            get => _hp;
            protected set => _hp = value >= 0 ? value : 0;
        }

        public float Speed
        {
            get => speed;
            set
            {
                speed              = value;
                navMeshAgent.speed = value;
            }
        }

        public virtual void Start()
        {
            
            if (!isUnmovable)
            {
                navMeshAgent                = GetComponent<NavMeshAgent>();
                navMeshAgent.updatePosition = false;
                navMeshAgent.updateRotation = false;
                // navMeshAgent.speed = Speed;
            }

            StartEventHandler?.Invoke();

            if (InitTarget != null)
            {
                Debug.Log("GOTO!");
                Goto(InitTarget);
            }
        }

        public void Update()
        {
            if (HP <= 0)
            {
                UnitDeathEventHandler?.Invoke();
                Destroy(gameObject);
            }

            _baseTimer += Time.deltaTime;
            try
            {
                // if(_baseTimer > 1f)
                // {
                //     var dst = navMeshAgent.destination;
                //     navMeshAgent.ResetPath();
                //     navMeshAgent.SetDestination(dst);
                //     _baseTimer = 0;
                // }      
                // if (!navMeshAgent.hasPath && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) {
                //     Debug.Log("Character stuck");
                //     navMeshAgent.enabled = false;
                //     navMeshAgent.enabled = true;
                //     Debug.Log("navmesh re enabled");
                //     // navmesh agent will start moving again
                // }
                //TODO 这里会跳出"GetRemainingDistance" can only be called on an active agent that has been placed on a NavMesh.
                if (!isUnmovable && navMeshAgent.remainingDistance < 0.2f && !_isAtTarget)
                {
                    navStopEventHandler?.Invoke(gameObject, navMeshAgent.gameObject.transform);
                    _isAtTarget = true;
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
        }

        private void LateUpdate()
        {
            Debug.Log("is unmovable? "+this.isUnmovable);
            Debug.Log("remaining distance "+this.navMeshAgent.destination);
            if (!this.isUnmovable && this.navMeshAgent.remainingDistance > .01f)
            {
                this.Move(this.navMeshAgent.nextPosition);
            }
        }


        #region 寻路

        /**************寻路******************/

        // 目的地到达事件
        public UnityAction<GameObject, Transform> navStopEventHandler;

        [SerializeField] private Transform _initTarget;

        protected Transform InitTarget
        {
            get => _initTarget;
            set => _initTarget = value;
        }

        protected                  NavMeshAgent navMeshAgent;

        private bool _isAtTarget;
        public  bool IsAtEnemyDoor { get; set; } = false;


        // 前往目的地
        protected void Goto(Transform tr)
        {
            if (!isUnmovable)
            {
                Debug.Log("Target:" + tr.position);
                navMeshAgent.SetDestination(tr.position);
            } 
            _isAtTarget = false;
        }

        public void Move(Vector3 destination)
        {
            Vector3 dir = (destination - this.transform.position).normalized;
            Debug.Log("I am moving!");
            this.UnitRigidbody.MovePosition(this.transform.position + dir * speed);
        }
        
        protected static float GetTwoPointDistanceOnNavMesh(Vector3 oriPoint, Vector3 targetPoint, bool isASide)
        {
            var path = new NavMeshPath();
            var side = isASide ? "A" : "B";
            LayerMask layerMask = (1 << NavMesh.GetAreaFromName(side + "Walkable")) |
                                  (1 << NavMesh.GetAreaFromName("Walkable"));
            NavMesh.CalculatePath(oriPoint, targetPoint, layerMask, path);
            var distance = 0f;
            for (var i = 1; i < path.corners.Length; i++)
                distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);

            return distance;
        }

        #endregion

        #region 战斗

        //收到攻击
        protected UnityAction<IMilitaryUnit> BeAttackedEventHandler;

        public void BeAttacked(IMilitaryUnit attacker)
        {
            var damage = attacker.AttackValue - defence > 0 ? attacker.AttackValue - defence : 1;
            HP -= damage;

            BeAttackedEventHandler?.Invoke(attacker);
        }

        #endregion
    }
}