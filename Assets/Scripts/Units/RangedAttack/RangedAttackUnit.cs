using Units;
using UnityEngine;

public class RangedAttackUnit : Unit, IMilitaryUnit
{
    [SerializeField] private int     attackPower;
    [SerializeField] private float   attackColdDownTime;
    [SerializeField] private float   attackRange;
    public                   Vector3 originalVelocity;


    public Transform shootTransform;

    //子弹相关
    public GameObject bulletObject;
    public float      findEnemyRadius = 8.8f;


    /************战斗*****************/
    public float shootAngleOffset;

    private FindEnemyTrigger _attackTrigger;
    private Unit             _enemyUnit;


    /********寻敌********/
    private float _giveUpRadius = 8f;
    private bool  _isFoundEnemy;
    private float _maxShootSpeed;

    private float   _timer;

    public override void Start()
    {
        BeAttackedEventHandler += AttackedReact;
        if (!isUnmovable)
            StartEventHandler += () => navMeshAgent.stoppingDistance = attackRange * 0.8f;
        InitTarget     = GetEnemySide();
        _maxShootSpeed = Mathf.Sqrt(attackRange * Bullet.Gravity / Mathf.Sin(45f * 2 * Mathf.Deg2Rad));
        _attackTrigger = transform.Find("FindEnemyRange").GetComponent<FindEnemyTrigger>();

        FindEnemy();

        base.Start();
    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        if (_enemyUnit != null)
            Goto(_enemyUnit.transform);
        // 寻敌攻击
        if (!_isFoundEnemy || _enemyUnit == null || _isFoundEnemy && _enemyUnit.HP <= 0)
        {
            //Debug.Log("FIND NEW!");
            FindEnemy();
            if (_enemyUnit != null)
            {
                Goto(_enemyUnit.transform);
                _isFoundEnemy = true;
            }
            else
            {
                Goto(InitTarget);
                _isFoundEnemy = false;
            }

            if (!isUnmovable)
                navMeshAgent.speed = Speed;
        }
        else if (_isFoundEnemy)
        {
            if (_timer > attackColdDownTime)
            {
                Attack();
                _timer = 0f;
            }
        }
    }

    public float AttackColdDownTime
    {
        get => attackColdDownTime;
        set => attackColdDownTime = value;
    }

    public int AttackValue
    {
        get => attackPower;
        set => attackPower = value;
    }

    public float AttackRange
    {
        get => attackRange;
        set => attackRange = value;
    }

    public int DefenceValue
    {
        get => defence;
        set => defence = value;
    }

    public float SpeedValue
    {
        get => Speed;
        set => Speed = value;
    }

    public Vector3 OriginalVelocity
    {
        get => originalVelocity;
        set => originalVelocity = value;
    }

    public void Attack()
    {
        // Debug.Log(this.navMeshAgent.velocity.magnitude);
        var distance = Vector3.Distance(transform.position, _enemyUnit.transform.position);
        if (distance < attackRange && (isUnmovable || navMeshAgent.velocity.magnitude < 2f))
        {
            if (!isUnmovable)
                transform.LookAt(_enemyUnit.transform);
            else
                shootTransform.LookAt(_enemyUnit.transform);
            if (!isUnmovable)
                navMeshAgent.speed = 0f;

            //射击角度 0~45
            //两者距离为0时，出射角度为0,；距离到attackrange时，出射角度为45
            //生成原因，需要再求一次余角
            var shootAngle = 90 - Mathf.Clamp01(distance / attackRange) * 45 * 0.65f + shootAngleOffset;

            var shootRotation = new Vector3(0f, shootTransform.eulerAngles.y - 90f, shootAngle);

            bulletObject.gameObject.layer = gameObject.layer;


            var bullet =
                Instantiate(bulletObject, shootTransform.position, Quaternion.Euler(shootRotation),
                            gameObject.transform.parent.parent
                                      .Find(gameObject.transform.parent.gameObject.name[0] + "Item").transform);

            var expectSpeed = Mathf.Clamp(
                                          Mathf.Sqrt(
                                                     Vector3.Distance(shootTransform.position,
                                                                      _enemyUnit.transform.position) * 1.2f *
                                                     Bullet.Gravity / Mathf.Sin(shootRotation.z * 2 * Mathf.Deg2Rad)),
                                          0,
                                          _maxShootSpeed);
            if (double.IsNaN(expectSpeed)) expectSpeed = 0.01f;
            var bulletComponent                        = bullet.GetComponent<Bullet>();
            bulletComponent.initSpeed = expectSpeed;
            bulletComponent.SetShooter(this);
            bulletComponent.enabled = true;
        }
    }

    public Unit GetUnit()
    {
        return this;
    }

    private Transform GetEnemySide()
    {
        return LayerMask.LayerToName(gameObject.layer) == "ASide"
            ? GameObject.Find("BDoor").transform
            : GameObject.Find("ADoor").transform;
    }

    private void FindEnemy()
    {
        // Collider[] enemiesCol = new Collider[10];
        // var size = Physics.OverlapSphereNonAlloc(this.transform.position, _findEnemyRadius, enemiesCol, 1 << LayerMask.NameToLayer(_enemyLayer));
        // if (size == 0) 
        //     return;
        // Array.Resize(ref enemiesCol, size);
        // try
        // {
        //     
        //     this._enemyUnit = enemiesCol?.Where(
        //                             enemy => ((enemy.gameObject.CompareTag(this.gameObject.tag))
        //                                             ||
        //                                             (enemy.gameObject.GetComponent<Unit>().IsAtEnemyDoor == true)
        //                                             ||
        //                                             (enemy.gameObject.CompareTag("Door"))
        //                                             )
        //                                         
        //                                             
        //                             )
        //                         ?.OrderBy(enemy =>
        //                         {
        //                             if (this.isUnmovable)
        //                                 return Vector3.Distance(this.transform.position, enemy.transform.position) * 3.5;
        //                             return GetAgentDistanceOnNavMesh(enemy.transform.position);
        //                         })
        //                         ?.ToArray()?[0]
        //                         .gameObject
        //                         .GetComponent<Unit>();
        //
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        //     //throw;
        // }

        _enemyUnit = _attackTrigger.GetEnemyInList();

        // Debug.Log("END FIND:" + this._enemyUnit.gameObject.name);
    }


    private float GetAgentDistanceOnNavMesh(Vector3 targetPoint)
    {
        return GetTwoPointDistanceOnNavMesh(transform.position, targetPoint,
                                            LayerMask.LayerToName(gameObject.layer) == "ASide");
    }


    private void AttackedReact(IMilitaryUnit attacker)
    {
        try
        {
            if (attacker != null)
                if (Vector3.Distance(transform.position, _enemyUnit.transform.position) >
                    Vector3.Distance(transform.position, attacker.GetUnit().transform.position))
                    _enemyUnit = attacker.GetUnit();
        }
        catch
        {
            // ignored
        }
    }
}