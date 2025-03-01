/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

WorldTileEditor.cs

*/

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(WorldTile))]
public class WorldTileEditor : Editor
{
    public Vector2 Size = new Vector2(180, 180);

    private WorldTile TargetWorldTile => (WorldTile)target;

    private SerializedProperty _propSize;
    private SerializedProperty _propNavMeshData;

    private NavMeshData _overrideNavMeshData;

    public void OnEnable()
    {
        _propSize = serializedObject.FindProperty("Size");
        _propNavMeshData = serializedObject.FindProperty("NavMeshData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(_propNavMeshData);
        GUI.enabled = true;
        EditorGUILayout.PropertyField(_propSize);

        EditorGUILayout.Space();

        OnInspectorGUIPrefab(TargetWorldTile.gameObject);

        serializedObject.ApplyModifiedProperties();
    }

    private void OnInspectorGUIPrefab(GameObject gameObject)
    {
        var prefabInstanceRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
        if (prefabInstanceRoot == null)
        {
            AddHelpMessageLabel("Select prefab instance inside active scene");
            return;
        }

        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(prefabInstanceRoot);

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Prefab", prefab, typeof(WorldTile), false);
        GUI.enabled = true;

        var prefabPath = AssetDatabase.GetAssetPath(prefab);

        if (string.IsNullOrWhiteSpace(prefabPath))
        {
            if (prefab == null)
                AddHelpMessageLabel("Failed to detect prefab");
            else
                AddHelpMessageLabel("Save prefab first");
            return;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear Nav Mesh"))
        {
            OnClear(prefabInstanceRoot, prefab, prefabPath);
        }

        if (GUILayout.Button("Bake Nav Mesh"))
        {
            OnBake(prefabInstanceRoot, prefab, prefabPath);
        }

        GUILayout.EndHorizontal();

        _overrideNavMeshData = EditorGUILayout.ObjectField("Baked Nav Mesh", _overrideNavMeshData, typeof(NavMeshData), false) as NavMeshData;

        if (_overrideNavMeshData)
        {
            if (GUILayout.Button("Borrow Nav Mesh"))
            {
                OnBorrow(prefabInstanceRoot, prefab, prefabPath);
            }
        }
    }

    private void OnClear(GameObject instance, GameObject prefab, string prefabPath)
    {
        DestroyPrefabNavMeshData(prefabPath);

        AssetDatabase.SaveAssets();
    }

    private void OnBake(GameObject instance, GameObject prefab, string prefabPath)
    {
        var prefabWorldTile = prefab.GetComponent<WorldTile>();

        DestroyPrefabNavMeshData(prefabPath);

        var navMeshData = BuildNavMeshData(instance, prefabWorldTile.Size);

        AssetDatabase.AddObjectToAsset(navMeshData, prefab);

        prefabWorldTile.NavMeshData = navMeshData;

        AssetDatabase.SaveAssets();

        _overrideNavMeshData = navMeshData;
    }

    private void OnBorrow(GameObject instance, GameObject prefab, string prefabPath)
    {
        var prefabWorldTile = prefab.GetComponent<WorldTile>();

        if (prefabWorldTile.NavMeshData == _overrideNavMeshData)
            return;

        DestroyPrefabNavMeshData(prefabPath);

        var navMeshData = Instantiate(_overrideNavMeshData);

        AssetDatabase.AddObjectToAsset(navMeshData, prefab);

        prefabWorldTile.NavMeshData = navMeshData;

        AssetDatabase.SaveAssets();
    }

    private NavMeshData BuildNavMeshData(GameObject instance, Vector2 tileSize)
    {
        // TODO: are we really interested in all layers?
        int layerMask = ~0;
        // TODO: is it walkable?
        int defaultArea = 0;

        var sources = new List<NavMeshBuildSource>();
        var markups = new List<NavMeshBuildMarkup>();

        UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(instance.transform, layerMask, NavMeshCollectGeometry.RenderMeshes, defaultArea, markups, instance.scene, sources);

        var settings = NavMesh.GetSettingsByID(0);

        {
            // var t = typeof(NavMeshBuildSettings);
            // var fields = t.GetFields(BindingFlags.NonPublic);
            // foreach (var field in fields)
            // {
            //     Debug.Log(field.Name);
            // }

            var field = typeof(NavMeshBuildSettings).GetField("m_AccuratePlacement", BindingFlags.Instance | BindingFlags.NonPublic);

            object boxed = settings;

            field?.SetValue(boxed, -1);

            settings = (NavMeshBuildSettings)boxed;
        }

        // var bounds = new Bounds(Vector3.zero, new Vector3(tileSize.x, 100f, tileSize.y));
        var bounds = new Bounds(Vector3.zero, 1000.0f * Vector3.one);

        var navMeshData = UnityEngine.AI.NavMeshBuilder.BuildNavMeshData(settings, sources, bounds, instance.transform.position, instance.transform.rotation);

        navMeshData.name = "NavMesh";

        return navMeshData;
    }

    private void AddHelpMessageLabel(string message)
    {
        GUILayout.Label(message, EditorStyles.helpBox);
    }

    private void DestroyPrefabNavMeshData(string prefabPath)
    {
        foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(prefabPath))
        {
            if (asset is NavMeshData)
            {
                DestroyImmediate(asset, true);
            }
        }
    }
}
