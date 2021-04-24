using System.Collections.Generic;
using Game;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public Transform TilePrefab;
    public Transform TileRoot;

    public List<Tile> GenerateTiles(int rowAmount, int columnAmount)
    {
        List<Tile> result = new List<Tile>();
        for (int i = 0; i < rowAmount; i++)
        {
            for (int j = 0; j < columnAmount; j++)
            {
                var newTile = Instantiate(TilePrefab, transform.position, Quaternion.identity);
                newTile.parent = TileRoot;
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
