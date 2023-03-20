namespace ConnectFoods.Manager
{
    using ConnectFoods.Data;
    using ConnectFoods.Game;
    using ConnectFoods.UI;
    using UnityEngine;

    public class GameManager : BaseObject
    {
        private ResourcesManager _resourcesManager;

        private BoardManager _boardManager;

        private static GameManager _instance;

        protected override void Awake()
        {
            if (_instance)
            {
                Debug.LogError("GameManager must be one!!");
                Destroy(gameObject);
                return;
            }
            base.Awake();

            _resourcesManager = ResourcesManager.Instance;
            InitCanvas();
            InitBoardManager();
            InitHomePanel();

            _instance = this;
        }

        protected override void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
            base.OnDestroy();
        }

        private void InitCanvas()
        {
            _resourcesManager.GetObject<BaseCanvas>();
        }

        private void InitBoardManager()
        {
            _boardManager = _resourcesManager.GetObject<BoardManager>();

            _boardManager.LevelCompleted += OnLevelCompleted;
            _boardManager.LevelFailed += OnLevelFailed;
        }

        private void OnLevelCompleted(int totalScore)
        {
            _boardManager.CurrentLevelData.isCompleted = true;

            if (totalScore > _boardManager.CurrentLevelData.highScore)
            {
                _boardManager.CurrentLevelData.highScore = totalScore;
                Effect effect = _resourcesManager.GetObject<Effect>("confetti");
                effect.PlayEffect(() =>
                {
                    HomePanel homePanel = _resourcesManager.GetObject<HomePanel>(BaseCanvas.Instance.transform);
                    homePanel.ShowLevelsPopup();
                    _boardManager.ClearBoard();
                });
            }
            else
            {
                HomePanel homePanel = _resourcesManager.GetObject<HomePanel>(BaseCanvas.Instance.transform);
                homePanel.ShowLevelsPopup();
            }
        }

        private void OnLevelFailed()
        {
            _resourcesManager.GetObject<HomePanel>(BaseCanvas.Instance.transform);
        }

        private void InitHomePanel()
        {
            if (!_boardManager)
            {
                Debug.LogError("BoardManager initialized before homePanel!!");
                return;
            }

            HomePanel homePanel = _resourcesManager.GetObject<HomePanel>(BaseCanvas.Instance.transform);
            homePanel.LevelSelected += OnLevelSelected;
        }

        private void OnLevelSelected(LevelData levelData)
        {
            StartGame(levelData);
        }

        private void StartGame(LevelData levelData)
        {
            _boardManager.SetBoard(levelData);

            Camera.main.orthographicSize = Mathf.Clamp(((levelData.columnCount - 2) * 2), 6, 20);

            InGamePanel inGamePanel = _resourcesManager.GetObject<InGamePanel>(BaseCanvas.Instance.transform);
            inGamePanel.SetBoardManager(_boardManager);
            inGamePanel.SetLevelData(levelData);
        }
    }
}