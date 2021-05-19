using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using UI.ChineseSports.Battle;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Local
{
    public class UnitDispatchUI : MonoBehaviour
    {
        public          Text                  unitNumberText;
        public          Button                unitRemoveButton;
        public          GameObject            unitSetIndicator;
        public          Unit                  unit;
        public          int                   unitNumber;
        public          Player.Player         player;
        public          int                   onceMax;
        public          Text                  resourceText;
        public          UnitDispatchManagerUI dispatchManagerUI;
        public          SpriteIndicatorUI     spriteIndicatorUI;
        public readonly Stack<bool>           UnitSelectStack = new Stack<bool>();
        private         Button                _dispatchButton;


        private Text _foodWoodRequiredText;
        private Text _goldRequiredText;

        public int UnitDispatchNumber { get; set; }

        private void Awake()
        {
            // _foodWoodRequiredText = transform.Find("ResourceRequired").Find("FoodWood").GetComponent<Text>();
            // _goldRequiredText     = transform.Find("ResourceRequired").Find("Gold").GetComponent<Text>();
            //
            // _foodWoodRequiredText.text = unit.costFood + " 食物 " + unit.costWood + " 木材";
            // _goldRequiredText.text     = unit.costGold + " 黄金";
            _dispatchButton   = gameObject.GetComponent<Button>();
            resourceText.text = unit.costGold.ToString();
        }

        private void Start()
        {
            try
            {
                int enemyUnitsNum = GameManager.GameManager.GetManager.unitsList.Count(u => u.IsEnemy);
                unitNumber = GameManager.GameManager.GetManager.unitsList.FindIndex(u => u.UnitPrefab == this.unit) - enemyUnitsNum;
                if (unitNumber == -1 - enemyUnitsNum)
                {
                    this.gameObject.SetActive(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.gameObject.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            if (UnitDispatchNumber < onceMax && player.IsEnoughForUnit(unit, GameResourceType.Gold) && !dispatchManagerUI.IsInDispatching && player.CountUnits(unit.unitType) + UnitDispatchNumber < unit.playerOwnMax)
                _dispatchButton.interactable = true;
            else
                _dispatchButton.interactable = false;
        }

        public void AddClick(bool isFoodAndWood)
        {
            if (UnitDispatchNumber >= onceMax) return;

            dispatchManagerUI.IsInDispatching = true;
            try
            {
                spriteIndicatorUI.UnitSprite =
                    unit.gameObject.transform.Find("Character").GetComponent<SpriteRenderer>().sprite;
                spriteIndicatorUI.transform.localScale = unit.gameObject.transform.Find("Character").transform.localScale;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            spriteIndicatorUI.gameObject.SetActive(true);

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
            if (unitNumberText != null)
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
                spriteIndicatorUI.gameObject.SetActive(false);
            }
        }
    }
}