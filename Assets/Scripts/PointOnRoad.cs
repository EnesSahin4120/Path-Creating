using UnityEngine;

public class PointOnRoad 
{
    public Vector3 pos;
    public Quaternion rot;

    public PointOnRoad(Vector3 pos, Quaternion rot)
    {
        this.pos = pos;
        this.rot = rot;
    }
    public PointOnRoad(Vector3 pos, Vector3 forward)
    {
        this.pos = pos;
        this.rot = Quaternion.LookRotation(forward);
    }

    public Vector3 WorldPosition(Vector3 localPos)
    {
        return pos + rot * localPos;
    }

    public Vector3 WorldAxis(Vector3 localPos)  
    {
        return rot * localPos;
    }
}
