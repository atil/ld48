using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileGenerator : MonoBehaviour
{
    public Transform TilePrefab;
    public Transform TileRoot;

    private Dictionary<TileType, Sprite> _sprites;
    private Sprite[] _waterSprites;

    private void Awake()
    {
        _waterSprites = Resources.LoadAll<Sprite>("Textures/Water");
        _sprites = new Dictionary<TileType, Sprite>
        {
            {
                TileType.Water,
                _waterSprites[23]
            },
            {
                TileType.Gem,
                Resources.Load<Sprite>("Textures/Gem")
            },
            {
                TileType.Oxygen,
                Resources.Load<Sprite>("Textures/Tank")
            },
            {
                TileType.Shark,
                Resources.Load<Sprite>("Textures/Shark")
            }
        };
    }

    public List<Tile> GenerateTiles(int rowAmount, int columnAmount)
    {
        List<Tile> result = new List<Tile>();
        for (int i = 0; i < rowAmount; i++)
        {
            for (int j = 0; j < columnAmount; j++)
            {
                var newTile = Instantiate(TilePrefab, transform.position, Quaternion.identity);
                newTile.parent = TileRoot;
                
                var tileData = newTile.GetComponent<Tile>();
                tileData.Type = (TileType)Random.Range(0, 4);
                tileData.Value = Random.Range(1, 4);
                
                newTile.Find("Visual").GetComponent<SpriteRenderer>().sprite = _sprites[tileData.Type];
                
                result.Add(newTile.GetComponent<Tile>());
            }
        }

        return result;
    }

    public Tile[] GenerateRow()
    {
        Tile[] result = new Tile[4]; 
        for (int i = 0; i < 4; i++)
        {
            var newTile = Instantiate(TilePrefab, transform.position, Quaternion.identity);
            newTile.parent = TileRoot;
            result[i] = newTile.GetComponent<Tile>();
        }

        return result;
    }
}
