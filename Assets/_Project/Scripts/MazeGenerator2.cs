using Unity.AppUI.Core;
using UnityEngine;

public enum CellType
{
  Empty,
  Wall,
  Player,
  Chest,
}

public enum Direction
{
  None, Left, Right, Up, Down
}

public class MazeGenerator2 : MonoBehaviour
{
  [Header("Labyrinth config")]
  [SerializeField] private int width = 20;
  [SerializeField] private int height = 20;
  [SerializeField] private float cellSize = 1f;
  [SerializeField] private int corridorsNum = 80;

  [Header("Prefabs")]
  [SerializeField] private GameObject wallPrefab;
  [SerializeField] private GameObject chestPrefab;

  private CellType[,] grid;


  void Start()
  {
    GenerateMaze();
    SpawnObjects();
  }

  private void GenerateMaze()
  {
    grid = new CellType[width, height];
    var playerStartX = Random.Range(5, width - 5);
    var playerStartY = Random.Range(5, height - 5);

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        grid[x, y] = CellType.Wall;
      }
    }

    MakeCorridors(playerStartX, playerStartY);
    MakeOuterWalls();

    grid[playerStartX, playerStartY] = CellType.Player;
  }

  private void MakeCorridors(int startX, int startY)
  {
    var x = startX;
    var y = startY;
    var dir = Direction.None;
    for (int i = 0; i < corridorsNum; i++)
    {
      Direction[] availableDirs;
      switch (dir)
      {
        case Direction.Left:
          availableDirs = new[] { Direction.Left, Direction.Up, Direction.Down };
          break;
        case Direction.Right:
          availableDirs = new[] { Direction.Right, Direction.Up, Direction.Down };
          break;
        case Direction.Up:
          availableDirs = new[] { Direction.Right, Direction.Up, Direction.Left };
          break;
        case Direction.Down:
          availableDirs = new[] { Direction.Right, Direction.Down, Direction.Left };
          break;
        default:
          availableDirs = new[] { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
          break;
      }
      dir = availableDirs[Random.Range(0, availableDirs.Length)];
      for (int len = 1; len <= 2; len++)
      {
        switch (dir)
        {
          case Direction.Left: x--; break;
          case Direction.Right: x++; break;
          case Direction.Up: y++; break;
          case Direction.Down: y--; break;
        }
        if (x == -1 || y == -1 || x == width || y == height)
        {
          x = startX;
          y = startY;
          dir = Direction.None;
          break;
        }
        if (grid[x, y] == CellType.Player)
        {
          continue;
        }
        grid[x, y] = CellType.Empty;
      }
    }
  }

  private void MakeOuterWalls()
  {
    for (int x = 0; x < width; x++)
    {
      grid[x, 0] = CellType.Wall;
      grid[x, height - 1] = CellType.Wall;
    }
    for (int y = 0; y < height; y++)
    {
      grid[0, y] = CellType.Wall;
      grid[width - 1, y] = CellType.Wall;
    }
  }

  private void SpawnObjects()
  {
    // walls in a maze
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        var cell = grid[x, y];
        if (cell == CellType.Wall)
        {
          var pos = new Vector3(x * cellSize, 0, y * cellSize);
          Instantiate(wallPrefab, pos, Quaternion.identity, transform);
        }
        else
        {
          var floorPos = new Vector3(x * cellSize, -cellSize, y * cellSize);
          Instantiate(wallPrefab, floorPos, Quaternion.identity, transform);
        }
      }
    }
  }
}
