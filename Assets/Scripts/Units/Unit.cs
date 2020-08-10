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
        [SerializeField] private int _hp;
        public int defence;

        // 基本属性
        public int HP
        {
            get => _hp;
            set => _hp = value >= 0 ?  value : 0;
        }

        public int Speed { get; }

        public Road road;
        public Player sidePlayer;


        /****总****/
        public void Start()
        {
            _agent = this.GetComponent<NavMeshAgent>();
            if (InitTarget != null) this.Goto(InitTarget);
        }

        public void Update()
        {
            if (this.HP <= 0)
            {
                GameObject.Destroy(this.gameObject);
            }
            if (_agent.remainingDistance < 0.2f && !_isAtTarget)
            {
                this.onNavStop?.Invoke(this.gameObject,this._agent.transform);
                _isAtTarget = true;
            }
        }
    
    
        /**************寻路******************/
    
        // 目的地到达事件
        public UnityAction<GameObject, Transform> onNavStop;

        protected Transform InitTarget { get; set; }
        private NavMeshAgent _agent;
        private bool _isAtTarget = false;



        // 前往目的地
        protected void Goto(Transform tr)
        {
            this._agent.SetDestination(tr.position);
            _isAtTarget = false;
        }
    
    


    }
}