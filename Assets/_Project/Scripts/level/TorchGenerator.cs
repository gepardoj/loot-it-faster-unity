using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class TorchGenerator : MonoBehaviour, ILevelGenerator
{
  private static float TORCH_OFFSET = 1.2f;

  [SerializeField] private GameObject torchPrefab;

  public void Generate(MazeGenerator maze)
  {
  }

  public void SpawnObjects(MazeGenerator maze)
  {
    for (int x = 0; x < maze.Width; x++)
    {
      for (int y = 0; y < maze.Height; y++)
      {
        var cell = maze.Grid[x, y];
        if (cell == CellType.Empty && Random.Range(1, 10) == 1)
        {
          try
          {
            var (dx, dy) = maze.GetNearestWall(x, y);
            var pos = new Vector3(x * maze.CellSize + dx * TORCH_OFFSET, -maze.CellSize / 10f, y * maze.CellSize + dy * TORCH_OFFSET);
            torchPrefab.name = $"torch dx = {dx}, dy = {dy}";
            var rotation = Quaternion.identity;
            if (dx == 1) rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            else if (dx == 0 && dy == 1) rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            else if (dx == -1) rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            Instantiate(torchPrefab, pos, rotation, transform);
          }
          catch (Exception) { }
        }
      }
    }
  }
}
