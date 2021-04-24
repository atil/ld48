using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum TileType
    {
        Water,
        Oxygen,
        Shark,
        Gem
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
            _game.HoveredTiles.Add(this);
        }
        
        private void OnMouseExit()
        {
            _game.HoveredTiles.Remove(this);
        }

        private void OnMouseDown()
        {
            if (Type == TileType.Water)
            {
                // No Effect on water
            }
            else if (Type == TileType.Oxygen)
            {
                _game.Oxygen += Value;
            }
            else if (Type == TileType.Shark)
            {
                _game.Oxygen -= Value;
            }
            else if (Type == TileType.Gem)
            {
                _game.Gem += Value;
            }
            else
            {
                Debug.LogError("Tile Type not found");
            }
            
            CoroutineStarter.Run(_game.OnTileClicked(this));
        }

        private void OnDestroy()
        {
            _game.HoveredTiles.Remove(this);
        }
    }
}