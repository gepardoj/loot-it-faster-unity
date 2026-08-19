using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class ChestGenerator : MonoBehaviour, ILevelGenerator
{
  public static ChestGenerator Instance { get; private set; }
  private const float CHEST_OFFSET = 1f;

  [SerializeField] private GameObject chestPrefab;

  [SerializeField] private int chestSpawnSectorSize = 6;


  private void Awake()
  {
    Instance = this;
  }

  public void Generate(MazeGenerator maze)
  {
    for (int x = 0; x < maze.Grid.GetLength(0) / chestSpawnSectorSize; x++)
    {
      for (int y = 0; y < maze.Grid.GetLength(1) / chestSpawnSectorSize; y++)
      {
        int chestX = Random.Range(x * chestSpawnSectorSize, (x + 1) * chestSpawnSectorSize);
        int chestY = Random.Range(y * chestSpawnSectorSize, (y + 1) * chestSpawnSectorSize);
        maze.Grid[chestX, chestY] = CellType.Chest;
        MakeCorridorsToPlayer(maze, Direction.None, chestX, chestY, maze.PlayerStartX, maze.PlayerStartY);
      }
    }
  }

  private void MakeCorridorsToPlayer(MazeGenerator maze, Direction comesFrom, int startX, int startY, int playerX, int playerY)
  {
    var left = maze.IsValid(startX - 1, startY) ? maze.Grid[startX - 1, startY] : CellType.Wall;
    var right = maze.IsValid(startX + 1, startY) ? maze.Grid[startX + 1, startY] : CellType.Wall;
    var up = maze.IsValid(startX, startY - 1) ? maze.Grid[startX, startY - 1] : CellType.Wall;
    var down = maze.IsValid(startX, startY + 1) ? maze.Grid[startX, startY + 1] : CellType.Wall;
    // if our chest is surrounded by walls then make corridor
    if ((comesFrom == Direction.Left || left == CellType.Wall || left == CellType.Chest) &&
    (comesFrom == Direction.Right || right == CellType.Wall || right == CellType.Chest) &&
    (comesFrom == Direction.Up || up == CellType.Wall || up == CellType.Chest) &&
    (comesFrom == Direction.Down || down == CellType.Wall || down == CellType.Chest))
    {
      int dx = playerX - startX;
      int dy = playerY - startY;
      if (dx != 0)
      {
        // move by x
        int sign = Math.Sign(dx);
        var from = sign == 1 ? Direction.Left : Direction.Right;
        var newX = startX + sign;
        maze.Grid[newX, startY] = CellType.Empty;
        MakeCorridorsToPlayer(maze, from, newX, startY, playerX, playerY);
      }
      else if (dy != 0)
      {
        // move by y
        int sign = Math.Sign(dy);
        var from = sign == 1 ? Direction.Up : Direction.Down;
        var newY = startY + sign;
        maze.Grid[startX, newY] = CellType.Empty;
        MakeCorridorsToPlayer(maze, from, startX, newY, playerX, playerY);
      }
    }
  }

  public void SpawnObjects(MazeGenerator maze)
  {
    for (int x = 0; x < maze.Width; x++)
    {
      for (int y = 0; y < maze.Height; y++)
      {
        var cell = maze.Grid[x, y];
        if (cell == CellType.Player || (cell == CellType.Empty && Random.Range(1, 15) == 1))
        {
          Vector3 pos;
          var rotation = Quaternion.identity;
          try
          {
            var (dx, dy) = maze.GetNearestWall(x, y);
            pos = new Vector3(x * maze.CellSize + dx * CHEST_OFFSET, -maze.CellSize / 2f, y * maze.CellSize + dy * CHEST_OFFSET);
            chestPrefab.name = $"chest dx = {dx}, dy = {dy}";
            if (dx == 1) rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            else if (dx == 0 && dy == 1) rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            else if (dx == 0) rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            else if (dx == -1) rotation = Quaternion.Euler(new Vector3(0, 0, 0));
          }
          catch (Exception)
          {
            pos = new Vector3(x * maze.CellSize, -maze.CellSize / 2f, y * maze.CellSize);
          }
          Instantiate(chestPrefab, pos, rotation, transform);
        }
      }
    }
  }
}
