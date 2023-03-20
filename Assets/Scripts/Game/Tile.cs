namespace ConnectFoods.Game
{
    using DG.Tweening;
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Collider2D))]
    public class Tile : BaseObject, IPoolable
    {
        private Collider2D _collider;
        public Collider2D Collider 
        {
            get
            {
                if(!_collider)
                    _collider = GetComponent<Collider2D>();
                return _collider;
            }
        }

        public Vector2 Position { get => transform.position; set => transform.position = value; }
        public Item Item { get; set; }
        public bool IsEmpty => Item == null;
        public bool IsSelected { get; private set; }

        public event Action<Tile> PointerDown;
        public event Action<Tile> Dragging;
        public event Action<Tile> PointerUp;
        public event Action<Tile> PointerEnter;
        public event Action<Tile> PointerExit;

        public event Action<Tile> Exploded;
        public event IPoolable.OnAddPool AddPool;

        private void OnMouseDown()
        {
            PointerDown?.Invoke(this);
        }

        private void OnMouseDrag()
        {
            Dragging?.Invoke(this);
        }

        private void OnMouseUp()
        {
            PointerUp?.Invoke(this);
        }

        private void OnMouseEnter()
        {
            PointerEnter?.Invoke(this);
        }

        private void OnMouseExit()
        {
            PointerExit?.Invoke(this);
        }

        public void SetSelected(bool isSelected)
        {
            IsSelected = isSelected;
            Item?.SetSelected(isSelected);
        }

        public void Move(Vector2 target, float duration = 1f)
        {
            transform.DOMove(target, duration);
        }

        public void Explode()
        {
            Item?.DeActivate();
            Exploded?.Invoke(this);
            SetSelected(false);
            Item = null;
        }

        public bool Equals(Tile other)
        {
            return !IsEmpty && !other.IsEmpty && Item.VariantKey.Equals(other.Item.VariantKey);
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }
    }
}