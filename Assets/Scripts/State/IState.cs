namespace ConnectFoods.State
{
    using System.Collections;
    
    public interface IState
    {
        public IEnumerator Action();
    }
}