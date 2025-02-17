using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

/// <summary>
/// Simple infinite tile map. Tracks main camera position and arranges up to 4 tiles around it filling the whole space.
/// Also manages navmesh and links.
/// </summary>
public class WorldTileMap : MonoBehaviour
{
    private class Cell
    {
        public bool Active;
        public bool Used;
        public Vector2Int Coordinates;
        public int Index;
        public WorldTile WorldTile;
        public NavMeshDataInstance NavMeshDataInstance;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    private const int MaxTiles = 4;
    private const int MaxActiveAreaWidth = 2;
    private const int MaxActiveAreaHeight = 2;

    // TODO: extend to support variety of tiles?
    public WorldTile WorldTileTemplate;

    private readonly List<WorldTile> _tiles = new List<WorldTile>(MaxTiles);
    private readonly List<Cell> _cells = new List<Cell>(MaxTiles);
    private readonly List<NavMeshLinkInstance> _navMeshLinks = new List<NavMeshLinkInstance>();

    private RectInt _activeAreaRect = new RectInt();

    private Vector2 _tileSize;
    private Vector2 _halfTileSize;
    private Vector2 _quarterTileSize;
    private Vector2 _gridOffset;

    private void Awake()
    {
        _tileSize = WorldTileTemplate.Size;
        _halfTileSize = _tileSize / 2f;
        _quarterTileSize = _tileSize / 4f;
        _gridOffset = -_halfTileSize;

        for (int i = 0; i < MaxTiles; ++i)
        {
            _cells.Add(new Cell() { Active = false, Index = -1, WorldTile = null });
        }
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            var child = transform.GetChild(i);

            var worldTile = child.GetComponent<WorldTile>();
            if (worldTile != null)
            {
                _tiles.Add(worldTile);
            }
        }

        foreach (var tile in _tiles)
        {
            tile.gameObject.SetActive(false);
        }

        UpdateActiveArea(true);
    }

    private void Update()
    {
        UpdateActiveArea();
    }

    private void OnDestroy()
    {
        _navMeshLinks.ForEach(navMeshLink => navMeshLink.Remove());
        _navMeshLinks.Clear();

        foreach (var cell in _cells)
        {
            if (cell.Active)
            {
                DeactivateCell(cell);
            }
        }

        _tiles.Clear();
        _cells.Clear();
    }

    private void UpdateActiveArea(bool force = false)
    {
        var mainCamera = Camera.main;

        var cameraPosition = mainCamera.transform.position;
        var cameraPositionXZ = new Vector2(cameraPosition.x, cameraPosition.z) - _gridOffset;

        var cameraAreaMin = cameraPositionXZ - _quarterTileSize;
        var cameraAreaMax = cameraPositionXZ + _quarterTileSize;
        var activeAreaMin = new Vector2Int(Mathf.FloorToInt(cameraAreaMin.x / _tileSize.x), Mathf.FloorToInt(cameraAreaMin.y / _tileSize.y));
        var activeAreaMax = new Vector2Int(Mathf.FloorToInt(cameraAreaMax.x / _tileSize.x), Mathf.FloorToInt(cameraAreaMax.y / _tileSize.y));

        var activeAreaRect = new RectInt(activeAreaMin.x, activeAreaMin.y, activeAreaMax.x - activeAreaMin.x + 1, activeAreaMax.y - activeAreaMin.y + 1);

        Debug.Assert(activeAreaRect.width <= MaxActiveAreaWidth);
        Debug.Assert(activeAreaRect.height <= MaxActiveAreaHeight);

        ActivateArea(activeAreaRect, force);
    }

