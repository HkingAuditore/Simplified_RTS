using System;
using System.Collections.Generic;
using System.Linq;
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
        public float  accelarateForce;

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

        private void Awake()
        {
            UnitRigidbody = this.GetComponent<Rigidbody>();
        }

        public virtual void Start()
        {
            
            if (!isUnmovable)
            {
                navMeshAgent.updatePosition = false;
                navMeshAgent.updateRotation = false;
                
                // navMeshAgent.speed = Speed;
            }

            StartEventHandler?.Invoke();

            if (InitTarget != null)
            {
                // Debug.Log("GOTO!");
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
                // if (!isUnmovable && navMeshAgent.remainingDistance < 0.2f && !_isAtTarget)
                if (!isUnmovable &&  !_isAtTarget && _pathPos.Count == 0)
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
            // Debug.Log("is unmovable? "+this.isUnmovable);
            // Debug.Log("remaining distance "+this.navMeshAgent.destination);
            if (!this.isUnmovable)
            {
                this.Move();
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

        public NavMeshAgent navMeshAgent;

        private bool _isAtTarget;
        public  bool IsAtEnemyDoor { get; set; } = false;

        private List<Vector3> _pathPos = new List<Vector3>();
        // 前往目的地
        protected void Goto(Transform tr)
        {
            if (!isUnmovable)
            {
                // Debug.Log("Target:" + tr.position);
                // Debug.DrawLine(this.transform.position,tr.position,Color.green);
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(tr.position);
                NavMeshPath p = new NavMeshPath();
                navMeshAgent.CalculatePath(tr.position, p);
                _pathPos              = p.corners.ToList();
                _pathPos.RemoveAt(0);
                _pathPos.ForEach(v => v.y = this.transform.position.y);
                navMeshAgent.enabled = false;
            } 
            _isAtTarget = false;
        }

        private void Move()
        {
            // this.UnitRigidbody.angularVelocity = Vector3.zero;
            Debug.Log(this.gameObject.name+" distance: " + Vector3.Distance(this.transform.position, _pathPos[0]));

            if (Vector3.Distance(this.transform.position, _pathPos[0]) < .35f)
            {
                // this.UnitRigidbody.velocity = Vector3.zero;
                _pathPos.RemoveAt(0);
                if (_pathPos.Count == 0) return;
            }
            for (int i = 1; i < _pathPos.Count; i++)
            {
                Debug.DrawLine(_pathPos[i -1], _pathPos[i], Color.magenta);
            }


            // Debug.DrawLine(this.transform.position, _pathPos[0], Color.magenta);


            _pathPos[0] = new Vector3(_pathPos[0].x, this.transform.position.y, _pathPos[0].z);
            Vector3 dir = (_pathPos[0] - this.transform.position).normalized;
            // Debug.DrawLine(this.transform.position, this.transform.position + dir * 10 ,Color.cyan);
            // this.transform.LookAt(_pathPos[0]);
            Debug.DrawLine(this.transform.position, this.transform.position + dir * accelarateForce);

            if(this.UnitRigidbody.velocity.magnitude <= speed)
            {
                this.UnitRigidbody.AddForce(dir * accelarateForce);
                // Debug.DrawLine(this.transform.position, this.transform.position + transform.forward * accelarateForce);
            }
            
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