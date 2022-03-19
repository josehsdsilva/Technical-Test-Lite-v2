using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

public class MapSpawner : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField]
    private List<GameObject> tiles;
    [SerializeField]
    private GameObject cube, wall;

    #endregion Serializable Fields

    #region Fields

    private List<GameObject> tilesList, objectsList;

    // Helper variables
    private static float cubetHeight = 0.25f;

    // Map variables
    private List<Map> maps;
    private Map map;
    public Map Level => map;
    [HideInInspector]
    public int currentLevel = 0;

    private int mapLength = 40;

    #endregion Fields

    #region Unity Methods

    void Awake()
    {
        SpawnMap();
    }

    #endregion Unity Methods

    #region Read Map and Instantiate
    private void ReadMap(Map _map)
    {
        map = _map;
        mapLength = _map.map.Length;
    }

    public void SpawnMap()
    {
        maps = Resources.LoadAll<Map>("Maps").ToList();

        ReadMap(maps[currentLevel]);
        DeleteMap();
        SpawnTiles();
        SpawnObjects();
    }

    public void DeleteMap()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            EditorCoroutineUtility.StartCoroutine(Destroy(transform.GetChild(i).gameObject), this);
        }
        tilesList?.Clear();
        objectsList?.Clear();
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }

    private void SpawnTiles()
    {
        tilesList = new List<GameObject>();
        for (int i = 0; i < mapLength; i++)
        {
            InstantiateTile((TileType)map.map[i], map.positions[i], map.rotations[i]);
        }
    }

    private void InstantiateTile(TileType tile, Vector3 pos, int rotation)
    {
        // Vertical or Horizontal rotation for the lines
        if(tile != TileType.Curve) rotation = rotation == 0 || rotation == 270 ? 0 : 90;

        // Hidding a problem with the Curve tile beeing a bit lower then the rest
        else pos += new Vector3(0, 0.04f, 0); 

        GameObject go = Instantiate(tiles[(int)tile], pos, Quaternion.Euler(0f, rotation, 0f), transform);
        tilesList.Add(go);
    }

    private void SpawnObjects()
    {
        objectsList = new List<GameObject>();

        for (int i = 0; i < mapLength; i++)
        {
            if(map.objects[i] > 0) InstantiateObjects(i);
        }
    }

    private void InstantiateObjects(int i)
    {
        float rotation = map.rotations[i] == 0 || map.rotations[i] == 270 ? 0 : 90;
        Vector3 position = map.positions[i] + new Vector3(0, cubetHeight, 0);

        GameObject go = Instantiate(cube, position, Quaternion.Euler(0, rotation, 0), transform);
        objectsList.Add(go);
    }

    #endregion Read Map and Instantiate

    #region Editor

#if UNITY_EDITOR

        [CustomEditor(typeof(MapSpawner)), CanEditMultipleObjects]
        public class MapCreatorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                MapSpawner mapSpawner = (MapSpawner)target;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);

                if(GUILayout.Button("Spawn Map"))
                {
                    mapSpawner.SpawnMap();
                    
                }
                if(GUILayout.Button("Delete Map"))
                {
                    mapSpawner.DeleteMap();
                }
            }
        }

#endif

    #endregion

}
