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
        public List<Tile> TempTiles;
        public Player Player;
        public AnimationCurve MoveCurve;
        public GameUi GameUi;

        public int Oxygen = 10;
        public GameDirection Direction = GameDirection.Down;
        public int Gem = 0;
        
        private List<Tile[]> _tiles = new List<Tile[]>();
        private bool _isMoving;
        
        private void Start()
        {
            GameUi.SetOxygen(Oxygen);
            Player.PlayIdleAnim(Direction);
            for (int i = 0; i < 5; i++)
            {
                _tiles.Add(new Tile[4]);
                for (int j = 0; j < 4; j++)
                {
                    _tiles[i][j] = TempTiles[i * 4 + j];
                    _tiles[i][j].Index = (i, j);
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