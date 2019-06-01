using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

public class 同步transform : 同步系统
{
    public 同步transform()
    {
        同步类型 = 数据类型.transform;
    }

    [NonSerialized] public TransformSerializable 本地数据 = new TransformSerializable();
    [NonSerialized] public TransformSerializable 网络数据 = null;

    private void Update()
    {
        if (网络数据 != null)
        {
            transform.position = 网络数据.Position;
            transform.eulerAngles = 网络数据.EulerAngles;
            网络数据 = null;
        }
    }

    public override void 本地更新(byte[] 数据)
    {
        网络数据 = BTC<TransformSerializable>(数据);
    }
    public override void 网络更新()
    {
        本地数据.Position = transform.position;
        本地数据.EulerAngles = transform.eulerAngles;
        网络系统.发送信息(GetComponent<网络物体身份>().网络标识.发送(同步类型), CTB(本地数据));
    }
}

[Serializable]
public class TransformSerializable
{
    private Vector3Serializer 位置;
    private Vector3Serializer 旋转;

    public Vector3 Position
    {
        get { return 位置.V; }
        set { 位置.V = value; }
    }

    public Vector3 EulerAngles
    {
        get { return 旋转.V; }
        set { 旋转.V = value; }
    }
}
[Serializable]
public struct Vector3Serializer
{
    public float x;
    public float y;
    public float z;

    public Vector3Serializer(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 V
    {
        get { return new Vector3(x, y, z); }
        set
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }
}