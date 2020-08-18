using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;

public class RangedAttackUnit : Unit
{
    public int attack;
    public float attackColdDownTime;
    
    public float attackRange;
    public Transform shootTransform;
    
    //子弹相关
    public GameObject BulletObject;
    public float ShootSpeed;
    
    public void Awake()
    {
        this.InitTarget = GetEnemySide();
        this.navMeshAgent.stoppingDistance = attackRange * 0.75f;
        FindEnemy();

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
    private float _giveUpRadius = 3f;
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
                            where enemy.gameObject.tag != "Unattackable"
                            orderby GetAgentDistanceOnNavMesh(enemy.transform.position)
                            select enemy).ToArray()[0].gameObject.GetComponent<Unit>();
        // Debug.Log("END FIND:" + this._enemyUnit.gameObject.name);
    }


    private float GetAgentDistanceOnNavMesh(Vector3 targetPoint) =>
        Unit.GetTwoPointDistanceOnNavMesh(this.transform.position, targetPoint,
            LayerMask.LayerToName(this.gameObject.layer) == "ASide");
    
    
    
    /************战斗*****************/
    private void Attack()
    {
        if (Vector3.Distance(this.transform.position, _enemyUnit.transform.position) < attackRange)
        {
            float shootAngle = (float)Math.Asin(
                                        Vector3.Distance(shootTransform.position, _enemyUnit.transform.position) * Bullet.Gravity * 0.5 / (ShootSpeed * ShootSpeed)
                                        )/2 * Mathf.Rad2Deg;
            Vector3 shootRotation =  new Vector3(this.transform.rotation.x,this.transform.rotation.y,shootAngle);
            BulletObject.gameObject.layer = this.gameObject.layer;
            var bullet = 
                Instantiate(BulletObject, shootTransform.position, Quaternion.Euler(shootRotation), this.gameObject.transform.parent.parent.Find(this.gameObject.transform.parent.gameObject.name[0] + "Item").transform);
            bullet.GetComponent<Bullet>().initSpeed = this.ShootSpeed;
            bullet.GetComponent<Bullet>().enabled = true;
        }
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