    private void ActivateArea(RectInt rect, bool force = false)
    {
        if (_activeAreaRect.Equals(rect) && !force)
            return;

        _navMeshLinks.ForEach(navMeshLink => navMeshLink.Remove());
        _navMeshLinks.Clear();

        foreach (var cell in _cells)
        {
            cell.Used = false;

            if (rect.Contains(cell.Coordinates))
                continue;

            if (cell.Active)
            {
                DeactivateCell(cell);
            }
        }

        for (int cellIndex = 0; cellIndex < MaxTiles; ++cellIndex)
        {
            var x = cellIndex % MaxActiveAreaWidth;
            var y = cellIndex / MaxActiveAreaWidth;

            var coordinates = rect.min + new Vector2Int(x, y);

            bool activate = rect.Contains(coordinates);

            if (activate)
            {
                var activeCell = _cells.Find(cell => cell.Active && cell.Coordinates.Equals(coordinates));
                if (activeCell != null)
                {
                    activeCell.Used = true;
                    activeCell.Index = cellIndex;

                    continue;
                }
            }

            var freeCell = _cells.Find(cell => !cell.Active && !cell.Used);
            if (freeCell != null)
            {
                freeCell.Used = true;
                freeCell.Index = cellIndex;
                freeCell.Coordinates = coordinates;

                if (activate)
                {
                    ActivateCell(freeCell);
                }

                continue;
            }

            throw new Exception("No free cell");
        }

        Debug.Assert(_cells.TrueForAll(cell => cell.Used));

        _cells.Sort((lhs, rhs) => lhs.Index - rhs.Index);

        // NOTE: those pairs match cell indices in cells list
        AddNavMeshLink(0, 1);
        AddNavMeshLink(0, 2);
        AddNavMeshLink(1, 3);
        AddNavMeshLink(2, 3);

        _activeAreaRect = rect;
    }

    private void ActivateCell(Cell cell)
    {
        // each newly activated tile will be rotated according to this "function" result
        // using coordinates as input gives deterministic output as opposed to random
        var rotationIndex = (Mathf.Abs(cell.Coordinates.x) * 3 + Mathf.Abs(cell.Coordinates.y) * 7) % 4;

        cell.Position = new Vector3(cell.Coordinates.x * _tileSize.x, 0f, cell.Coordinates.y * _tileSize.y);
        cell.Rotation = Quaternion.AngleAxis(90f * rotationIndex, Vector3.up);

        if (_tiles.Count > 0)
        {
            cell.WorldTile = _tiles[_tiles.Count - 1];

            _tiles.RemoveAt(_tiles.Count - 1);
        }
        else
        {
            cell.WorldTile = Instantiate(WorldTileTemplate, transform);
        }

        cell.WorldTile.transform.SetPositionAndRotation(cell.Position, cell.Rotation);
        cell.WorldTile.gameObject.SetActive(true);

        cell.NavMeshDataInstance = NavMesh.AddNavMeshData(cell.WorldTile.NavMeshData, cell.Position, cell.Rotation);

        cell.Active = true;
    }

    private void DeactivateCell(Cell cell)
    {
        _tiles.Add(cell.WorldTile);

        cell.Active = false;

        cell.NavMeshDataInstance.Remove();

        cell.WorldTile.gameObject.SetActive(false);
        cell.WorldTile = null;
    }

    private void AddNavMeshLink(int fromCellIndex, int toCellIndex)
    {
        const float LinkMargin = 1f;

        var from = _cells[fromCellIndex];
        var to = _cells[toCellIndex];

        if (!from.Active || !to.Active)
            return;

        var delta = to.Position - from.Position;

        delta.Normalize();

        var middle = Vector3.Lerp(from.Position, to.Position, 0.5f);

        var navMeshLinkData = new NavMeshLinkData()
        {
            agentTypeID = 0,
            area = 0,
            bidirectional = true,
            costModifier = -1,
            endPosition = delta * LinkMargin,
            startPosition = -delta * LinkMargin,
            width = Mathf.Max(_tileSize.x, _tileSize.y),
        };

        var navMeshLink = NavMesh.AddLink(navMeshLinkData, middle, Quaternion.identity);

        if (navMeshLink.valid)
        {
            navMeshLink.owner = this;

            _navMeshLinks.Add(navMeshLink);
        }
        else
        {
            Debug.LogWarning("Failed to add navmesh link");
        }
    }
}
