namespace ConnectFoods.UI
{
    using ConnectFoods.Data;
    using ConnectFoods.Manager;
    using UnityEngine;
    using UnityEngine.UI;

    public class HomePanel : BaseObject, IPoolable
    {
        [SerializeField] private Button _btnLevels;
        
        public event IPoolable.OnAddPool AddPool;
        public delegate void OnLevelSelect(LevelData levelData);
        public event OnLevelSelect LevelSelected;

        private LevelsPopup _levelsPopup;

        protected override void Awake()
        {
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
            _btnLevels.onClick.AddListener(OnBtnLevelsClicked);
        }

        private void UnRegistration()
        {
            _btnLevels.onClick.RemoveListener(OnBtnLevelsClicked);
        }

        private void OnBtnLevelsClicked()
        {
            ShowLevelsPopup();
        }

        public void ShowLevelsPopup()
        {
            _levelsPopup?.DeActivate();

            ResourcesManager resourcesManager = ResourcesManager.Instance;

            LevelsPopup levelsPopup = resourcesManager.GetObject<LevelsPopup>(transform);

            levelsPopup.LevelSelected += OnLevelSelected;
            levelsPopup.ObjectDestroyed += (BaseObject obj) => _levelsPopup = null;

            _levelsPopup = levelsPopup;
        }

        private void OnLevelSelected(LevelData levelData)
        {
            LevelSelected?.Invoke(levelData);
            DeActivate();
        }

        public override void Activate()
        {
            _levelsPopup?.DeActivate();
            base.Activate();
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }
    }
}