using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class MapCreator : MonoBehaviour, ISerializationCallbackReceiver
{
    private static float tileSize = 15f;

    [SerializeField, Range(20, 100)]
    private int newMapTilesCount = 40;

    private List<Map> maps;
    private Map level;
    private int[] tiles;
    private int[] objects;
    private Vector3[] positions;
    private int[] rotations;
    private MapSpawner mapSpawner;

    private void LoadAllMaps()
    {
        maps = Resources.LoadAll<Map>("Maps").ToList();
    }

    private void CreateRandomMap(bool isNew)
    {
        if(!isNew && maps.Count == 0) isNew = true;

        tiles = new int[newMapTilesCount];
        objects = new int[newMapTilesCount];

        tiles[0] = 0;
        for (int i = 1; i < newMapTilesCount - 1; i++)
        {
            tiles[i] = Random.Range(0, 2);
            if(tiles[i] == 0)
            {
                objects[i] = Random.Range(0, 2);
            }
        }
        tiles[newMapTilesCount - 1] = 2;

        GenerateDataFromMap(isNew);
    }

    private void GenerateDataFromMap(bool isNew)
    {
        positions = new Vector3[newMapTilesCount];
        rotations = new int[newMapTilesCount];

        int rotation = 0;
        Vector3 position = Vector3.zero;
        for (int i = 1; i < newMapTilesCount; i++)
        {
            if((TileType)tiles[i-1] == TileType.Line)
            {
                if(rotation == 0 || rotation == 270)
                {
                    position = MoveForward(position);
                }
                else if(rotation == 180)
                {
                    position = MoveUp(position);
                }
                else
                {
                    position = MoveDown(position);
                }
                rotation = rotations[i-1];
            }
            else
            {
                switch(rotation)
                {
                    case 0:
                        rotation = 180;
                        position = MoveUp(position);
                        break;

                    case 180:
                        rotation = 270;
                        position = MoveForward(position);
                        break;

                    case 270:
                        rotation = 90;
                        position = MoveDown(position);
                        break;

                    case 90:
                        rotation = 0;
                        position = MoveForward(position);
                        break;
                }
            }
            
            rotations[i] = rotation;
            positions[i] = position;
        }

        CreateOrUpdateScriptableObject(isNew);
    }

    void CreateOrUpdateScriptableObject(bool isNew)
    {
        if(isNew)
        {
            Map newMap = ScriptableObject.CreateInstance<Map>();
            newMap.UpdateData(tiles, positions, rotations, objects);
            string assetPath = $"Assets/Resources/Maps/Map {maps.Count}.asset";
            AssetDatabase.CreateAsset(newMap, assetPath);
            level = newMap;
        }
        else
        {
            level.UpdateData(tiles, positions, rotations, objects);
        }
        LoadAllMaps();
        Level = $"Map {maps.IndexOf(level)}";
        SpawnMap();
    }

    private void SpawnMap()
    {
        if(mapSpawner == null) mapSpawner = FindObjectOfType<MapSpawner>();
        LoadAllMaps();
        UpdateCurrentLevel();
    }

    private void DeleteMap()
    {
        int levelIndex = maps.IndexOf(level);
        string assetPath = $"Assets/Resources/Maps/Map {levelIndex}.asset";
        AssetDatabase.DeleteAsset(assetPath);
        AssetDatabase.SaveAssets();

        
        for (int i = levelIndex + 1; i < maps.Count; i++)
        {
            assetPath =  $"Assets/Resources/Maps/Map {i}.asset";
            AssetDatabase.RenameAsset(assetPath, $"Map {i-1}.asset");
            AssetDatabase.SaveAssets();
        }
        LoadAllMaps();

        if(maps.Count > 0)
        {
            if(maps.Count <= levelIndex)
            {
                levelIndex = maps.Count - 1;
            }
            level = maps[levelIndex];
            Level = $"Map {levelIndex}";
        }
        else
        {
            Level = null;
            mapSpawner.DeleteMap();
        }
    }

    private void UpdateCurrentLevel()
    {
        if(maps != null && maps.Count > 0)
        {
            int levelIndex = maps.IndexOf(level);
            mapSpawner.currentLevel = levelIndex;
            
            mapSpawner.SpawnMap();
        }
    }
    
    private Vector3 MoveForward(Vector3 currentPos)
    {
        return new Vector3(currentPos.x, currentPos.y, currentPos.z + tileSize);
    }
    private Vector3 MoveDown(Vector3 currentPos)
    {
        return new Vector3(currentPos.x + tileSize, currentPos.y, currentPos.z);
    }
    private Vector3 MoveUp(Vector3 currentPos)
    {
        return new Vector3(currentPos.x - tileSize, currentPos.y, currentPos.z);
    }

    #region Editor

#if UNITY_EDITOR

        #region Popup List

        // Popup List
        public static List<string> TMPList;
        [HideInInspector]
        public List<string> PopupList;

        [ListToPopup(typeof(MapCreator), "TMPList")]
        public string Level;
        public int thisMapTilesCount;

        public void OnBeforeSerialize()
        {
            LoadAllMaps();
            PopupList = maps.OrderBy(q => int.Parse(Regex.Match(q.name, @"\d+").Value)).Select(q => q.name).ToList();
            TMPList = PopupList;
        }
        public void OnAfterDeserialize() {}

        private void OnValidate()
        {
            if(maps != null && maps.Count > 0)
            {
                
                Map newLevel = maps[int.Parse(Regex.Match(Level, @"\d+").Value)];
                if(level != newLevel)
                {
                    level = newLevel;
                    SpawnMap();
                    thisMapTilesCount = level.map.Length;
                }
            }
        }

    #endregion


        [CustomEditor(typeof(MapCreator)), CanEditMultipleObjects]
        public class MapCreatorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                MapCreator mapCreator = (MapCreator)target;

                EditorGUILayout.Space();
                // EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);

                if(GUILayout.Button("Create New Map"))
                {
                    mapCreator.CreateRandomMap(true);
                }

                EditorGUILayout.Space();

                if(GUILayout.Button("Delete This Map"))
                {
                    mapCreator.DeleteMap();
                }

                EditorGUILayout.Space();
                
                if(GUILayout.Button("Generate New Data For This Map"))
                {
                    mapCreator.CreateRandomMap(false);
                }
            }
        }

#endif

    #endregion

}

#region TileType enum

public enum TileType
    {
        Line = 0,
        Curve,
        FinishLine
    }

#endregion TileType enum
