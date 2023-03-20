namespace ConnectFoods.Game
{
    using DG.Tweening;
    using UnityEngine;

    public class Item : BaseObject, IPoolable
    {
        [SerializeField] private SpriteRenderer _selectionRenderer;
        [SerializeField] private Sprite _selectedSprite;
        private Sprite _defaultSprite;

        [SerializeField] private string _name;
        public override string VariantKey => _name;

        public Vector2 Position { get => transform.position; set => transform.position = value; }

        public event IPoolable.OnAddPool AddPool;

        protected override void Awake()
        {
            _defaultSprite = _selectionRenderer.sprite;
            base.Awake();
        }

        public void Move(Vector2 target, float duration = 1)
        {
            transform.DOMove(target, duration);
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }

        public void SetSelected(bool isSelected)
        {
            _selectionRenderer.sprite = isSelected ? _selectedSprite : _defaultSprite;
        }
    }
}