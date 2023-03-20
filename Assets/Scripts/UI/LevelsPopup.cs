namespace ConnectFoods.UI
{
    using ConnectFoods.Data;
    using ConnectFoods.Manager;
    using System.Collections.Generic;
    using UnityEngine;

    public class LevelsPopup : BaseObject
    {
        [SerializeField] private RectTransform _contentParent;

        public delegate void OnLevelSelected(LevelData levelData);
        public event OnLevelSelected LevelSelected;

        protected override void Awake()
        {
            base.Awake();

            InitPopup();
        }

        private void InitPopup()
        {
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            List<LevelData> levels = resourcesManager.GetScriptableObjects<LevelData>();

            levels.Sort((l1, l2) =>
            {
                if (l1.level < l2.level)
                    return -1;
                else if (l1.level > l2.level)
                    return 1;
                else
                    return 0;
            });

            for(int i = 0; i < levels.Count; i++)
            {
                LevelSelectionItem item = resourcesManager.GetObject<LevelSelectionItem>(_contentParent);
                item.SetItem(levels[i], i == 0 || levels[i-1].isCompleted);

                item.Selected += OnLevelItemSelected;
            }
        }

        private void OnLevelItemSelected(LevelData levelData)
        {
            LevelSelected?.Invoke(levelData);
            DeActivate();
        }

        public override void DeActivate()
        {
            Destroy(gameObject);
        }
    }
}