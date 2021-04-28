using System.Collections;
using UnityEngine;

namespace Units
{
    /// <summary>
    ///     提升效果
    /// </summary>
    [RequireComponent(typeof(Unit))]
    public class Buff : MonoBehaviour
    {
        /// <summary>
        ///     Buff编号
        /// </summary>
        public int buffId;

        /// <summary>
        ///     Buff名称
        /// </summary>
        public string buffName;

        /// <summary>
        ///     攻击力增益
        /// </summary>
        public float attackBuff = 1f;

        /// <summary>
        ///     防御力增益
        /// </summary>
        public float defenceBuff = 1f;

        /// <summary>
        ///     速度增益
        /// </summary>
        public float speedBuff = 1f;

        /// <summary>
        ///     攻击范围增益
        /// </summary>
        public float attackRangeBuff = 1f;

        /// <summary>
        ///     冷却时间增益
        /// </summary>
        public float attackColdDownTimeBuff = 1f;

        /// <summary>
        ///     剩持续时间
        /// </summary>
        public float remainTime;

        /// <summary>
        ///     Buff附加的单位
        /// </summary>
        public Unit buffUnit;

        private bool          _isMilitaryUnit = true;
        public  IMilitaryUnit buffMilitaryUnit;

        private void Start()
        {
            try
            {
                buffUnit         = GetComponent<Unit>();
                buffMilitaryUnit = GetComponent<IMilitaryUnit>();
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
                buffMilitaryUnit.AttackValue        = (int) (buffMilitaryUnit.AttackValue        * attackBuff);
                buffMilitaryUnit.DefenceValue       = (int) (buffMilitaryUnit.DefenceValue       * defenceBuff);
                buffMilitaryUnit.SpeedValue         = (int) (buffMilitaryUnit.SpeedValue         * speedBuff);
                buffMilitaryUnit.AttackColdDownTime = (int) (buffMilitaryUnit.AttackColdDownTime * attackColdDownTimeBuff);
                buffMilitaryUnit.AttackRange        = (int) (buffMilitaryUnit.AttackRange        * attackRangeBuff);
            }
            else
            {
                buffUnit.defence = (int) (buffUnit.defence * defenceBuff);
                buffUnit.Speed   = (int) (buffUnit.Speed   * speedBuff);
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
                buffMilitaryUnit.AttackValue        = (int) (buffMilitaryUnit.AttackValue        / attackBuff);
                buffMilitaryUnit.DefenceValue       = (int) (buffMilitaryUnit.DefenceValue       / defenceBuff);
                buffMilitaryUnit.SpeedValue         = (int) (buffMilitaryUnit.SpeedValue         / speedBuff);
                buffMilitaryUnit.AttackColdDownTime = (int) (buffMilitaryUnit.AttackColdDownTime / attackColdDownTimeBuff);
                buffMilitaryUnit.AttackRange        = (int) (buffMilitaryUnit.AttackRange        / attackRangeBuff);
            }
            else
            {
                buffUnit.defence = (int) (buffUnit.defence / defenceBuff);
                buffUnit.Speed   = (int) (buffUnit.Speed   / speedBuff);
            }

            Debug.Log("Out Buff");
            Destroy(this);
        }
    }
}