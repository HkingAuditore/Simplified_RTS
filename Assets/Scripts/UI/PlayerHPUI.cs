using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    public Player player;

    private float _hpFull;
    private Image _fr;

    private void Awake()
    {
        this._hpFull = player.HP;
        _fr = this.transform.Find("FR").GetComponent<Image>();
    }

    void Update()
    {
        _fr.fillAmount =Mathf.Clamp(player.HP / _hpFull,0f,100f) ;
    }
}
