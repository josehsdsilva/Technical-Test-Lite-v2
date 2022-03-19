using UnityEngine;

public class Map : ScriptableObject
{
    public int[] map;
    public Vector3[] positions;
    public int[] rotations;
    public int[] objects;
    
    public void UpdateData(int[] _map, Vector3[] _positions, int[] _rotations, int[] _objects)
    {
        map = _map;
        positions = _positions;
        rotations = _rotations;
        objects = _objects;
    }
     
}
