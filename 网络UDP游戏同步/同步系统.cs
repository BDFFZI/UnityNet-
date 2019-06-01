using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Runtime.InteropServices;

public interface I数据同步
{
    void 本地更新(byte[] 数据);       
    void 网络更新();                  
}

public abstract class 同步系统 : MonoBehaviour, I数据同步
{
    [System.NonSerialized] public 数据类型 同步类型;
    private static BinaryFormatter 序列化 = new BinaryFormatter();

    protected IEnumerator 更新()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(网络系统.更新速度);
        while (true)
        {
            网络更新();
            yield return waitForSeconds;
        }
    }

    public abstract void 本地更新(byte[] 数据);   //用网络数据更新本地
    public abstract void 网络更新();              //本地数据发送到网络
    public virtual void 初始化()                  //启动网络更新
    {
        StartCoroutine("更新");
    }              

    public static byte[] CTB(object 数据)
    {
        MemoryStream stream = new MemoryStream();
        序列化.Serialize(stream, 数据);
        stream.Position = 0;
        byte[] 序列数据 = new byte[stream.Length];
        stream.Read(序列数据, 0, 序列数据.Length);
        stream.Close();
        return 序列数据;
    }
    public static T BTC<T>(byte[] 数据) where T : class
    {
        MemoryStream stream = new MemoryStream();
        stream.Write(数据, 0, 数据.Length);
        stream.Position = 0;
        T 反序列数据 = 序列化.Deserialize(stream) as T;
        stream.Close();
        return 反序列数据;
    }

    public static byte[] STB(object 数据)
    {
        int 数据大小 = Marshal.SizeOf(数据);
        System.IntPtr 缓存 = Marshal.AllocHGlobal(数据大小);
        byte[] 字节流 = new byte[数据大小];

        Marshal.StructureToPtr(数据, 缓存, false);
        Marshal.Copy(缓存, 字节流, 0, 数据大小);
        Marshal.FreeHGlobal(缓存);

        return 字节流;
    }
    public static T BTS<T>(byte[] 数据, int 数据大小) where T : struct
    {
        T type = new T();

        System.IntPtr 缓存 = Marshal.AllocHGlobal(数据大小);
        Marshal.Copy(数据, 0, 缓存, 数据大小);

        object 结构 = Marshal.PtrToStructure(缓存, type.GetType());
        Marshal.FreeHGlobal(缓存);

        return (T)结构;
    }
}