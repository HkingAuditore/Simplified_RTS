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
        FindEnemy();
        base.Start();
    }

    private float _timer;
    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        if(_enemyUnit!=null)
            this.Goto(_enemyUnit.transform);
        // 寻敌攻击
        if (!_isFoundEnemy || _enemyUnit?.HP <= 0 || Vector3.Distance(_enemyUnit.transform.position,this.transform.position) > _giveUpRadius)
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
            }
        }
        else if (_isFoundEnemy)
        {
            if (_timer > AttackColdDownTime)
            {
                this.Attack();
                _timer = 0f;
            }
        }
    }

    private Transform GetEnemySide() =>
        (LayerMask.LayerToName(this.gameObject.layer) == "ASide")
            ? GameObject.Find("BDoor").transform
            : GameObject.Find("ADoor").transform;


    /********寻敌********/
    private float _giveUpRadius = 1.5f;
    private bool _isFoundEnemy = false;
    private float _findEnemyRadius = 1.2f;
    private Unit _enemyUnit;

    private void FindEnemy()
    {
        var enemyLayer = LayerMask.LayerToName(this.gameObject.layer) == "ASide" ? "BSide" : "ASide";
        Collider[] enemiesCol = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(this.transform.position, _findEnemyRadius, enemiesCol, 1 << LayerMask.NameToLayer(enemyLayer));
        if (size == 0) 
            return;
        Array.Resize(ref enemiesCol, size);
        try
        {
            _enemyUnit = enemiesCol.Where(enemy => enemy.gameObject.tag != "Unattackable")
                .OrderBy(enemy => GetAgentDistanceOnNavMesh(enemy.transform.position))
                .ToArray()?[0].gameObject
                .GetComponent<Unit>();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
        // Debug.Log("END FIND:" + this._enemyUnit.gameObject.name);
    }


    private float GetAgentDistanceOnNavMesh(Vector3 targetPoint) =>
        Unit.GetTwoPointDistanceOnNavMesh(this.transform.position, targetPoint,
            LayerMask.LayerToName(this.gameObject.layer) == "ASide");
    
    
    
    /************战斗*****************/
    public void Attack()
    {
        if (Vector3.Distance(this.transform.position, _enemyUnit.transform.position) < AttackRange)
        {
            _enemyUnit.BeAttacked(this);
        }

        if (_enemyUnit.HP <= 0) _isFoundEnemy = false;
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