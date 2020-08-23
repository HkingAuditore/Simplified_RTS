using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Units
{
    public enum Road
    {
        Top,Mid,Bot
    }

    public interface IMilitaryUnit
    {
        int AttackPower { get; }
        float AttackColdDownTime { get; }
        float AttackRange { get; }

        void Attack();
    }
    
    
    public class Unit : MonoBehaviour
    {
        //基本属性
        
        public Road road;
        public Player sidePlayer;
        public bool isUnmovable;

        // 能力属性
        [SerializeField] private int _hp;
        public int defence;
        public int HP
        {
            get => _hp;
            protected set => _hp = value >= 0 ?  value : 0;
        }

        [SerializeField] private float speed = 1;
        public float Speed => speed;


        //生产成本
        public int costFood;
        public int costWood;
        public int costGold;


        /****总****/
        protected UnityAction StartEventHandler;
        public virtual void Start()
        {
            if (!isUnmovable)
            {
                navMeshAgent = this.GetComponent<NavMeshAgent>();
                navMeshAgent.speed = this.Speed;

            }

            StartEventHandler?.Invoke();

            if (InitTarget != null) this.Goto(InitTarget);
        }

        public void Update()
        {
            if (this.HP <= 0)
            {
                UnitDeathEventHandler?.Invoke();
                GameObject.Destroy(this.gameObject);
            }
            if (!isUnmovable && navMeshAgent.remainingDistance < 0.2f && !_isAtTarget)
            {
                this.navStopEventHandler?.Invoke(this.gameObject,this.navMeshAgent.gameObject.transform);
                _isAtTarget = true;
            }
        }

        #region 寻路

        /**************寻路******************/
    
        // 目的地到达事件
        public UnityAction<GameObject, Transform> navStopEventHandler;

        protected Transform InitTarget { get; set; }
        protected NavMeshAgent navMeshAgent;
        private bool _isAtTarget = false;


        // 前往目的地
        protected void Goto(Transform tr)
        {
            this.navMeshAgent.SetDestination(tr.position);
            _isAtTarget = false;
        }

        protected static float GetTwoPointDistanceOnNavMesh(Vector3 oriPoint, Vector3 targetPoint, bool isASide)
        {
            var path = new NavMeshPath();
            var side = isASide ? "A" : "B";
            LayerMask layerMask = 1 << NavMesh.GetAreaFromName(side + "Walkable") | (1 << NavMesh.GetAreaFromName("Walkable"));
            NavMesh.CalculatePath(oriPoint, targetPoint, layerMask, path);
            float distance = 0f;
            for (int i = 1; i <path.corners.Length; i++)
            {
                distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            return distance;
        }

        

        #endregion

        #region 事件触发

        public UnityAction UnitDeathEventHandler;

        #endregion

        #region 战斗

        //收到攻击
        protected UnityAction<IMilitaryUnit> BeAttackedEventHandler;
        public void BeAttacked(IMilitaryUnit attacker)
        {
            var damage = (attacker.AttackPower - this.defence) > 0 ? (attacker.AttackPower - this.defence) : 1;
            this.HP -= damage;

            BeAttackedEventHandler?.Invoke(attacker);
        }

        #endregion


    }
}