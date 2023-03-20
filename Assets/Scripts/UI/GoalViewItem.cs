namespace ConnectFoods.UI
{
    using TMPro;
    using UnityEngine;

    public class GoalViewItem : BaseObject, IPoolable
    {
        [SerializeField] private string _name;
        public override string VariantKey => _name;

        [SerializeField] private TextMeshProUGUI _txtRemainCount;

        public event IPoolable.OnAddPool AddPool;

        public int RemainCount { get; private set; }

        public void SetRemainCount(int remainCount)
        {
            RemainCount = Mathf.Clamp(remainCount, 0, 999);
            _txtRemainCount.text = remainCount.ToString("00");
            if (RemainCount == 0)
                DeActivate();
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }
    }
}