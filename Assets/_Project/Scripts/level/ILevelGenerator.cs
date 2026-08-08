using UnityEngine;

public interface ILevelGenerator
{
    public void Generate(MazeGenerator maze);
    public void SpawnObjects(MazeGenerator maze);
}