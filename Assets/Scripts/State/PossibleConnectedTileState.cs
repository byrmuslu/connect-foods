namespace ConnectFoods.State
{
    using ConnectFoods.Game;
    using ConnectFoods.Manager;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class PossibleConnectedTileState : IState
    {
        private BoardManager _boardManager;

        public PossibleConnectedTileState(BoardManager boardManager)
        {
            _boardManager = boardManager;
        }

        public IEnumerator Action()
        {
            var wait = new WaitForSeconds(.2f);
            while (true)
            {
                if (!_boardManager.Tiles.Any(t => t.IsEmpty))
                {
                    // for performance!!
                    bool shouldShuffle = true;
                    foreach (var tile in _boardManager.Tiles)
                    {
                        if (_boardManager.FindNeighbors(tile).Count > 2)
                        {
                            shouldShuffle = false;
                            break;
                        }
                    }
                    if (shouldShuffle)
                        ShuffleTiles();
                }
                else
                    _boardManager.SetState(new EmptyTilesFillState(_boardManager));
                yield return wait;
            }
        }


        public void ShuffleTiles()
        {
            var random = new System.Random();
            var listcCount = _boardManager.Tiles.Count;
            var newShuffledList = new List<Tile>();
            var list = new List<Tile>();
            list.AddRange(_boardManager.Tiles);

            for (int i = 0; i < listcCount; i++)
            {
                var randomElementInList = random.Next(0, list.Count);
                newShuffledList.Add(list[randomElementInList]);
                list.Remove(list[randomElementInList]);
            }

            for (int i = 0; i < listcCount; i++)
            {
                newShuffledList[i].Move(_boardManager.Tiles[i].Position);
            }

            _boardManager.Tiles = newShuffledList;

            _boardManager.Tiles.ForEach(t => t.SetSelected(false));
            _boardManager.SelectedTile = null;
        }

    }
}
