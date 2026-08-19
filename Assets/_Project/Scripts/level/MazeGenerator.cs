using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

public class MazeGenerator : MonoBehaviour
{
  [Header("Labyrinth config")]
  [SerializeField] private int width = 20;
  public int Width => width;
  [SerializeField] private int height = 20;
  public int Height => height;
  [SerializeField] private float cellSize = 3f;
  public float CellSize => cellSize;
  [SerializeField] private int corridorsNum = 80;

  [Header("Prefabs")]
  [SerializeField] private GameObject playerPrefab;
  [SerializeField] private GameObject wallPrefab;

  private CellType[,] grid;
  public CellType[,] Grid => grid;

  private int playerStartX;
  public int PlayerStartX => playerStartX;
  private int playerStartY;
  public int PlayerStartY => playerStartY;


  private void Awake()
  {
    GenerateMaze();
    SpawnObjects();
    var generators = GetComponentsInChildren<MonoBehaviour>().OfType<ILevelGenerator>();
    foreach (var gen in generators)
    {
      gen.Generate(this);
      gen.SpawnObjects(this);
    }
  }

  private void GenerateMaze()
  {
    grid = new CellType[width, height];
    playerStartX = Random.Range(5, width - 5);
    playerStartY = Random.Range(5, height - 5);

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

  public bool IsValid(int x, int y)
  {
    return x >= 0 && x < width && y >= 0 && y < height;
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
          var obj = Instantiate(wallPrefab, pos, Quaternion.identity, transform);
        }
        else
        {
          var floorPos = new Vector3(x * cellSize, -cellSize, y * cellSize);
          Instantiate(wallPrefab, floorPos, Quaternion.identity, transform);
          var ceilingPos = new Vector3(x * cellSize, cellSize, y * cellSize);
          Instantiate(wallPrefab, ceilingPos, Quaternion.identity, transform);
          if (cell == CellType.Player)
          {
            var pos = new Vector3(x * cellSize, -cellSize / 2f, y * cellSize);
            playerPrefab.transform.SetPositionAndRotation(pos, Quaternion.identity);
          }
        }
      }
    }
  }

  public (int, int) GetNearestWall(int posX, int posY)
  {
    if (grid[posX - 1, posY] == CellType.Wall) return (-1, 0);
    if (grid[posX + 1, posY] == CellType.Wall) return (1, 0);
    if (grid[posX, posY - 1] == CellType.Wall) return (0, -1);
    if (grid[posX, posY + 1] == CellType.Wall) return (0, 1);
    throw new System.Exception("Wall not found");
  }
}
