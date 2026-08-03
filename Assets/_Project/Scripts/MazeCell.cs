public class MazeCell
{
  public int X { get; }
  public int Y { get; }
  public bool Visited { get; set; } = false;

  public bool WallLeft { get; set; } = true;
  public bool WallRight { get; set; } = true;
  public bool WallFront { get; set; } = true;
  public bool WallBack { get; set; } = true;

  public MazeCell(int x, int y)
  {
    X = x;
    Y = y;
  }
}