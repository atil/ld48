using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public int ShowReturnButtonAt = 5;
        public GameObject Cursor;
        public HashSet<Tile> HoveredTiles = new HashSet<Tile>();
        private Tile _hoveredTile;
        public ParticleSystem BubbleTrail;

        public int MaxOxygen = 16;
        public int Oxygen = 10;
        public GameDirection Direction = GameDirection.Down;
        public int Score = 0;

        public int RowCount = 4;
        public int ColumnCount = 5;
        public float OffsetBetweenTiles = 1.5f;
        public int PlayerRowIndex = -1;
        public int PlayerColumnIndex = 2;
        public int PlayerCurrentDepthRecord = 0;

        public int VerticalRange = 1;
        public int HorizontalRange = 1;

        private List<Tile[]> _tiles = new List<Tile[]>();
        private bool _isMoving;

        public bool OnReturnStage = false;
        public GameObject EndTileRoot;
        public Transform CameraTransform;

        private void Start()
        {
            Sfx.Instance.ChangeMusicTrack("Music", true);
            ResultData.Instance.Clear();

            Oxygen = MaxOxygen;
            GameUi.SetOxygen(MaxOxygen);

            var initialTiles = TileGenerator.GenerateTiles(RowCount, ColumnCount);
            
            Player.PlayIdleAnim(GameDirection.Up);
            
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

            Score = 0;
            GameUi.SetScore(Score);
        }

        private void Update()
        {
            if (HoveredTiles.Count == 1 && !_isMoving)
            {
                Tile tile = HoveredTiles.First();
                if (IsTileMovable(tile) && _hoveredTile != tile)
                {
                    Cursor.SetActive(true);
                    Cursor.transform.position = tile.transform.position;
                    Sfx.Instance.Play("Hover");
                    _hoveredTile = tile;
                }
            }
            else
            {
                Cursor.SetActive(false);
                _hoveredTile = null;
            }

            BubbleTrail.transform.position = Player.transform.position;
        }

        private bool IsTileMovable(Tile tile)
        {
            if (Direction == GameDirection.Down && PlayerRowIndex != tile.Index.i - VerticalRange)
            {
                return false; // Can't go up or sideways while travelling down
            }
            
            if (Direction == GameDirection.Up)
            {
                if (tile.Index.i == -1)
                {
                    return true; // End Tile Index
                }
                if (PlayerRowIndex != tile.Index.i + VerticalRange)
                {
                    return false; // Can't go down or sideways when travelling up
                }
            }

            if (Mathf.Abs(PlayerColumnIndex - tile.Index.j) > HorizontalRange)
            {
                return false;
            }

            return true;
        }

        public IEnumerator OnTileClicked(Tile tile)
        {
            if (_isMoving)
            {
                yield break;
            }

            if (!IsTileMovable(tile))
            {
                yield break;
            }
            
            //
            // Move is valid. Change stuff here
            //

            tile.PlaySfx(PlayerRowIndex);

            int amountOfMovement = Mathf.Abs(tile.Index.i - PlayerRowIndex);
            PlayerRowIndex = tile.Index.i;
            PlayerColumnIndex = tile.Index.j;
            GameUi.SetDepth(PlayerRowIndex);

            if (!OnReturnStage)
            {
                Score += (PlayerRowIndex + 1);
                GameUi.SetScore(Score, (PlayerRowIndex + 1), ScoreType.Depth);
            }
            
            //
            // Tile special effect
            // 
            
            int delta = ((PlayerRowIndex / 10) + 1) * 5;
            
            if (tile.Type == TileType.Water)
            {
                // No Effect on water
            }
            else if (tile.Type == TileType.Oxygen)
            {
                Oxygen += 2;
                Oxygen = Mathf.Clamp(Oxygen, 0, MaxOxygen + 1);
            }
            else if (tile.Type == TileType.Shark)
            {
                Oxygen -= 1;
                Oxygen = Mathf.Clamp(Oxygen, 0, MaxOxygen + 1);
            }
            else if (tile.Type == TileType.Gem)
            {
                Score += delta;
                GameUi.SetScore(Score, delta, ScoreType.Gem);
            }
            else if (tile.Type == TileType.End)
            {
                Score += delta;
                GameUi.SetScore(Score, delta, ScoreType.Gem);
            }
            else
            {
                Debug.LogError("Tile Type not found");
            }
            
            //
            // Oxygen decrease due to depth
            //

            Oxygen--;
            GameUi.SetOxygen(Oxygen);
            Player.Exclamation.SetActive(Oxygen <= 3);
            Player.Exclamation.transform.SetParent(Direction == GameDirection.Down ? Player.ExclamationParentDown : Player.ExclamationParentUp, false);

            Vector3 playerSrc = Player.transform.position;
            Vector3 playerTarget = tile.transform.position;

            if (tile.Index.i >= 0 && tile.Index.j >= 0)
            {
                _tiles[tile.Index.i][tile.Index.j] = null;
            }
            Destroy(tile.gameObject);

            // 
            // First, move the player to the target tile
            // 

            
            _isMoving = true;
            const float moveDuration = 0.5f;
            
            // Set bubble particles
            // BubbleTrail.Play();
            // BubbleTrail.simulationSpace = ParticleSystemSimulationSpace.World;
            // ParticleSystem.ShapeModule shape = BubbleTrail.shape;
            // shape.position = 
            //
            // CoroutineStarter.RunDelayed(moveDuration, () => BubbleTrail.simulationSpace = ParticleSystemSimulationSpace.Local);
            
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
            Vector3 scrollAmount = (Direction == GameDirection.Up ? Vector3.up : Vector3.down) * spaceBetweenTiles * amountOfMovement;
            Vector3 cameraSrc = CameraTransform.position;
            Vector3 cameraTarget = cameraSrc + scrollAmount;
            
            Curve.Tween(MoveCurve,
                moveDuration,
                t =>
                {
                    CameraTransform.position = Vector3.Lerp(cameraSrc, cameraTarget, t);
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

            if (PlayerRowIndex > ShowReturnButtonAt && !OnReturnStage)
            {
                GameUi.ShowReturnButton();
            }

            if (PlayerRowIndex < 0 && OnReturnStage)
            {
                _isMoving = true;
                PlayerPrefs.SetInt("DepthRecord", PlayerCurrentDepthRecord);
                ResultData.Instance.HasWon = true;
                ResultData.Instance.Score = Score;
                ResultData.Instance.Depth = PlayerCurrentDepthRecord;
                GameUi.CloseFlash();
            }
            else if (Oxygen <= 0)
            {
                _isMoving = true;
                ResultData.Instance.HasWon = false;
                CoroutineStarter.Run(Player.PlayDeadAnim(1.9f, Direction));
                GameUi.CloseFlash();
            }
        }

        public void OnReturnButtonClicked()
        {
            if (OnReturnStage)
            {
                return;
            }

            PlayerCurrentDepthRecord = PlayerRowIndex + 1;
            StartCoroutine(SwimUpCoroutine());
        }

        private IEnumerator SwimUpCoroutine()
        {
            _isMoving = true;

            Sfx.Instance.Play("SwimUpRotate");
            
            const float moveDuration = 1.5f;
            int amountOfMovement = Mathf.Abs((_tiles.Count - 1) - PlayerRowIndex); 

            Direction = GameDirection.Up;
            
            CoroutineStarter.Run(Player.PlayMoveAnim(moveDuration, Direction));
            
            const float spaceBetweenTiles = 1.5f;
            Vector3 scrollAmount = (Direction == GameDirection.Up ? Vector3.up : Vector3.down) * spaceBetweenTiles * amountOfMovement;
            Vector3 cameraSrc = CameraTransform.position;
            Vector3 cameraTarget = cameraSrc + scrollAmount;
            
            Curve.Tween(MoveCurve,
                moveDuration,
                t =>
                {
                    CameraTransform.position = Vector3.Lerp(cameraSrc, cameraTarget, t);
                },
                () => { });

            
            EndTileRoot.SetActive(true);
            EndTileRoot.GetComponent<Tile>().Index = (-1, 2);
            
            OnReturnStage = true;

            yield return new WaitForSeconds(moveDuration);
            
            _isMoving = false;
        } 
    }
}