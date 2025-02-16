using UnityEngine;
using UnityEngine.AI;

public class WorldTile : MonoBehaviour
{
    public NavMeshData NavMeshData;
    public bool FollowTransform = true;
    public Vector2 Size = new Vector2(180, 180);

    private NavMeshDataInstance _navMeshDataInstance;
}
