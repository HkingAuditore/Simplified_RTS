using System;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class Buff : MonoBehaviour
{
    public int buffId;
    public string buffName;
    public float attackBuff = 1f;
    public float defenceBuff = 1f;
    public float speedBuff = 1f;
    public float attackRangeBuff = 1f;
    public float attackColdDownTimeBuff = 1f;
    
    public float remainTime;
    public Unit buffUnit;
    public IMilitaryUnit buffMilitaryUnit;

    private bool _isMilitaryUnit = true;
    private void Start()
    {
        try
        {
            buffUnit = this.GetComponent<Unit>();
            buffMilitaryUnit = this.GetComponent<IMilitaryUnit>();
        }
        catch
        {
            _isMilitaryUnit = false;
        }

        RegisterBuff();
    }

    private void RegisterBuff()
    {
        if (_isMilitaryUnit)
        {
            buffMilitaryUnit.AttackValue = (int) (buffMilitaryUnit.AttackValue * attackBuff);
            buffMilitaryUnit.DefenceValue = (int) (buffMilitaryUnit.DefenceValue * defenceBuff);
            buffMilitaryUnit.SpeedValue = (int) (buffMilitaryUnit.SpeedValue * speedBuff);
            buffMilitaryUnit.AttackColdDownTime = (int) (buffMilitaryUnit.AttackColdDownTime * attackColdDownTimeBuff);
            buffMilitaryUnit.AttackRange = (int) (buffMilitaryUnit.AttackRange * attackRangeBuff);

        }
        else
        {
            buffUnit.defence = (int) (buffUnit.defence * defenceBuff);
            buffUnit.Speed = (int) (buffUnit.Speed * speedBuff);
        }
        Debug.Log("In Buff [" + _isMilitaryUnit + "]");
        //启动延时自动消除
        StartCoroutine(RemoveBuff());
    }
        
    private IEnumerator RemoveBuff()
    {
        yield return new WaitForSeconds(remainTime);
        if (_isMilitaryUnit)
        {
            buffMilitaryUnit.AttackValue = (int) (buffMilitaryUnit.AttackValue / attackBuff);
            buffMilitaryUnit.DefenceValue = (int) (buffMilitaryUnit.DefenceValue / defenceBuff);
            buffMilitaryUnit.SpeedValue = (int) (buffMilitaryUnit.SpeedValue / speedBuff);
            buffMilitaryUnit.AttackColdDownTime = (int) (buffMilitaryUnit.AttackColdDownTime / attackColdDownTimeBuff);
            buffMilitaryUnit.AttackRange = (int) (buffMilitaryUnit.AttackRange / attackRangeBuff);
        }
        else
        {
            buffUnit.defence = (int) (buffUnit.defence / defenceBuff);
            buffUnit.Speed = (int) (buffUnit.Speed / speedBuff);
        }
        
        Debug.Log("Out Buff");
        Destroy(this);
    }
}
