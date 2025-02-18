using UnityEngine;
using UnityEngine.AI;

public class WorldTile : MonoBehaviour
{
    /// <summary>
    /// Holds navigation mesh baked for this tile
    /// </summary>
    public NavMeshData NavMeshData;

    /// <summary>
    /// Tile size, XZ
    /// </summary>
    public Vector2 Size = new Vector2(180, 180);
}
