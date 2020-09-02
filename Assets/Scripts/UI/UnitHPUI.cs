using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.UI;

public class UnitHPUI : MonoBehaviour
{
    public Unit unit;

    private float _hpFull;
    private Image _fr;

    private void Awake()
    {
        this._hpFull = unit.HP;
        _fr = this.transform.Find("FR").GetComponent<Image>();
    }

    void Update()
    {
        _fr.fillAmount =Mathf.Clamp(unit.HP / _hpFull,0f,100f) ;
    }
}
