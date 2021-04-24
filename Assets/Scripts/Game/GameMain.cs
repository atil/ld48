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
        public Transform TileRoot;
        public Transform BackgroundRoot;

        public int Oxygen = 10;
        public GameDirection Direction = GameDirection.Down;
        public int Gem = 0;

        public int RowCount = 4;
        public int ColumnCount = 5;
        public float OffsetBetweenTiles = 1.5f;
        
        private List<Tile[]> _tiles = new List<Tile[]>();
        private bool _isMoving;
        private int _playerRowIndex = -1;
        
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

            if (Direction == GameDirection.Down && _playerRowIndex >= tile.Index.i)
            {
                yield break; // Can't go up or sideways while travelling down
            }
            
            if (Direction == GameDirection.Up && _playerRowIndex <= tile.Index.i)
            {
                yield break; // Can't go down or sideways when travelling up
            }

            int amountOfMovement = Mathf.Abs(tile.Index.i - _playerRowIndex);
            _playerRowIndex = tile.Index.i;

            Oxygen--;
            GameUi.SetOxygen(Oxygen);
            
            GameUi.SetGem(Gem);
            
            Vector3 playerSrc = Player.transform.position;
            Vector3 playerTarget = tile.transform.position;

            _tiles[tile.Index.i][tile.Index.j] = null;
            Destroy(tile.gameObject);

            // 
            // First, move the player to the target tile
            // 
            
            _isMoving = true;
            const float moveDuration = 0.5f;
            CoroutineStarter.Run(Player.PlayMoveAnim(moveDuration, Direction));
            yield return Curve.Tween(MoveCurve,
                moveDuration,
                t =>
                {
                    Player.transform.position = Vector3.Lerp(playerSrc, playerTarget, t);
                });
            
            // 
            // Then, move these upwards
            // - Tiles
            // - Player
            // - Background
            // 

            const float spaceBetweenTiles = 1.5f;
            Vector3 scrollAmount = (Direction == GameDirection.Down ? Vector3.up : Vector3.down) * spaceBetweenTiles * amountOfMovement;
            Vector3 tileRootSrc = TileRoot.position;
            Vector3 tileRootTarget = TileRoot.position + scrollAmount;
            
            Curve.Tween(MoveCurve,
                moveDuration,
                t =>
                {
                    TileRoot.position = Vector3.Lerp(tileRootSrc, tileRootTarget, t);
                },
                () => { });

            playerSrc = Player.transform.position;
            playerTarget = playerSrc + scrollAmount;
            
            Curve.Tween(MoveCurve,
                moveDuration,
                t =>
                {
                    Player.transform.position = Vector3.Lerp(playerSrc, playerTarget, t);
                },
                () => { });

            Vector3 backgroundSrc = BackgroundRoot.position;
            Vector3 backgroundTarget = BackgroundRoot.position + scrollAmount;
            Curve.Tween(MoveCurve,
                moveDuration,
                t =>
                {
                    BackgroundRoot.position = Vector3.Lerp(backgroundSrc, backgroundTarget, t);
                },
                () => { });

            // Generate new row

            for (int j = 0; j < amountOfMovement; j++)
            {
                var newRow = TileGenerator.GenerateRow(ColumnCount);
                for (int i = 0; i < ColumnCount; i++)
                {
                    newRow[i].Index = (_tiles.Count, i);
                    newRow[i].transform.localPosition = new Vector3(i * OffsetBetweenTiles, _tiles.Count * -1 * OffsetBetweenTiles, 0);
                }

                _tiles.Add(newRow);
            }
            
            yield return new WaitForSeconds(moveDuration);
            
            Player.transform.position = playerTarget;
            _isMoving = false;
            
            if (Oxygen == 0)
            {
                SceneManager.LoadScene("End"); // TODO: Smooth transition
            }
        }

        public void OnReturnButtonClicked()
        {
            _isMoving = true;
            const float moveDuration = 1.5f;
            int amountOfMovement = Mathf.Abs((_tiles.Count - 1) - _playerRowIndex); 

            Direction = GameDirection.Up;
            
            CoroutineStarter.Run(Player.PlayMoveAnim(moveDuration, Direction));
            
            const float spaceBetweenTiles = 1.5f;
            Vector3 scrollAmount = (Direction == GameDirection.Down ? Vector3.up : Vector3.down) * spaceBetweenTiles * amountOfMovement;
            Vector3 tileRootSrc = TileRoot.position;
            Vector3 tileRootTarget = TileRoot.position + scrollAmount;
            
            Curve.Tween(MoveCurve,
                moveDuration,
                t =>
                {
                    TileRoot.position = Vector3.Lerp(tileRootSrc, tileRootTarget, t);
                },
                () => { });
            
            Vector3 playerSrc = Player.transform.position;
            Vector3 playerTarget = playerSrc + scrollAmount;
            
            Curve.Tween(MoveCurve,
                moveDuration,
                t =>
                {
                    Player.transform.position = Vector3.Lerp(playerSrc, playerTarget, t);
                },
                () => { });

            Vector3 backgroundSrc = BackgroundRoot.position;
            Vector3 backgroundTarget = BackgroundRoot.position + scrollAmount;
            Curve.Tween(MoveCurve,
                moveDuration,
                t =>
                {
                    BackgroundRoot.position = Vector3.Lerp(backgroundSrc, backgroundTarget, t);
                },
                () => { });

            _isMoving = false;
        }
    }
}