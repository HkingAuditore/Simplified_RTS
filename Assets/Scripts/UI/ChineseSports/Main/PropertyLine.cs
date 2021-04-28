using System;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.ChineseSports.Main
{
    public enum PropertyType
    {
        /// <summary>
        ///     体力值
        /// </summary>
        Hp,

        /// <summary>
        ///     攻击力
        /// </summary>
        Attack,

        /// <summary>
        ///     防御力
        /// </summary>
        Defence,

        /// <summary>
        ///     速度
        /// </summary>
        Speed,

        /// <summary>
        ///     攻击范围
        /// </summary>
        AttackRange,

        /// <summary>
        ///     消耗
        /// </summary>
        Cost
    }

    public class PropertyLine : MonoBehaviour
    {
        public                                        Text          propertyName;
        public                                        Text          propertyContent;
        [FormerlySerializedAs("PropertyType")] public PropertyType  propertyType;
        private                                       IMilitaryUnit _militaryUnit;

        private string _nameString;

        public IMilitaryUnit MilitaryUnit
        {
            get => _militaryUnit;
            set
            {
                _militaryUnit = value;
                SetPropertyString();
            }
        }

        private void Start()
        {
            Set();
        }

        [ContextMenu("Set")]
        public void Set()
        {
#if UNITY_EDITOR
            gameObject.name = propertyType.ToString();
#endif
            switch (propertyType)
            {
                case PropertyType.Hp:
                    _nameString = "体力";
                    break;
                case PropertyType.Attack:
                    _nameString = "攻击力";
                    break;
                case PropertyType.Defence:
                    _nameString = "防御力";
                    break;
                case PropertyType.Speed:
                    _nameString = "速度";
                    break;
                case PropertyType.AttackRange:
                    _nameString = "攻击范围";
                    break;
                case PropertyType.Cost:
                    _nameString = "价格";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            propertyName.text = _nameString;
            SetPropertyString();
        }

        public void SetPropertyString()
        {
            switch (propertyType)
            {
                case PropertyType.Hp:
                    propertyContent.text = MilitaryUnit.GetUnit().HP.ToString();
                    break;
                case PropertyType.Attack:
                    propertyContent.text = MilitaryUnit.AttackValue.ToString();
                    break;
                case PropertyType.Defence:
                    propertyContent.text = MilitaryUnit.DefenceValue.ToString();
                    break;
                case PropertyType.Speed:
                    propertyContent.text = MilitaryUnit.SpeedValue.ToString("f2");
                    break;
                case PropertyType.AttackRange:
                    propertyContent.text = MilitaryUnit.AttackRange.ToString("f2");
                    break;
                case PropertyType.Cost:
                    propertyContent.text = MilitaryUnit.GetUnit().costGold.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}