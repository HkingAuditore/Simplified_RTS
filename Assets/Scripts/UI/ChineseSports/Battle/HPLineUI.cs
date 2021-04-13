using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPLineUI : MonoBehaviour
{
    public List<Sprite> indicatorList = new List<Sprite>();
    
    public Player       aPlayer;
    public Player       bPlayer;

    public Image aPlayerHpUi;
    public Image bPlayerHpUi;
    public Image aPlayerIndicator;
    public Image bPlayerIndicator;

    private int _aPlayerFullHp;
    private int _bPlayerFullHp;

    private void Start()
    {
        _aPlayerFullHp = aPlayer.HP;
        _bPlayerFullHp = bPlayer.HP;
    }

    private void Update()
    {
        aPlayerHpUi.fillAmount = ((float)aPlayer.HP / _aPlayerFullHp);
        bPlayerHpUi.fillAmount = ((float)bPlayer.HP / _bPlayerFullHp);
        ShowIndicator();
    }

    private void ShowIndicator()
    {
        float aStep  = (float)_aPlayerFullHp / indicatorList.Count;
        int aCur = (int)Mathf.Clamp(((float) aPlayer.HP / aStep),0,indicatorList.Count-1);
        aPlayerIndicator.sprite = indicatorList[aCur];
        
        float bStep = (float)_bPlayerFullHp / indicatorList.Count;
        int   bCur  = (int)Mathf.Clamp(((float) bPlayer.HP / bStep), 0, indicatorList.Count -1);
        bPlayerIndicator.sprite = indicatorList[bCur];

        
    }
}
