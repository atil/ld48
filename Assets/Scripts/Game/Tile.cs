using System;
using UnityEngine;

namespace Game
{
    public enum TileType
    {
        Water,
        Oxygen,
        Shark
    }
    public class Tile : MonoBehaviour
    {
        public (int i, int j) Index { get; set; }

        public int Value;
        public TileType Type;
        
        private GameMain _game;

        void Start()
        {
            _game = FindObjectOfType<GameMain>();
        }
        
        private void OnMouseEnter()
        {
            
        }
        
        private void OnMouseExit()
        {
            
        }

        private void OnMouseDown()
        {
            CoroutineStarter.Run(_game.OnTileClicked(this));

            if (Type == TileType.Water)
            {
                
            }
            else if (Type == TileType.Oxygen)
            {
                _game.Oxygen += Value;
            }
            else if (Type == TileType.Shark)
            {
                _game.Oxygen -= Value;
            }
        }
    }
}