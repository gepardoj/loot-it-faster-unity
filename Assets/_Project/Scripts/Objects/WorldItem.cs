using UnityEngine;

public class WorldItem : MonoBehaviour
{
  public Item Item { get; private set; }

  public void SetItem(Item item)
  {
    Item = item;
  }
}