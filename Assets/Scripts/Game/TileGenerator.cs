using System.Collections.Generic;
using System.Linq;
using Game;
using JamKit;
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
                _waterSprites[92]
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
            },
            {
                TileType.End,
                Resources.Load<Sprite>("Textures/End")
            }
        };
    }

    public List<Tile> GenerateTiles(int rowAmount, int columnAmount)
    {
        List<Tile> result = new List<Tile>();
        for (int i = 0; i < rowAmount; i++)
        {
            List<Tile> tempTiles = new List<Tile>();
            for (int j = 0; j < columnAmount; j++)
            {
                Tile newTile = i == 0 ? CreateWaterTile() : CreateRandomTile(tempTiles); // First row is water-tile only
                tempTiles.Add(newTile);
            }
            tempTiles.Shuffle();
            result.AddRange(tempTiles);
        }

        return result;
    }

    public Tile[] GenerateRow(int columnAmount)
    {
        Tile[] result = new Tile[columnAmount]; 
        List<Tile> tempTiles = new List<Tile>();
        for (int i = 0; i < columnAmount; i++)
        {
            var newTile = CreateRandomTile(tempTiles);
            tempTiles.Add(newTile);
        }
        tempTiles.Shuffle();
        for (int i = 0; i < columnAmount; i++)
        {
            result[i] = tempTiles[i];
        }

        return result;
    }

    private Tile CreateRandomTile(List<Tile> excludedTiles)
    {
        var newTile = Instantiate(TilePrefab, transform.position, Quaternion.identity);
        newTile.parent = TileRoot;

        var tileData = newTile.GetComponent<Tile>();
        if (excludedTiles.Count > 3)
        {
            tileData.Type = TileType.Shark;
        }
        else
        {
            do
            {
                tileData.Type = (TileType) Random.Range(0, 4);
            } while (excludedTiles.Any(x => tileData.Type == x.Type));
        }
        
        tileData.Value = 1;

        newTile.Find("Visual").GetComponent<SpriteRenderer>().sprite = _sprites[tileData.Type];
        return newTile.GetComponent<Tile>();
    }

    private Tile CreateWaterTile()
    {
        var newTile = Instantiate(TilePrefab, transform.position, Quaternion.identity);
        newTile.parent = TileRoot;

        var tileData = newTile.GetComponent<Tile>();
        tileData.Type = TileType.Water;
        tileData.Value = 1;

        newTile.Find("Visual").GetComponent<SpriteRenderer>().sprite = _sprites[TileType.Water];
        return newTile.GetComponent<Tile>();
    }
}
