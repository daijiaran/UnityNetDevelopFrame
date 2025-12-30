using UnityEngine;
using GameShared.Net;

public static class VectorExtensions
{
    // 将 NetVector3 转为 Unity Vector3
    public static Vector3 ToUnity(this NetVector3 netVec)
    {
        return new Vector3(netVec.X, netVec.Y, netVec.Z);
    }

    // 将 Unity Vector3 转为 NetVector3
    public static NetVector3 ToNet(this Vector3 unityVec)
    {
        return new NetVector3(unityVec.x, unityVec.y, unityVec.z);
    }
}