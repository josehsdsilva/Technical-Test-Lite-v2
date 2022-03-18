using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> tiles;
    [SerializeField]
    private GameObject player, cube, wall;

    private List<GameObject> trackObjects;

    private float tileSize = 15f;
    private Vector3 nextPosition = Vector3.zero;
    private int mapLength = 40;
    private int[] track;
    private int[] map;
    private int[] objects;
    private Vector3[] positions;
    public Vector3[] Positions => positions;
    private int[] rotations;

    void Awake()
    {
        trackObjects = new List<GameObject>();
        CreateRandomTrack();
        ReadMap(track);
        SpawnTrack();
        SpawnPlayer();
        SpawnObjects();
    }

    void CreateRandomTrack()
    {
        track = new int[mapLength];
        objects = new int[mapLength];
        track[0] = 0;
        track[mapLength - 1] = 2;
        for (int i = 1; i < mapLength - 1; i++)
        {
            track[i] = Random.Range(0, 2);
            if(track[i] == 0)
            {
                objects[i] = Random.Range(0, 2);
            }
        }
    }

    private void ReadMap(int[] _map)
    {
        map = _map;
        positions = new Vector3[mapLength];
        rotations = new int[mapLength];

        int rotation = 0;
        Vector3 position = Vector3.zero;
        for (int i = 0; i < mapLength; i++)
        {
            if(i == 0) continue;

            if((TileType)map[i-1]  == TileType.Line)
            {
                if(rotations[i-1] == 0 || rotations[i-1] == 270)
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
                switch(rotations[i-1])
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
    }

    private void SpawnTrack()
    {
        for (int i = 0; i < mapLength; i++)
        {
            InstantiateTile((TileType)map[i], positions[i], rotations[i]);
        }
    }

    private void InstantiateTile(TileType tile, Vector3 pos, int rotation)
    {
        // Vertical or Horizontal rotation for the lines
        if(tile != TileType.Curve) rotation = rotation == 0 || rotation == 270 ? 0 : 90;

        // Hidding a problem with the assets
        else pos += new Vector3(0, 0.04f, 0); 

        GameObject go = Instantiate(tiles[(int)tile], pos, Quaternion.Euler(0f, rotation, 0f), transform);
        trackObjects.Add(go);
    }
    private void SpawnPlayer()
    {
        Instantiate(player, new Vector3(0, 1.5f, 0), Quaternion.identity);
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < mapLength; i++)
        {
            if(objects[i] > 0) InstantiateObjects(i);
        }
    }

    private void InstantiateObjects(int i)
    {
        float rotation = rotations[i] == 0 || rotations[i] == 270 ? 0 : 90;
        Vector3 position = positions[i] + new Vector3(0, 0.25f, 0);

        Instantiate(cube, position, Quaternion.Euler(0, rotation, 0));
    }

    private Vector3 MoveForward(Vector3 currentPos)
    {
        return new Vector3(currentPos.x, currentPos.y, currentPos.z + 15);
    }
    private Vector3 MoveDown(Vector3 currentPos)
    {
        return new Vector3(currentPos.x + 15, currentPos.y, currentPos.z);
    }
    private Vector3 MoveUp(Vector3 currentPos)
    {
        return new Vector3(currentPos.x - 15, currentPos.y, currentPos.z);
    }
}

public enum TileType
{
    Line = 0,
    Curve,
    FinishLine
}

