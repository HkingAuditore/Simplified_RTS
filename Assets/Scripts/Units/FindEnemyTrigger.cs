using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;

public class FindEnemyTrigger : MonoBehaviour
{
    public  bool isRangeUnit;
    private Unit _baseUnit;

    private LayerMask _enemyLayerMask;

    private List<Unit> _enemyList = new List<Unit>();

    //private string _enemyLayer;
    private string _thisRoadTag;

    private void Start()
    {
        var parent = transform.parent;

        if (isRangeUnit)
        {
            _baseUnit                             = parent.GetComponent<RangedAttackUnit>();
            GetComponent<SphereCollider>().radius = ((RangedAttackUnit) _baseUnit).AttackRange;
        }
        else
        {
            _baseUnit                             = parent.GetComponent<MeleeUnit>();
            GetComponent<SphereCollider>().radius = ((MeleeUnit) _baseUnit).findEnemyRadius;
        }

        _thisRoadTag = parent.tag;
        _enemyLayerMask =
            1 << LayerMask.NameToLayer(LayerMask.LayerToName(parent.gameObject.layer) == "ASide" ? "BSide" : "ASide");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger && !other.gameObject.CompareTag("Untagged") &&
            ((1 << other.gameObject.layer) & _enemyLayerMask.value) != 0)
        {
            // Debug.Log( _baseUnit.gameObject.name + " New Enemy In Layer!");
            Component unitComponent;
            if (other.gameObject.TryGetComponent(typeof(Unit), out unitComponent))
                // Debug.Log( _baseUnit.gameObject.name + " New Enemy In Layer!");
                if ((other.gameObject.CompareTag(_thisRoadTag)           ||
                     other.gameObject.GetComponent<Unit>().IsAtEnemyDoor ||
                     other.gameObject.CompareTag("Door")) &&(
                    other.gameObject.GetComponent<Unit>().HP > 0))
                {
                    _enemyList.Add(other.gameObject.GetComponent<Unit>());
                    Debug.Log(_baseUnit.sidePlayer.gameObject.name + "::"+_baseUnit.gameObject.name + " New Enemy In!:: " +
                             "["+other.gameObject.GetComponent<Unit>().sidePlayer.gameObject.name+"]"+ other.gameObject.GetComponent<Unit>().gameObject.name);
                }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            Component unitComponent;
            if (other.gameObject.TryGetComponent(typeof(Unit), out unitComponent))
                if (_enemyList.Contains((Unit) unitComponent))
                {
                    Debug.Log("Enemy Out!");
                    _enemyList.Remove((Unit) unitComponent);
                }
            
        }
    }

    public Unit GetEnemyInList()
    {
        if (_enemyList.Count == 0)
            return null;
        try
        {
            _enemyList = (from unit in _enemyList
                          where (unit != null && unit.HP > 0)
                          orderby Vector3.Distance(_baseUnit.transform.position, unit.transform.position)
                          select unit).ToList();
            return _enemyList.First();
        }
        catch
        {
            return null;
        }
    }
}