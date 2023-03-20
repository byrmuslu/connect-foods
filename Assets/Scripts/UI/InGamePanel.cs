namespace ConnectFoods.UI
{
    using ConnectFoods.Data;
    using ConnectFoods.Manager;
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class InGamePanel : BaseObject, IPoolable
    {
        [SerializeField] private TextMeshProUGUI _txtTotalMoves;
        [SerializeField] private TextMeshProUGUI _txtScores;
        [SerializeField] private RectTransform _goalsContentParent;
        [SerializeField] private Button _btnBackToHome;

        private BoardManager _boardManager;

        private Dictionary<string, GoalViewItem> _goalViewItems = new Dictionary<string, GoalViewItem>();

        public event IPoolable.OnAddPool AddPool;

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
            _btnBackToHome.onClick.AddListener(OnBtnBackToHomeClicked);
        }

        private void UnRegistration()
        {
            _btnBackToHome.onClick.RemoveListener(OnBtnBackToHomeClicked);
        }

        private void OnBtnBackToHomeClicked()
        {
            _boardManager.LevelFail();
        }

        public void SetBoardManager(BoardManager boardManager)
        {
            _boardManager = boardManager;

            _boardManager.UserMoveCountChanged -= OnUserMoved;
            _boardManager.GoalEffected -= OnGoalEffected;
            _boardManager.TotalScoreChanged -= OnTotalScoreChanged;
            _boardManager.LevelCompleted -= OnLevelCompleted;
            _boardManager.LevelFailed -= OnLevelFailed;

            _boardManager.UserMoveCountChanged += OnUserMoved;
            _boardManager.GoalEffected += OnGoalEffected;
            _boardManager.TotalScoreChanged += OnTotalScoreChanged;
            _boardManager.LevelCompleted += OnLevelCompleted;
            _boardManager.LevelFailed += OnLevelFailed;
        }

        protected override void OnDisable()
        {
            if (_boardManager)
            {
                _boardManager.UserMoveCountChanged -= OnUserMoved;
                _boardManager.GoalEffected -= OnGoalEffected;
                _boardManager.TotalScoreChanged -= OnTotalScoreChanged;
                _boardManager.LevelCompleted -= OnLevelCompleted;
                _boardManager.LevelFailed -= OnLevelFailed;
            }

            base.OnDisable();
        }

        private void OnLevelFailed()
        {
            DeActivate();
        }

        private void OnLevelCompleted(int totalScore)
        {
            DeActivate();
        }

        private void OnTotalScoreChanged(int totalScore)
        {
            _txtScores.text = totalScore.ToString();
        }

        private void OnGoalEffected(string goalName, int remainCount)
        {
            if (_goalViewItems.ContainsKey(goalName))
                _goalViewItems[goalName].SetRemainCount(remainCount);
        }

        private void OnUserMoved(int remainCount)
        {
            _txtTotalMoves.text = remainCount.ToString();
        }

        public void SetLevelData(LevelData levelData)
        {
            if (_goalViewItems.Count > 0)
            {
                foreach (GoalViewItem item in _goalViewItems.Values)
                    item.DeActivate();
                _goalViewItems.Clear();
            }

            ResourcesManager resourcesManager = ResourcesManager.Instance;

            levelData.targets.ForEach(t =>
            {
                GoalViewItem goalViewItem = resourcesManager.GetObject<GoalViewItem>(_goalsContentParent, t.name);
                goalViewItem.SetRemainCount(t.count);
                if (!_goalViewItems.ContainsKey(t.name))
                    _goalViewItems.Add(t.name, goalViewItem);
                else
                    _goalViewItems[t.name] = goalViewItem;
            });
            _txtTotalMoves.text = levelData.totalMove.ToString();
            _txtScores.text = "0";
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }
    }
}