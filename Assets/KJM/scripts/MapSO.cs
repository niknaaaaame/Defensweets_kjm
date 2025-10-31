using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Map")]
public class MapSO : ScriptableObject
{
    public int mapWidth = 29;
    public int mapHeight = 12;
    public int tileSize = 60;

    public Vector2Int startPos = new Vector2Int(0, 4);
    public Vector2Int goalPos = new Vector2Int(28, 4);
}
