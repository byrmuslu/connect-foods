namespace ConnectFoods.UI
{
    using ConnectFoods.Data;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class LevelSelectionItem : BaseObject
    {
        [SerializeField] private TextMeshProUGUI _txtLevelIndex;
        [SerializeField] private TextMeshProUGUI _txtHighscore;

        private Button _btn;

        private LevelData _levelData;

        public delegate void OnLevelSelected(LevelData levelData);
        public event OnLevelSelected Selected;

        protected override void Awake()
        {
            _btn = GetComponent<Button>();
            
            Registration();

            base.Awake();
        }

        protected override void OnDestroy()
        {
            UnRegistration();

            base.OnDestroy();
        }

        private void Registration()
        {
            _btn.onClick.AddListener(OnBtnClicked);
        }

        private void UnRegistration()
        {
            _btn.onClick.RemoveListener(OnBtnClicked);
        }

        private void OnBtnClicked()
        {
            Selected?.Invoke(_levelData);
        }

        public void SetItem(LevelData levelData, bool isPlayable)
        {
            _levelData = levelData;
            _txtLevelIndex.text = levelData.level.ToString("00");
            _txtHighscore.text = "Highscore<br>" + PlayerPrefs.GetInt("Level-" + levelData.level, 0);
            _btn.interactable = isPlayable;
        }

        public override void DeActivate()
        {
            Destroy(gameObject);
        }
    }
}