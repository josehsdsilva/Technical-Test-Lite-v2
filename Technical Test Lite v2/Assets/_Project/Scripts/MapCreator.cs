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

    private async void CreateRandomMap(bool isNew)
    {
        if(!isNew && maps.Count == 0)
        {
            // if there's no maps create a new one
            isNew = true;
        }
        else if(!isNew && level == null)
        {
            // if level is not asigned get the current level data
            if(mapSpawner == null) mapSpawner = FindObjectOfType<MapSpawner>();
            level = maps[mapSpawner.currentLevel];
        }

        tiles = new int[newMapTilesCount];
        objects = new int[newMapTilesCount];

        bool first = true;

        // Create basic level data
        tiles[0] = 0;
        for (int i = 1; i < newMapTilesCount - 1; i++)
        {
            tiles[i] = Random.Range(0, 2);
            if(tiles[i] == 0)
            {
                if(first)
                {
                    objects[i] = 1;
                    first = false;
                    continue;
                }
                int random = Random.Range(0, 100);
                if(random > 50)
                {
                    objects[i] = 1;
                }
                else if(random > 40)
                {
                    objects[i] = 2;
                }
                else if(random > 20)
                {
                    objects[i] = 3;
                }
                else if(random > 10)
                {
                    objects[i] = 4;
                }
                else
                {
                    objects[i] = 5;
                }
                
            }
        }
        tiles[newMapTilesCount - 1] = 2;

        ExpandMapData(isNew);
    }

    // Expand the map data
    private void ExpandMapData(bool isNew)
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

    // Create (or Update) the map data (on the) scriptable object
    void CreateOrUpdateScriptableObject(bool isNew)
    {
        if(isNew)
        {
            Map newMap = ScriptableObject.CreateInstance<Map>();
            string assetPath = $"Assets/Resources/Maps/Map {maps.Count}.asset";
            AssetDatabase.CreateAsset(newMap, assetPath);
            level = newMap;
        }
        level.UpdateData(tiles, positions, CalculatePath(), rotations, objects);
        LoadAllMaps();
        Level = $"Map {maps.IndexOf(level)}";
        ShowLevelOnEditor();
    }

    private Vector3[] CalculatePath()
    {
        List<Vector3> path = positions.ToList();
        int counter = 0;
        int listCount = 0;
        for (int i = 1; i < rotations.Length; i++)
        {
            if(rotations[i] != rotations[i-1])
            {
                path[i-1] = path[i-1] + ((path[i]-path[i-1]) / 2);
            }
        }
        for (int i = 0; i < rotations.Length; i++)
        {
            if((TileType)tiles[i] == TileType.Line)
            {
                Vector3 oldPos = path[i + counter];
                if(rotations[i] == 0 || rotations[i] == 270)
                {
                    path.Insert(i + 1 + counter, oldPos - (oldPos - MoveForward(oldPos))/2);
                }
                else if(rotations[i] == 180)
                {
                    path.Insert(i + 1 + counter, oldPos - (oldPos - MoveUp(oldPos))/2);
                }
                else
                {
                    path.Insert(i + 1 + counter, oldPos - (oldPos - MoveDown(oldPos))/2);
                }
                counter++;
            }
        }
        counter = 0;
        listCount = path.Count - 1;
        for (int i = 0; i < listCount; i++)
        {
            path.Insert(i + 1 + counter, path[i + counter] + path[i + 1 + counter] - path[i + counter]);
            counter++;
        }
        // counter = 0;
        // listCount = path.Count - 1;
        // for (int i = 0; i < listCount; i++)
        // {
        //     path.Insert(i + 1 + counter, path[i + counter] + path[i + 1 + counter] - path[i + counter]);
        //     counter++;
        // }
        return path.ToArray();
    }

    private void ShowLevelOnEditor()
    {
        if(mapSpawner == null) mapSpawner = FindObjectOfType<MapSpawner>();

        LoadAllMaps();

        if(maps != null && maps.Count > 0)
        {
            int levelIndex = maps.IndexOf(level);
            mapSpawner.currentLevel = levelIndex;
            
            mapSpawner.SpawnMap();
        }
    }

    private void DeleteMapData()
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
                    ShowLevelOnEditor();
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

                if(GUILayout.Button("Create New Map"))
                {
                    mapCreator.CreateRandomMap(true);
                }

                EditorGUILayout.Space();

                if(GUILayout.Button("Delete This Map"))
                {
                    mapCreator.DeleteMapData();
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
