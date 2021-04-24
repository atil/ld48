using System.Collections;
using System.Collections.Generic;
using JamKit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public enum GameDirection
    {
        Up, Down
    }
    
    public class GameMain : MonoBehaviour
    {
        public Player Player;
        public AnimationCurve MoveCurve;
        public GameUi GameUi;
        public TileGenerator TileGenerator;

        public int Oxygen = 10;
        public GameDirection Direction = GameDirection.Down;
        public int Gem = 0;

        public int RowCount = 4;
        public int ColumnCount = 5;
        public float OffsetBetweenTiles = 1.5f;
        
        private List<Tile[]> _tiles = new List<Tile[]>();
        private bool _isMoving;
        
        private void Start()
        {
            GameUi.SetOxygen(Oxygen);

            var initialTiles = TileGenerator.GenerateTiles(RowCount, ColumnCount);
            
            Player.PlayIdleAnim(Direction);
            
            for (int i = 0; i < RowCount; i++)
            {
                _tiles.Add(new Tile[ColumnCount]);
                for (int j = 0; j < ColumnCount; j++)
                {
                    _tiles[i][j] = initialTiles[i * ColumnCount + j];
                    _tiles[i][j].Index = (i, j);
                    _tiles[i][j].transform.localPosition = new Vector3(j * OffsetBetweenTiles, i * -1 * OffsetBetweenTiles, 0);
                }
            }
        }

        public IEnumerator OnTileClicked(Tile tile)
        {
            if (_isMoving)
            {
                yield break;
            }
            
            // TODO: Click only on the tiles below

            Oxygen--;
            GameUi.SetOxygen(Oxygen);
            
            GameUi.SetGem(Gem);
            
            Vector3 srcPos = Player.transform.position;
            Vector3 targetTilePos = tile.transform.position;
            
            _tiles[tile.Index.i][tile.Index.j] = null;
            Destroy(tile.gameObject);

            _isMoving = true;
            const float moveDuration = 0.5f;
            CoroutineStarter.Run(Player.PlayMoveAnim(moveDuration, Direction));
            yield return Curve.Tween(MoveCurve,
                moveDuration,
                t =>
                {
                    Player.transform.position = Vector3.Lerp(srcPos, targetTilePos, t);
                });
            
            Player.transform.position = targetTilePos;
            _isMoving = false;
            
            if (Oxygen == 0)
            {
                SceneManager.LoadScene("End"); // TODO: Smooth transition
            }
        }
    }
}