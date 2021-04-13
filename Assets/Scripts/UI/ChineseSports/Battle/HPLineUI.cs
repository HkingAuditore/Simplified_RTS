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
    public Image aPlayerFlag;
    public Image bPlayerFlag;

    private int _aPlayerFullHp;
    private int _bPlayerFullHp;

    private Vector2 _leftPos;
    private Vector2 _midPos;
    private Vector2 _rightPos;

    private void Start()
    {
        _aPlayerFullHp = aPlayer.HP;
        _bPlayerFullHp = bPlayer.HP;

        RectTransform panelRect = this.transform.Find("BackGround").GetComponent<RectTransform>();
        _midPos   = panelRect.anchoredPosition + new Vector2( 0,                           -4.799988f);
        _leftPos  = _midPos                    - new Vector2( .5f * panelRect.sizeDelta.x, 0);
        _rightPos = _midPos                    + new Vector2( .5f * panelRect.sizeDelta.x, 0);
    }

    private void Update()
    {
        aPlayerHpUi.fillAmount = ((float)aPlayer.HP / _aPlayerFullHp);
        bPlayerHpUi.fillAmount = ((float)bPlayer.HP / _bPlayerFullHp);
        // ShowIndicator();
        ShowFlagAndIndicator();
    }

    private void ShowIndicator()
    {
        float aStep  = (float)_aPlayerFullHp / indicatorList.Count;
        int aCur = (int)Mathf.Clamp(((float) aPlayer.HP / aStep),0,indicatorList.Count-1);
        aPlayerIndicator.sprite                         = indicatorList[aCur];
        aPlayerIndicator.rectTransform.anchoredPosition = Vector2.Lerp(_leftPos, _midPos, aPlayerHpUi.fillAmount);
        
        float bStep = (float)_bPlayerFullHp / indicatorList.Count;
        int   bCur  = (int)Mathf.Clamp(((float) bPlayer.HP / bStep), 0, indicatorList.Count -1);
        bPlayerIndicator.sprite                         = indicatorList[bCur];
        bPlayerIndicator.rectTransform.anchoredPosition = Vector2.Lerp(_rightPos, _midPos,bPlayerHpUi.fillAmount);
        
    }
    private void ShowFlagAndIndicator()
    {
        float aStep  = (float)_aPlayerFullHp / indicatorList.Count;
        int aCur = (int)Mathf.Clamp(((float) aPlayer.HP / aStep),0,indicatorList.Count-1);
        aPlayerIndicator.sprite                    = indicatorList[aCur];
        aPlayerFlag.rectTransform.anchoredPosition = Vector2.Lerp(_midPos, _leftPos, aPlayerHpUi.fillAmount);
        
        float bStep = (float)_bPlayerFullHp / indicatorList.Count;
        int   bCur  = (int)Mathf.Clamp(((float) bPlayer.HP / bStep), 0, indicatorList.Count -1);
        bPlayerIndicator.sprite                    = indicatorList[bCur];
        bPlayerFlag.rectTransform.anchoredPosition = Vector2.Lerp(_midPos, _rightPos, bPlayerHpUi.fillAmount);
        
    }
}
