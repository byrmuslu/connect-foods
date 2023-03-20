namespace ConnectFoods.Game
{
    using DG.Tweening;
    using UnityEngine;

    public class BrokenTile : BaseObject, IPoolable
    {
        [SerializeField] private string _type;
        public override string VariantKey => _type;
        public Vector2 Position { get => transform.position; set => transform.position = value; }

        public event IPoolable.OnAddPool AddPool;

        public void MoveAndDestroy(Vector2 target, float duration = 1f)
        {
            transform.DOMove(target, duration).OnComplete(DeActivate);
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }
    }
}