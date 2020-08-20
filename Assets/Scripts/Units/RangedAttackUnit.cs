using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;

public class RangedAttackUnit : Unit, IMilitaryUnit
{
    [SerializeField] private int attackPower;
    [SerializeField] private float attackColdDownTime;
    [SerializeField] private float attackRange;

    public float AttackColdDownTime => attackColdDownTime;
    public int AttackPower => attackPower;
    public float AttackRange => attackRange;

    public Transform shootTransform;
    
    //子弹相关
    public GameObject bulletObject;
    private float _maxShootSpeed;
    
    public void Awake()
    {
        StartEventHandler += () => this.navMeshAgent.stoppingDistance = attackRange * 0.8f;
        this.InitTarget = GetEnemySide();
        _maxShootSpeed = Mathf.Sqrt(attackRange * Bullet.Gravity / (float) Mathf.Sin(45f * 2 * Mathf.Deg2Rad));
        FindEnemy();

    }

    private float _timer;
    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        if(_enemyUnit!=null)
            this.Goto(_enemyUnit.transform);
        // 寻敌攻击
        if (!_isFoundEnemy || _enemyUnit?.HP <= 0 || Vector3.Distance(_enemyUnit.transform.position,this.transform.position) > attackRange)
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
            this.navMeshAgent.speed = this.Speed;

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
    private float _giveUpRadius = 8f;
    private bool _isFoundEnemy = false;
    private float _findEnemyRadius = 8.8f;
    private Unit _enemyUnit;

    private void FindEnemy()
    {
        var enemyLayer = LayerMask.LayerToName(this.gameObject.layer) == "ASide" ? "BSide" : "ASide";
        Collider[] enemiesCol = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(this.transform.position, _findEnemyRadius, enemiesCol, 1 << LayerMask.NameToLayer(enemyLayer));
        if (size == 0) 
            return;
        Array.Resize(ref enemiesCol, size);
        
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
    public void Attack()
    {
        // Debug.Log(this.navMeshAgent.velocity.magnitude);
        float distance = Vector3.Distance(this.transform.position, _enemyUnit.transform.position);
        if (distance < attackRange && this.navMeshAgent.velocity.magnitude < 2f)
        {
            this.transform.LookAt(_enemyUnit.transform);
            this.navMeshAgent.speed = 0f;
            
            //射击角度 0~45
            //两者距离为0时，出射角度为0,；距离到attackrange时，出射角度为45
            //生成原因，需要再求一次余角
            float shootAngle = 90 - Mathf.Clamp01(distance / attackRange) * 45 * 0.65f;

            Vector3 shootRotation =  new Vector3(shootTransform.eulerAngles.x,shootTransform.eulerAngles.y-90f,shootAngle );
            
            bulletObject.gameObject.layer = this.gameObject.layer;
            

            var bullet = 
                Instantiate(bulletObject, shootTransform.position, Quaternion.Euler(shootRotation), this.gameObject.transform.parent.parent.Find(this.gameObject.transform.parent.gameObject.name[0] + "Item").transform);

            float expectSpeed = Mathf.Clamp(
                (float) Mathf.Sqrt(
                                Vector3.Distance(shootTransform.position, _enemyUnit.transform.position) * 1.2f *
                                Bullet.Gravity / (float) Mathf.Sin(shootRotation.z * 2 * Mathf.Deg2Rad)),
                        0,
                            _maxShootSpeed);
            
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.initSpeed = expectSpeed;
            bulletComponent.SetShooter(this);
            bulletComponent.enabled = true;
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
