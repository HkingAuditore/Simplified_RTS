using System.Collections;
using System.Collections.Generic;
using Player;
using Units;
using UnityEngine;
using UnityEngine.UI;

public class DispatchUnitChineseUI : MonoBehaviour
{
    public Text                  unitNumberText;
    public Button                unitRemoveButton;
    public GameObject            unitSetIndicator;
    public Unit                  unit;
    public int                   unitNumber;
    public Player.Player         player;
    public UnitDispatchManagerUI managerUI;

    private          Text        _foodWoodRequiredText;
    private          Text        _goldRequiredText;
    private readonly Stack<bool> _unitSelectStack = new Stack<bool>();

    public int UnitDispatchNumber { get; set; }

    private void Awake()
    {
        _foodWoodRequiredText = transform.Find("ResourceRequired").Find("FoodWood").GetComponent<Text>();
        _goldRequiredText     = transform.Find("ResourceRequired").Find("Gold").GetComponent<Text>();

        _foodWoodRequiredText.text = unit.costFood + " 食物 " + unit.costWood + " 木材";
        _goldRequiredText.text     = unit.costGold + " 黄金";
    }

    public void AddClick(bool isFoodAndWood)
    {
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
            player.ChangeResource(GameResourceType.Food, unit.costFood);
            player.ChangeResource(GameResourceType.Wood, unit.costWood);
        }
        else
        {
            player.ChangeResource(GameResourceType.Gold, unit.costGold);
        }

        if (UnitDispatchNumber <= 0)
        {
            unitNumberText.gameObject.SetActive(false);
            unitSetIndicator.gameObject.SetActive(false);
            unitRemoveButton.gameObject.SetActive(false);
        }
    }
}