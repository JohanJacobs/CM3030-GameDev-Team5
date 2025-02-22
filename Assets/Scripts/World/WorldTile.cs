/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

WorldTile.cs

*/

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
