using Units;
using UnityEngine;

public class MeleeUnit : Unit, IMilitaryUnit
{
    [SerializeField] private int       attackPower;
    [SerializeField] private float     attackColdDownTime;
    [SerializeField] private float     attackRange;
    public                   Vector3   originalVelocity;
    public                   float     findEnemyRadius = 2.2f;

    private                  bool      _isFoundEnemy;
    private                  Unit      _enemyUnit;

    private FindEnemyTrigger _attackTrigger;


    /********寻敌********/
    private float _giveUpRadius = 1.5f;

    private float _timer;


    public override void Start()
    {
        
        InitTarget              = GetEnemySide();
        _attackTrigger          = transform.Find("FindEnemyRange").GetComponent<FindEnemyTrigger>();
        // UnitRigidbody          = this.GetComponent<Rigidbody>();
        UnitRigidbody.velocity = OriginalVelocity;
        // Debug.Log(this.GetComponent<Rigidbody>().velocity);
        FindEnemy();
        base.Start();
    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        // if(_enemyUnit!=null && Vector3.Distance( this.navMeshAgent.destination,this._enemyUnit.transform.position) > .1f)
        // {
        //     this.Goto(_enemyUnit.transform);
        // }
        // 寻敌攻击
        if (!_isFoundEnemy || _enemyUnit == null || (_isFoundEnemy && _enemyUnit?.HP <= 0))
        {
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
        }
        else if (_isFoundEnemy)
        {
            Goto(_enemyUnit.transform);
            // Debug.DrawLine(this.transform.position,_enemyUnit.transform.position,this.sidePlayer.gameObject.name == "APlayer" ? Color.green : Color.magenta);
            // Debug.DrawLine(this.transform.position,this.navMeshAgent.destination,this.sidePlayer.gameObject.name == "APlayer" ? Color.cyan : Color.yellow);
            if (_timer                                                              > AttackColdDownTime &&
                Vector3.Distance(transform.position, _enemyUnit.transform.position) < AttackRange)
            {
                Attack();
                _timer = 0f;
            }
        }

        // Debug.Log(this.navMeshAgent.destination);
    }

    public int AttackValue
    {
        get => attackPower;
        set => attackPower = value;
    }

    public float AttackColdDownTime
    {
        get => attackColdDownTime;
        set => attackColdDownTime = value;
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

    private Transform GetEnemySide()
    {
        return LayerMask.LayerToName(gameObject.layer) == "ASide"
            ? GameObject.Find("BDoor").transform
            : GameObject.Find("ADoor").transform;
    }

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

        _enemyUnit = _attackTrigger.GetEnemyInList();
        try
        {
            Debug.Log(gameObject.name + " Find Enemy :: " + _enemyUnit.gameObject.GetComponent<Unit>().gameObject.name);
        }
        catch
        {
            // ignored
        }


        // Debug.Log("END FIND:" + this._enemyUnit.gameObject.name);
    }


    private float GetAgentDistanceOnNavMesh(Vector3 targetPoint)
    {
        return GetTwoPointDistanceOnNavMesh(transform.position, targetPoint,
                                            LayerMask.LayerToName(gameObject.layer) == "ASide");
    }

    private void AttackedReact(Unit attacker)
    {
        if (Vector3.Distance(transform.position, _enemyUnit.transform.position) >
            Vector3.Distance(transform.position, attacker.transform.position))
            _enemyUnit = attacker;
    }
}