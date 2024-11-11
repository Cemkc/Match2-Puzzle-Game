using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Grid
{

    [SerializeField] private int _playFieldDimension;
    [SerializeField] private int[] _occcupiedPositions;
    private Dictionary<int, Queue<TileObjType>> _tileObjPool;
    [SerializeField] private GameObject[] _blockPrefabs;

    // private float[,] _gridArray;

    public int PlayFieldDimension { get => _playFieldDimension; }
    public int[] OcccupiedPositions { get => _occcupiedPositions; }

    public Grid(GridBlueprint gridBP)
    {
        _playFieldDimension = gridBP.PlayFieldDimension;
        _occcupiedPositions = gridBP.OcccupiedPositions;
        _blockPrefabs = gridBP.BlockPrefabs;
    }

    public void RandomFillTiles()
    {   
        foreach (int tile in _occcupiedPositions)
        {
            int tileObjectsIndex = Random.Range(0, LevelManager.s_Instance.TileObjects.Count);
            TileObject tileObj = LevelManager.s_Instance.TileObjects[tileObjectsIndex].GetComponent<TileObject>();
            LevelManager.s_Instance.TileMap[tile].SetTile(tileObj.GetTileObjType());
        }
    }

    public void ClickTile(int tileNumber)
    {
        // foreach (int tile in GetAdjacentTiles(tileNumber))
        // {
        //     Debug.Log(tile);
        // }


        List<int> connectedTiles = new List<int>();
        GetConnectedTiles(tileNumber, ref connectedTiles);
        string str = "Cliked at tile: " + tileNumber + ", connected tiles are: ";
        foreach (int tile in connectedTiles)
        {
            str += tile + ", ";
        }
        Debug.Log(str);

        if(connectedTiles.Count > 1)
        {
            foreach (int tileId in connectedTiles)
            {
                LevelManager.s_Instance.TileMap[tileId].SetTile(TileObjType.None);
            }
        }

    }

    private void GetConnectedTiles(int tile, ref List<int> connectedTiles, int previousTile = -1)
    {
        List<int> adjacentTiles = GetAdjacentTiles(tile);
        // string str = "";
        // foreach (int adjTile in adjacentTiles)
        // {
        //     str += adjTile.ToString() + ", ";
        // }
        // Debug.Log("Adjacent tiles for tile " + tile + ": " + str);

        if(previousTile == -1)
        {
            connectedTiles.Add(tile);
        }
        
        foreach (int adjacentTile in adjacentTiles)
        {
            if(adjacentTile == previousTile)
            {
                continue;
            }
            TileObjType selfType = LevelManager.s_Instance.TileMap[tile].GetTileType();
            TileObjType adjacentType = LevelManager.s_Instance.TileMap[adjacentTile].GetTileType();
            if(selfType == adjacentType)
            {
                if(!connectedTiles.Contains(adjacentTile)) connectedTiles.Add(adjacentTile);
                GetConnectedTiles(adjacentTile, ref connectedTiles, tile);
            }
        }

        return;
    }

    private List<int> GetAdjacentTiles(int tile)
    {
        int dimension = LevelManager.s_Instance.PlayfieldDimension;
        List<int> adjacentTiles = new List<int>();

        // Calculate row and column of the given tile
        int row = tile / dimension;
        int col = tile % dimension;

        // Check above (row + 1)
        if (row + 1 < dimension)
        {
            adjacentTiles.Add(tile + dimension);
        }

        // Check below (row - 1)
        if (row - 1 >= 0)
        {
            adjacentTiles.Add(tile - dimension);
        }

        // Check right (col + 1)
        if (col + 1 < dimension)
        {
            adjacentTiles.Add(tile + 1);
        }

        // Check left (col - 1)
        if (col - 1 >= 0)
        {
            adjacentTiles.Add(tile - 1);
        }

        return adjacentTiles;
    }

}
