using System.Collections.Generic;
using UnityEngine;

namespace UI.ChineseSports.Main
{
    public class MainUIManager : MonoBehaviour
    {
        public GameObject      itemUI;
        public ScrollUI        scrollUI;
        public SettingUI       settingUI;
        public CharacterViewUI characterViewUI;

        [Header("Map Roll")] public RectTransform backGround;

        public float rollSpeed;
        public float bound;
        public float threshold;

        [Header("Map Cloud")] public List<LevelUI> LevelMapDicts = new List<LevelUI>();

        [Header("Scroll")] public List<string> titles           = new List<string>();
        public                    List<Sprite> titleSprites     = new List<Sprite>();
        public                    List<string> battleSceneNames = new List<string>();


        private bool _isOpenPanel;

        private void Start()
        {
            Time.timeScale = 1f;
        }

        private void Update()
        {
            if (!_isOpenPanel)
            {
                var mouseOffset = new Vector2(Input.mousePosition.x, Input.mousePosition.y) -
                                  new Vector2(Screen.width / 2f,     Screen.height / 2f);
                mouseOffset =
                    new
                        Vector2(Mathf.Sign(mouseOffset.x) * Mathf.Clamp01(Mathf.Abs(mouseOffset.x / Screen.width) - threshold),
                                mouseOffset.y             / Screen.height);
                var offset = mouseOffset.x * rollSpeed * Time.deltaTime;
                if (Mathf.Abs(mouseOffset.x) < .2f || Mathf.Abs(backGround.anchoredPosition.x - offset) > bound) return;
                // Debug.Log(mouseOffset);

                backGround.anchoredPosition =
                    new Vector2(backGround.anchoredPosition.x - offset, backGround.anchoredPosition.y);
            }
        }

        public void ShowItemUI()
        {
            itemUI.SetActive(true);
            _isOpenPanel = true;
        }

        public void CloseItemUI()
        {
            itemUI.SetActive(false);
            _isOpenPanel = false;
        }

        public void ShowCharacterViewUI()
        {
            characterViewUI.gameObject.SetActive(true);
            _isOpenPanel = true;
        }

        public void CloseCharacterViewUI()
        {
            characterViewUI.gameObject.SetActive(false);
            _isOpenPanel = false;
        }

        public void ShowScrollUI(int index)
        {
            scrollUI.sceneName   = battleSceneNames[index];
            scrollUI.TitleName   = titles[index];
            scrollUI.TitleSprite = titleSprites[index];
            scrollUI.gameObject.SetActive(true);
            _isOpenPanel = true;
        }

        public void CloseScrollUI()
        {
            scrollUI.Close();
            _isOpenPanel = false;
        }

        public void ShowSettingUI()
        {
            settingUI.gameObject.SetActive(true);
            _isOpenPanel = true;
        }

        public void CloseSettingUI()
        {
            settingUI.gameObject.SetActive(false);
            _isOpenPanel = false;
        }
    }
}