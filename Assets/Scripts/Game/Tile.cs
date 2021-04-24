using System;
using UnityEngine;

namespace Game
{
    public class Tile : MonoBehaviour
    {
        public (int i, int j) Index { get; set; }
        
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
        }
    }
}