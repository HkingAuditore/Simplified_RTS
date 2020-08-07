using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MeleeUnit : Unit
{
    public int attack;
    public float attackColdDownTime;
    public float attackRange;
    


    public void Awake()
    {
        this.InitTarget = GetEnemySide();
        FindEnemy();

    }

    private float _timer;
    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        if(_enemyUnit!=null)
            this.Goto(_enemyUnit.transform);
        // 寻敌攻击
        if (!_isFoundEnemy || _enemyUnit?.HP <= 0)
        {
            FindEnemy();
            if(_enemyUnit!=null)
                this.Goto(_enemyUnit.transform);
            else
            {
                this.Goto(this.InitTarget);
            }
        }
        else if (_isFoundEnemy)
        {
            if (_timer > attackColdDownTime)
            {
                this.Attack();
                _timer = 0f;
            }
        }
    }

    private Transform GetEnemySide() =>
        ( LayerMask.LayerToName(this.gameObject.layer) == "ASide")
            ? GameObject.FindGameObjectsWithTag("Door")[1].transform
            : GameObject.FindGameObjectsWithTag("Door")[0].transform;


    /********寻敌********/
    private bool _isFoundEnemy = false;
    private float _findEnemyRadius = 2.5f;
    private Unit _enemyUnit;

    private void FindEnemy()
    {
        var enemyLayer = LayerMask.LayerToName(this.gameObject.layer) == "ASide" ? "BSide" : "ASide";
        Collider[] enemiesCol = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(this.transform.position, _findEnemyRadius, enemiesCol, 1 << LayerMask.NameToLayer(enemyLayer));
        if (size == 0) 
            return;
        Array.Resize(ref enemiesCol, size);
        _isFoundEnemy = true;
        this._enemyUnit =  (from enemy in enemiesCol
                            where enemy.gameObject.GetComponent<Unit>()?.road == this.road
                            orderby Vector3.Distance(enemy.transform.position, this.transform.position)
                            select enemy).ToArray()[0].gameObject.GetComponent<Unit>();
        Debug.Log("END FIND:" + this._enemyUnit.gameObject.name);
    }
    
    
    
    /************战斗*****************/
    private void Attack()
    {
        if (Vector3.Distance(this.transform.position, _enemyUnit.transform.position) < attackRange)
        {
            _enemyUnit.HP -= (this.attack-_enemyUnit.defence) > 0 ? (this.attack-_enemyUnit.defence) : 1;
        }

        if (_enemyUnit.HP <= 0) _isFoundEnemy = false;
    }
}