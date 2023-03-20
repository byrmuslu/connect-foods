namespace ConnectFoods.State
{
    using ConnectFoods.Game;
    using ConnectFoods.Manager;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EmptyTilesFillState : IState
    {
        private BoardManager _boardManager;

        public EmptyTilesFillState(BoardManager boardManager)
        {
            _boardManager = boardManager;
        }

        public IEnumerator Action()
        {
            var wait = new WaitForSeconds(.2f);
            while (true)
            {
                List<Tile> emptyTiles = _boardManager.Tiles.FindAll(c => c.IsEmpty);
                if (emptyTiles.Count == 0)
                    _boardManager.SetState(new PossibleConnectedTileState(_boardManager));
                else
                    emptyTiles.ForEach(c => _boardManager.FillTile(c));
                yield return wait;
            }
        }
    }
}
