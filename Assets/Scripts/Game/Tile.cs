﻿using System;
using JamKit;
using UnityEngine;

namespace Game
{
    public enum TileType
    {
        Water,
        Oxygen,
        Shark,
        Gem,
        End
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

        public void PlaySfx(int playerRowIndex)
        {
            if (playerRowIndex == -1)
            {
                Sfx.Instance.Play("FirstSplash");
            }
            else
            {
                Sfx.Instance.Play("Swim");
            }

            switch (Type)
            {
                case TileType.Water: 
                    break;
                case TileType.Oxygen:
                    Sfx.Instance.Play("Breathe");
                    break;
                case TileType.Shark:
                    Sfx.Instance.Play("Shark");
                    break;
                case TileType.Gem:
                    Sfx.Instance.Play("Gem");
                    break;
                case TileType.End:
                    Sfx.Instance.Play("FirstSplash");
                    Sfx.Instance.Play("Gem"); //TODO End Game Sfx
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
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
                _game.Oxygen = Mathf.Clamp(_game.Oxygen, 0, _game.MaxOxygen);
            }
            else if (Type == TileType.Shark)
            {
                _game.Oxygen -= Value;
                _game.Oxygen = Mathf.Clamp(_game.Oxygen, 0, _game.MaxOxygen);
            }
            else if (Type == TileType.Gem)
            {
                _game.Score += 10;
            }
            else if (Type == TileType.End)
            {
                _game.Score += 10;
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