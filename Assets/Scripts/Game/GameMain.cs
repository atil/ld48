using System;
using System.Collections.Generic;
using JamKit;
using UnityEngine;

namespace Game
{
    public class GameMain : MonoBehaviour
    {
        public List<Tile> TempTiles;
        public Player Player;
        public AnimationCurve MoveCurve;
        
        private List<Tile[]> _tiles = new List<Tile[]>();
        private bool _isMoving;
        
        private void Start()
        {
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

        public void OnTileClicked(Tile tile)
        {
            if (_isMoving)
            {
                return;
            }
            
            Vector3 srcPos = Player.transform.position;
            Vector3 targetTilePos = tile.transform.position;
            
            _tiles[tile.Index.i][tile.Index.j] = null;
            Destroy(tile.gameObject);

            _isMoving = true;
            Curve.Tween(MoveCurve,
                0.5f,
                t =>
                {
                    Player.transform.position = Vector3.Lerp(srcPos, targetTilePos, t);
                },
                () =>
                {
                    Player.transform.position = targetTilePos;
                    _isMoving = false;
                });
        }
    }
}