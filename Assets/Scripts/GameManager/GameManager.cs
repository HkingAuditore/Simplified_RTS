using System.Collections.Generic;
using Pathfinding;
using Saver;
using UI.ChineseSports.Battle;
using Units;
using UnityEngine;
using UnityEngine.Events;

namespace GameManager
{
    /// <summary>
    ///     关卡全局管理控制器
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        ///     我方
        /// </summary>
        public Player.Player aSide;

        /// <summary>
        ///     敌人
        /// </summary>
        public Player.Player bSide;

        /// <summary>
        ///     寻路管理
        /// </summary>
        public Seeker seeker;

        /// <summary>
        ///     主相机
        /// </summary>
        public Camera mainCamera;

        /// <summary>
        ///     单位列表
        /// </summary>
        public List<Unit> unitsList = new List<Unit>();

        /// <summary>
        ///     结果显示UI
        /// </summary>
        public ResultUI resultUI;

        /// <summary>
        ///     胜利事件
        /// </summary>
        public UnityEvent winEvent = new UnityEvent();

        /// <summary>
        ///     失败事件
        /// </summary>
        public UnityEvent loseEvent = new UnityEvent();

        /// <summary>
        ///     胜利后解锁的关卡
        /// </summary>
        public int unlockLevel;

        /// <summary>
        ///     胜利后解锁的角色
        /// </summary>
        public int unlockCharacter;

        /// <summary>
        ///     胜利后解锁的物体
        /// </summary>
        public int[] unlockItem;

        /// <summary>
        ///     BGM音源
        /// </summary>
        public AudioSource audioSource;

        private       bool        _isEventHasBeenLaunched;
        public static GameManager GetManager { get; private set; }

        private void Awake()
        {
            GetManager = this;
        }

        private void Start()
        {
            if (DataTransfer.GetDataTransfer.isSoundsActive)
                audioSource.Play();
            else
                audioSource.Stop();
            winEvent.AddListener(() =>
                                 {
                                     if (unlockLevel != null) DataTransfer.GetDataTransfer.levelRevealedList[unlockLevel] = true;
                                 });
            winEvent.AddListener(() =>
                                 {
                                     if (unlockItem != null)
                                         foreach (var i in unlockItem)
                                             DataTransfer.GetDataTransfer.itemRevealedList[i] = true;
                                 });
            winEvent.AddListener(() => { DataTransfer.GetDataTransfer.characterRevealedList[unlockCharacter] = true; });
        }

        /// <summary>
        ///     我方胜利
        /// </summary>
        public void PlayerWin()
        {
            if (!_isEventHasBeenLaunched)
            {
                resultUI.isWin = true;
                resultUI.ShowResult();
                winEvent.Invoke();
                DataTransfer.GetDataTransfer.xmlSaver.SaveData();
                _isEventHasBeenLaunched = true;
            }
        }

        /// <summary>
        ///     我方失败
        /// </summary>
        public void PlayerLose()
        {
            if (!_isEventHasBeenLaunched)
            {
                resultUI.isWin = false;
                resultUI.ShowResult();
                loseEvent.Invoke();
                _isEventHasBeenLaunched = true;
            }
        }
    }
}