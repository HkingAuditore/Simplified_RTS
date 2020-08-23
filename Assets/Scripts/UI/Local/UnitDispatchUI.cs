using System;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.UI;

public class UnitDispatchUI : MonoBehaviour
{
    public Text unitNumberText;
    public Button unitRemoveButton;
    public GameObject unitSetIndicator;
    public Unit unit;
    public Player player;
    public UnitDIspatchManagerUI managerUI;

    public int UnitDispatchNumber { get; set; } = 0;
    private Stack<bool> _unitSelectStack = new Stack<bool>();

    public void AddClick(bool isFoodAndWood)
    {
        // 尝试分配资源
        if (isFoodAndWood)
        {
            try
            {
                this.player.ChangeResource(Resource.Food,-unit.costFood);
                this.player.ChangeResource(Resource.Wood,-unit.costWood);
                
            }
            catch (ResourceRunOutException e)
            {
                if (e.resource == Resource.Wood)
                {
                    this.player.ChangeResource(Resource.Food,unit.costFood);
                }

                return;
            }

        }
        else
        {
            try
            {
                this.player.ChangeResource(Resource.Gold,-unit.costGold);
                
            }
            catch (ResourceRunOutException)
            {
                return;
            }
        }
        UnitDispatchNumber++;

        unitNumberText.gameObject.SetActive(true);
        unitNumberText.text = UnitDispatchNumber.ToString();
        unitSetIndicator.gameObject.SetActive(true);
        unitRemoveButton.gameObject.SetActive(true);
        
        _unitSelectStack.Push(isFoodAndWood);
    }

    public void RemoveClick()
    {
        UnitDispatchNumber--;
        unitNumberText.text = UnitDispatchNumber.ToString();

        if (_unitSelectStack.Pop())
        {
            this.player.ChangeResource(Resource.Food,unit.costFood);
            this.player.ChangeResource(Resource.Wood,unit.costWood);
        }
        else
        {
            this.player.ChangeResource(Resource.Gold,unit.costGold);
        }
        
        if (UnitDispatchNumber <= 0)
        {
            unitNumberText.gameObject.SetActive(false);
            unitSetIndicator.gameObject.SetActive(false);
            unitRemoveButton.gameObject.SetActive(false);
        }

    }

}
