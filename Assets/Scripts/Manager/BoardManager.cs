namespace ConnectFoods.Manager
{
    using ConnectFoods.Data;
    using ConnectFoods.Game;
    using ConnectFoods.State;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [RequireComponent(typeof(LineRenderer))]
    public class BoardManager : BaseObject
    {
        [SerializeField] private float _tileOffset;

        private LineRenderer _lineRenderer;
        public List<Tile> Tiles { get; set; } = new List<Tile>();
        public List<Item> Items { get; private set; } = new List<Item>();

        private const float RAY_DISTANCE = 2f;

        public delegate void OnUserMoveRemainCountChanged(int remainMoveCount);
        public event OnUserMoveRemainCountChanged UserMoveCountChanged;
        public delegate void OnTotalScoreChanged(int totalScore);
        public event OnTotalScoreChanged TotalScoreChanged;
        public delegate void OnGoalEffected(string goalName, int remainCount);
        public event OnGoalEffected GoalEffected;

        public event Action LevelFailed;
        public delegate void OnLevelComplete(int totalScore);
        public event OnLevelComplete LevelCompleted;

        private int _remainMoveCount;
        private int _totalScore;

        public LevelData CurrentLevelData { get; private set; }

        private readonly Dictionary<string, int> _goalsRemainCount = new Dictionary<string, int>();

        public Tile SelectedTile { get; set; }

        private IState _currentState;

        protected override void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            base.Awake();
        }

        public void SetState(IState state)
        {
            _currentState = state;
            StopAllCoroutines();
            StartCoroutine(state.Action());
        }

        public void LevelFail()
        {
            LevelFailed?.Invoke();
            StopAllCoroutines();
            ClearBoard();
        }

        public void LevelComplete()
        {
            LevelCompleted?.Invoke(_totalScore);
            StopAllCoroutines();
        }

        public void FillTile(Tile tile)
        {
            Tile upTile = Ray(tile, Vector2.up);
            if (!upTile)
            {
                Vector2 position = tile.transform.position;
                position.y = 10;
                Item newItem = ResourcesManager.Instance.GetRandomObjectInVariants<Item>(transform);
                newItem.Position = position;
                tile.Item = newItem;
                newItem.Move(tile.Position, .5f);
                Items.Add(newItem);
                return;
            }
            if (!upTile.IsEmpty)
            {
                upTile.Item.Move(tile.Position, .5f);
                tile.Item = upTile.Item;
                upTile.Item = null;
                FillTile(upTile);
            }
        }

        public void SetBoard(LevelData levelData)
        {
            ClearBoard();

            CurrentLevelData = levelData;

            levelData.targets.ForEach(t =>
            {
                _goalsRemainCount.Add(t.name, t.count);
            });

            _remainMoveCount = levelData.totalMove;

            StopAllCoroutines();
            
            InitBoard(levelData.rowCount, levelData.columnCount, _tileOffset);

            SetState(new EmptyTilesFillState(this));
        }

        private void InitBoard(int rowCount, int columnCount, float tileOffset)
        {
            Vector2 originPos = transform.position;
            originPos.x -= (((columnCount - 1) * tileOffset) / 2);
            originPos.y -= (((rowCount - 1) * tileOffset) / 2);

            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    Vector2 pos = new Vector2(i * tileOffset, j * tileOffset) + originPos;
                    Tile newTile = CreateNewTile(pos);
                    Tiles.Add(newTile);
                }
            }
        }

        public void ClearBoard()
        {
            _totalScore = 0;
            SelectedTile = null;
            _goalsRemainCount.Clear();

            Items.ForEach(i => i.DeActivate());
            Items.Clear();

            Tiles.ForEach(t => t.DeActivate());
            Tiles.Clear();
        }

        private void DecreaseUserMoveCount()
        {
            _remainMoveCount = Mathf.Clamp(_remainMoveCount - 1, 0, 999);
            UserMoveCountChanged?.Invoke(_remainMoveCount);

            if (_remainMoveCount == 0)
                LevelFail();
        }

        private void TakeScore(int amount)
        {
            _totalScore += amount;
            TotalScoreChanged?.Invoke(_totalScore);
        }

        private Tile CreateNewTile(Vector2 pos)
        {
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            Tile newTile = resourcesManager.GetObject<Tile>(transform);
            newTile.Item = null;
            newTile.SetSelected(false);
            newTile.Position = pos;

            newTile.PointerDown -= OnTilePointerDown;
            newTile.PointerUp -= OnTilePointerUp;
            newTile.PointerEnter -= OnTilePointerEnter;
            newTile.Exploded -= OnTileExploded;

            newTile.PointerDown += OnTilePointerDown;
            newTile.PointerUp += OnTilePointerUp;
            newTile.PointerEnter += OnTilePointerEnter;
            newTile.Exploded += OnTileExploded;

            return newTile;
        }

        private void DecreaseGoalCount(string goalName, int amount)
        {
            if (!_goalsRemainCount.ContainsKey(goalName))
                return;
            _goalsRemainCount[goalName] = Mathf.Clamp(_goalsRemainCount[goalName] - amount, 0, 999);
            GoalEffected?.Invoke(goalName, _goalsRemainCount[goalName]);
            if (!_goalsRemainCount.Values.Any(c => c > 0))
                LevelComplete();
        }

        private void OnTilePointerDown(Tile tile)
        {
            if (_remainMoveCount <= 0)
                return;
            tile.SetSelected(true);
            SelectedTile = tile;
            _lineRenderer.positionCount = 1;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, tile.Position);
        }

        private void OnTilePointerUp(Tile tile)
        {
            if (_remainMoveCount <= 0)
                return;

            if (SelectedTile)
            {
                List<Tile> neighbors = FindNeighbors(tile);
                if (neighbors.Count(n => n.IsSelected) > 2)
                {
                    if (_goalsRemainCount.ContainsKey(tile.Item.VariantKey))
                        DecreaseGoalCount(tile.Item.VariantKey, neighbors.Count(n => n.IsSelected));

                    neighbors.ForEach(n =>
                    {
                        if (n.IsSelected)
                            n.Explode();
                    });

                    DecreaseUserMoveCount();
                    
                    TakeScore(neighbors.Count);

                }
                else
                {
                    neighbors.ForEach(n => n.SetSelected(false));
                }
            }

            SelectedTile = null;
            tile.SetSelected(false);
            _lineRenderer.positionCount = 0;
        }

        private void OnTilePointerEnter(Tile tile)
        {
            if (_remainMoveCount <= 0 || !SelectedTile || tile.IsSelected)
                return;

            List<Tile> neighbors = FindNearestNeighbors(tile);
            if (neighbors.Contains(SelectedTile))
            {
                SelectedTile = tile;
                tile.SetSelected(true);
                _lineRenderer.positionCount++;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, tile.Position);
            }
        }

        private void OnTileExploded(Tile tile)
        {
            CreateBrokenTiles(6, tile.Position);

            Items.Remove(tile.Item);
        }

        private void CreateBrokenTiles(int count, Vector2 originPos)
        {
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            for (int i = 0; i < count; i++)
            {
                BrokenTile newBrokenTile = resourcesManager.GetRandomObjectInVariants<BrokenTile>(transform);
                newBrokenTile.Position = originPos;
                Vector2 randomPos = Random.insideUnitCircle * 2f + originPos;
                newBrokenTile.MoveAndDestroy(randomPos);
            }
        }

        public List<Tile> FindNeighbors(Tile tile)
        {
            return FindNeighbors(tile, tile, new());
        }

        private List<Tile> FindNeighbors(Tile original, Tile element, HashSet<Tile> neighbors)
        {
            if (element is null || neighbors.Contains(element) || !original.Equals(element))
            {
                return new();
            }

            neighbors.Add(element);

            List<Tile> localNeighbors = new() { element };

            localNeighbors.AddRange(FindNeighbors(original, Ray(element, Vector2.up), neighbors));
            localNeighbors.AddRange(FindNeighbors(original, Ray(element, Vector2.down), neighbors));
            localNeighbors.AddRange(FindNeighbors(original, Ray(element, Vector2.right), neighbors));
            localNeighbors.AddRange(FindNeighbors(original, Ray(element, Vector2.left), neighbors));
            localNeighbors.AddRange(FindNeighbors(original, Ray(element, Vector2.one), neighbors));
            localNeighbors.AddRange(FindNeighbors(original, Ray(element, Vector2.one * -1), neighbors));
            localNeighbors.AddRange(FindNeighbors(original, Ray(element, new Vector2(-1, 1)), neighbors));
            localNeighbors.AddRange(FindNeighbors(original, Ray(element, new Vector2(1, -1)), neighbors));

            return localNeighbors;
        }

        private List<Tile> FindNearestNeighbors(Tile tile)
        {
            List<Tile> neighbors = new List<Tile>();

            List<Vector2> directions = new List<Vector2>()
            {
                Vector2.up,
                Vector2.down,
                Vector2.left,
                Vector2.right,
                Vector2.one,
                Vector2.one * -1,
                new Vector2(1,-1),
                new Vector2(-1,1)
            };

            directions.ForEach(d =>
            {
                Tile neighbor = RayToEqualityTile(tile, d);
                if (neighbor != null)
                    neighbors.Add(neighbor);
            });

            return neighbors;
        }

        private Tile RayToEqualityTile(Tile t, Vector2 direction)
        {
            t.Collider.enabled = false;
            var hit = Physics2D.Raycast(t.Position, direction, RAY_DISTANCE, LayerMask.GetMask("Tile"));
            t.Collider.enabled = true;

            if (hit.collider && hit.collider.TryGetComponent<Tile>(out Tile tile) && tile.Equals(t))
                return tile;
            return null;
        }

        public Tile Ray(Tile t, Vector2 direction)
        {
            t.Collider.enabled = false;
            var hit = Physics2D.Raycast(t.Position, direction, RAY_DISTANCE, LayerMask.GetMask("Tile"));
            t.Collider.enabled = true;

            return hit.collider?.GetComponent<Tile>();
        }
    }
}