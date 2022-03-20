using UnityEngine;

public class Map : ScriptableObject
{
    public int[] map;
    public Vector3[] positions;
    public Vector3[] path;
    public int[] rotations;
    public int[] objects;
    
    public void UpdateData(int[] _map, Vector3[] _positions, Vector3[] _path, int[] _rotations, int[] _objects)
    {
        map = _map;
        positions = _positions;
        path = _path;
        rotations = _rotations;
        objects = _objects;
    }
     
}
