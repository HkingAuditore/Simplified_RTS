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
    public class Unit : MonoBehaviour
    {
        //基本属性
        public Road road;
        public Player sidePlayer;

        // 能力属性
        [SerializeField] private int _hp;
        public int defence;
        public int HP
        {
            get => _hp;
            set => _hp = value >= 0 ?  value : 0;
        }

        [SerializeField] private float _speed = 1;
        public float Speed => _speed;


        //生产成本
        public int costFood;
        public int costWood;
        public int costGold;


        /****总****/
        public void Start()
        {
            _agent = this.GetComponent<NavMeshAgent>();
            _agent.speed = this.Speed;
            if (InitTarget != null) this.Goto(InitTarget);
        }

        public void Update()
        {
            if (this.HP <= 0)
            {
                unitDeathEventHandler?.Invoke();
                GameObject.Destroy(this.gameObject);
            }
            if (_agent.remainingDistance < 0.2f && !_isAtTarget)
            {
                this.navStopEventHandler?.Invoke(this.gameObject,this._agent.gameObject.transform);
                _isAtTarget = true;
            }
        }

        #region 寻路

        /**************寻路******************/
    
        // 目的地到达事件
        public UnityAction<GameObject, Transform> navStopEventHandler;

        protected Transform InitTarget { get; set; }
        private NavMeshAgent _agent;
        private bool _isAtTarget = false;


        // 前往目的地
        protected void Goto(Transform tr)
        {
            this._agent.SetDestination(tr.position);
            _isAtTarget = false;
        }
        

        #endregion

        #region 事件触发

        public UnityAction unitDeathEventHandler;

        #endregion



    }
}