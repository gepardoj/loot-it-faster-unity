using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

  [Header("Labyrinth config")]
  [SerializeField] private int width = 10;
  [SerializeField] private int height = 10;
  [SerializeField] private float cellSize = 3f;

  [Header("Prefabs")]
  [SerializeField] private GameObject wallPrefab;
  [SerializeField] private GameObject floorPrefab;
  [SerializeField] private GameObject chestPrefab;

  private MazeCell[,] grid;


  void Start()
  {
    GenerateMaze();
    SpawnMazeObjects();
  }

  private void GenerateMaze()
  {
    grid = new MazeCell[width, height];

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        grid[x, y] = new MazeCell(x, y);
      }
    }

    var stack = new Stack<MazeCell>();
    var current = grid[0, 0];
    current.Visited = true;

    while (true)
    {
      var unvisitedNeighbours = GetUnvisitedNeighbours(current);

      if (unvisitedNeighbours.Count > 0)
      {
        MazeCell next = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
        BreakWalls(current, next);
        stack.Push(current);
        current = next;
        current.Visited = true;
      }
      else if (stack.Count > 0)
      {
        current = stack.Pop();
      }
      else
      {
        break;
      }
    }
  }

  private List<MazeCell> GetUnvisitedNeighbours(MazeCell cell)
  {
    var neighbours = new List<MazeCell>();

    if (cell.X > 0 && !grid[cell.X - 1, cell.Y].Visited) neighbours.Add(grid[cell.X - 1, cell.Y]);
    if (cell.X < width - 1 && !grid[cell.X + 1, cell.Y].Visited) neighbours.Add(grid[cell.X + 1, cell.Y]);
    if (cell.Y > 0 && !grid[cell.X, cell.Y - 1].Visited) neighbours.Add(grid[cell.X, cell.Y - 1]);
    if (cell.Y < height - 1 && !grid[cell.X, cell.Y + 1].Visited) neighbours.Add(grid[cell.X, cell.Y + 1]);

    return neighbours;
  }

  private void BreakWalls(MazeCell current, MazeCell next)
  {
    if (current.X > next.X) { current.WallLeft = false; next.WallRight = false; }
    else if (current.X < next.X) { current.WallRight = false; next.WallLeft = false; }
    else if (current.Y > next.Y) { current.WallBack = false; next.WallFront = false; }
    else if (current.Y < next.Y) { current.WallFront = false; next.WallBack = false; }
  }

  private void SpawnMazeObjects()
  {
    // outer walls
    for (int x = 0; x < width; x++)
    {
      wallPrefab.name = $"outer wall x = {x}";
      Instantiate(wallPrefab, new Vector3(x + 1, cellSize / 2f, -1 + 1), Quaternion.identity, transform);
      Instantiate(wallPrefab, new Vector3(x + 1, cellSize / 2f, height + 1), Quaternion.identity, transform);
    }
    for (int y = 0; y < height; y++)
    {
      wallPrefab.name = $"outer wall y = {y}";
      Instantiate(wallPrefab, new Vector3(-1 * cellSize, cellSize / 2f, y * cellSize), Quaternion.identity, transform);
      Instantiate(wallPrefab, new Vector3(width * cellSize, cellSize / 2f, y * cellSize), Quaternion.identity, transform);
    }
    wallPrefab.name = $"wall";

    // walls in a maze
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        var cellPosition = new Vector3(x * cellSize, 0, y * cellSize);
        // Instantiate(floorPrefab, cellPosition, Quaternion.identity, transform);
        if (grid[x, y].WallLeft)
        {
          var wallPos = cellPosition + new Vector3(-cellSize / 2f, cellSize / 2f, 0);
          Instantiate(wallPrefab, wallPos, Quaternion.Euler(0, 90, 0), transform);
        }
        if (grid[x, y].WallFront)
        {
          var wallPos = cellPosition + new Vector3(0, cellSize / 2f, cellSize / 2f);
          Instantiate(wallPrefab, wallPos, Quaternion.Euler(0, 90, 0), transform);
        }
        // Заглушки для внешних границ (Право и Низ лабиринта)
        if (x == width - 1 && grid[x, y].WallRight)
        {
          var wallPos = cellPosition + new Vector3(cellSize / 2f, cellSize / 2f, 0);
          Instantiate(wallPrefab, wallPos, Quaternion.Euler(0, 90, 0), transform);
        }
        if (y == 0 && grid[x, y].WallBack)
        {
          var wallPos = cellPosition + new Vector3(0, cellSize / 2f, -cellSize / 2f);
          Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
        }

        if (GetWallCount(grid[x, y]) == 3 && (x != 0 || y != 0))
        {
          Instantiate(chestPrefab, cellPosition + new Vector3(0, .5f, 0), Quaternion.identity, transform);
        }
      }
    }
  }

  private int GetWallCount(MazeCell cell)
  {
    int count = 0;
    if (cell.WallLeft) count++;
    if (cell.WallRight) count++;
    if (cell.WallFront) count++;
    if (cell.WallBack) count++;
    return count;
  }
}
