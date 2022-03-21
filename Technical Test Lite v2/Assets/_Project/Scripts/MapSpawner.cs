using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
public class MapSpawner : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField]
    private List<GameObject> tiles;
    [SerializeField]
    private GameObject cube, wall;

    #endregion Serializable Fields

    #region Fields

    private List<GameObject> pathList, tilesList, objectsList;

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
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        SpawnMap();
    }

    #endregion Unity Methods

    #region Read Map and instantiate it
    public void SpawnMap()
    {
        maps = Resources.LoadAll<Map>("Maps").ToList();

        ReadMap(maps[currentLevel]);
        DeleteMap();
        SpawnTiles();
        // SpawnPath();
        SpawnObjects();
    }

    private void ReadMap(Map _map)
    {
        map = _map;
        mapLength = _map.map.Length;
    }

    public void DeleteMap()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            #if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutine(Destroy(transform.GetChild(i).gameObject), this);
            #else
                Destroy(transform.GetChild(i).gameObject);
            #endif
        }
        tilesList?.Clear();
        objectsList?.Clear();
    }

    private void SpawnTiles()
    {
        tilesList = new List<GameObject>();
        for (int i = 0; i < mapLength; i++)
        {
            InstantiateTile((TileType)map.map[i], map.positions[i], map.rotations[i]);
        }
    }

    private void SpawnPath()
    {
        objectsList = new List<GameObject>();
        for (int i = 0; i < map.path.Length; i++)
        {
            InstantiateCube(map.path[i], 0);
        }
    }

    private void SpawnObjects()
    {
        objectsList = new List<GameObject>();

        for (int i = 0; i < mapLength; i++)
        {
            if(map.objects[i] > 0) InstantiateObjects(i);
        }
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
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

    private void InstantiateObjects(int i)
    {
        Vector3 position = map.positions[i] + new Vector3(0, cubetHeight, 0);
        int rotation = map.rotations[i] == 0 || map.rotations[i] == 270 ? 0 : 90;
        int forwardOrBack = Random.Range(0, 2) * 2 - 1;

        if(map.objects[i] == 1)
        {
            InstantiateCube(position, rotation);
        }
        else if(map.objects[i] == 2)
        {
            InstantiateCube(position, rotation);
            InstantiateSingleCubeWall(position, rotation, forwardOrBack);
        }
        else if(map.objects[i] == 3)
        {
            InstantiateCube(position, rotation);
            InstantiateWall(position, rotation, forwardOrBack);
        }
        else if(map.objects[i] == 4)
        {
            InstantiateSingleCubeWall(position, rotation, forwardOrBack);
        }
        else if(map.objects[i] == 5)
        {
            InstantiateWall(position, rotation, forwardOrBack);
        }
    }

    private void InstantiateCube(Vector3 position, int rotation)
    {
        GameObject go = Instantiate(cube, position, Quaternion.Euler(0, rotation, 0), transform);
        objectsList.Add(go);
    }
    private void InstantiateSingleCubeWall(Vector3 position, int rotation, int forwardOrBack)
    {
        
        GameObject go = Instantiate(wall, GetRelativePosition(position, rotation, forwardOrBack), Quaternion.Euler(0, rotation, 0), transform);
        objectsList.Add(go);
    }
    private void InstantiateWall(Vector3 position, int rotation, int forwardOrBack)
    {
        GameObject go;
        for (int i = -4; i < 5; i++)
        {
            go = Instantiate(wall, GetRelativeSidePosition(position, rotation, i, forwardOrBack), Quaternion.Euler(0, rotation, 0), transform);
            objectsList.Add(go);
        }
    }

    private Vector3 GetRelativePosition(Vector3 position, int rotation, int forwardOrBack)
    {
        Vector3 forward = new Vector3(0, 0, 2);
        Vector3 upOrDown = new Vector3(2, 0, 0);
        forward *= forwardOrBack;

        return rotation == 0 ? position + forward : position + upOrDown;
    }
    private Vector3 GetRelativeSidePosition(Vector3 position, int rotation, int side, int forwardOrBack)
    {
        Vector3 forward = new Vector3(0, 0, 2);
        Vector3 upOrDown = new Vector3(2, 0, 0);
        forward *= forwardOrBack;

        return rotation == 0  ? position + forward + (upOrDown / 3) * side : position + upOrDown + (forward / 3) * side;
    }

    #endregion Read Map and instantiate it

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
