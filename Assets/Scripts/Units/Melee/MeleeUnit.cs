using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;
using UnityEngine.AI;


public class MeleeUnit : Unit, IMilitaryUnit
{
    
    [SerializeField] private int attackPower;
    [SerializeField] private float attackColdDownTime;
    [SerializeField] private float attackRange;

    public int AttackPower => attackPower;

    public float AttackColdDownTime => attackColdDownTime;

    public float AttackRange => attackRange;

    public override void Start()
    {
        this.InitTarget = GetEnemySide();
        _attackTrigger = this.transform.Find("FindEnemyRange").GetComponent<FindEnemyTrigger>();
        FindEnemy();
        base.Start();
    }

    private float _timer;
    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        // if(_enemyUnit!=null && Vector3.Distance( this.navMeshAgent.destination,this._enemyUnit.transform.position) > .1f)
        // {
        //     this.Goto(_enemyUnit.transform);
        // }
        // 寻敌攻击
        if (!_isFoundEnemy || _enemyUnit == null || (_isFoundEnemy && _enemyUnit?.HP <= 0) )
        {
            FindEnemy();
            if (_enemyUnit != null)
            {
                this.Goto(_enemyUnit.transform);
                _isFoundEnemy = true;

            }
            else
            {
                this.Goto(this.InitTarget);
                _isFoundEnemy = false;
            }
        }
        else if (_isFoundEnemy)
        {
            if (_timer > AttackColdDownTime && Vector3.Distance(this.transform.position,_enemyUnit.transform.position) < AttackRange)
            {
                this.Attack();
                _timer = 0f;
            }
        }

        // Debug.Log(this.navMeshAgent.destination);
    }

    private Transform GetEnemySide() =>
        (LayerMask.LayerToName(this.gameObject.layer) == "ASide")
            ? GameObject.Find("BDoor").transform
            : GameObject.Find("ADoor").transform;


    /********寻敌********/
    private float _giveUpRadius = 1.5f;
    public bool _isFoundEnemy = false;
    public float findEnemyRadius = 2.2f;
    public Unit _enemyUnit;

    private FindEnemyTrigger _attackTrigger;

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
        
        this._enemyUnit = _attackTrigger.GetEnemyInList();
        try
        {
            Debug.Log( this.gameObject.name + " Find Enemy :: " + this._enemyUnit .gameObject.GetComponent<Unit>().gameObject.name);

        }
        catch
        {
            // ignored
        }


        // Debug.Log("END FIND:" + this._enemyUnit.gameObject.name);
    }


    private float GetAgentDistanceOnNavMesh(Vector3 targetPoint) =>
        Unit.GetTwoPointDistanceOnNavMesh(this.transform.position, targetPoint,
            LayerMask.LayerToName(this.gameObject.layer) == "ASide");
    
    
    
    /************战斗*****************/
    public void Attack()
    {
        _enemyUnit.BeAttacked(this);
        if (_enemyUnit.HP <= 0) _isFoundEnemy = false;
    }

    public Unit GetUnit()
    {
        return this;
    }

    private void AttackedReact(Unit attacker)
    {
        if (Vector3.Distance(this.transform.position, _enemyUnit.transform.position) >
            Vector3.Distance(this.transform.position, attacker.transform.position))
        {
            this._enemyUnit = attacker;
        }
    }

}