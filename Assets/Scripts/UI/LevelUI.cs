using Saver;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelUI : MonoBehaviour
    {
        public Button levelButton;
        public Image  cloud;
        public int    levelIndex;

        [SerializeField] private bool _isRevealed;

        public bool IsRevealed
        {
            get => _isRevealed;
            set
            {
                _isRevealed = value;
                SetCloud();
            }
        }

        private void Start()
        {
            IsRevealed = DataTransfer.GetDataTransfer.levelRevealedList[levelIndex];
            SetCloud();
        }

        [ContextMenu("SetCloud")]
        public void SetCloud()
        {
            levelButton.interactable = IsRevealed;
            cloud?.gameObject.SetActive(!IsRevealed);
        }
    }
}