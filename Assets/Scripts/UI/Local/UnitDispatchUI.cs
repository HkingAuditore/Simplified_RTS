using System;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.UI;

public class UnitDispatchUI : MonoBehaviour
{
    public          Text                  unitNumberText;
    public          Button                unitRemoveButton;
    public          GameObject            unitSetIndicator;
    public          Unit                  unit;
    public          int                   unitNumber;
    public          Player                player;
    public          int                   onceMax;
    public          Text                  resourceText;
    public          UnitDispatchManagerUI dispatchManagerUI;
    public readonly Stack<bool>           UnitSelectStack = new Stack<bool>();


    private         Text        _foodWoodRequiredText;
    private         Text        _goldRequiredText;
    private         Button      _dispatchButton;

    public int UnitDispatchNumber { get; set; }

    private void Awake()
    {
        // _foodWoodRequiredText = transform.Find("ResourceRequired").Find("FoodWood").GetComponent<Text>();
        // _goldRequiredText     = transform.Find("ResourceRequired").Find("Gold").GetComponent<Text>();
        //
        // _foodWoodRequiredText.text = unit.costFood + " 食物 " + unit.costWood + " 木材";
        // _goldRequiredText.text     = unit.costGold + " 黄金";
        _dispatchButton   = this.gameObject.GetComponent<Button>();
        resourceText.text = this.unit.costGold.ToString();
    }

    public void AddClick(bool isFoodAndWood)
    {
        if (UnitDispatchNumber >= onceMax) return;
        dispatchManagerUI.IsInDispatching = true;
        // 尝试分配资源
        if (isFoodAndWood)
            try
            {
                player.ChangeResource(GameResourceType.Food, -unit.costFood);
                player.ChangeResource(GameResourceType.Wood, -unit.costWood);
            }
            catch (GameException e)
            {
                if (e.GameResourceType == GameResourceType.Wood) player.ChangeResource(GameResourceType.Food, unit.costFood);

                return;
            }
        else
            try
            {
                player.ChangeResource(GameResourceType.Gold, -unit.costGold);
            }
            catch (GameException)
            {
                return;
            }

        UnitDispatchNumber++;

        unitNumberText?.gameObject.SetActive(true);
        ShowStackNumber();

        unitSetIndicator.gameObject.SetActive(true);
        unitRemoveButton.gameObject.SetActive(true);

        UnitSelectStack.Push(isFoodAndWood);


        
    }

    private void ShowStackNumber()
    {
        if(unitNumberText!=null)
            unitNumberText.text = UnitDispatchNumber.ToString();
    }

    public void RemoveClick()
    {
        UnitDispatchNumber--;
        ShowStackNumber();

        if (UnitSelectStack.Pop())
        {
            player.ChangeResource(GameResourceType.Food, unit.costFood);
            player.ChangeResource(GameResourceType.Wood, unit.costWood);
        }
        else
        {
            player.ChangeResource(GameResourceType.Gold, unit.costGold);
        }
        dispatchManagerUI.IsInDispatching = false;

        if (UnitDispatchNumber <= 0)
        {
            unitNumberText?.gameObject.SetActive(false);
            unitSetIndicator.gameObject.SetActive(false);
            unitRemoveButton.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (UnitDispatchNumber < onceMax && player.IsEnoughForUnit(this.unit, GameResourceType.Gold) && !dispatchManagerUI.IsInDispatching && (player.CountUnits(this.unit.unitType) + UnitDispatchNumber)< this.unit.playerOwnMax)
        {
            this._dispatchButton.interactable = true;
        }
        else
        {
            this._dispatchButton.interactable = false;
        }
    }
}