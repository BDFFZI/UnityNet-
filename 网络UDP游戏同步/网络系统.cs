using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using UnityEngine.UI;
using System;

public enum 数据类型
{
    active = -1,
    NULL = 0,
    transform = 1,
}

public enum 网络属性
{
    单机 = 0,
    联机 = 1,
}

public struct 全局ID
{
    public string 预制体路径;
    public int 网络ID;
    public int 编号;

    public 全局ID(string 预制体路径, int 网络ID, int 编号)
    {
        this.预制体路径 = 预制体路径;
        this.网络ID = 网络ID;
        this.编号 = 编号;
    }
}
public struct 全局状态
{
    public GameObject 物体;
    public bool 状态;

    public 全局状态(GameObject 物体, bool 状态)
    {
        this.物体 = 物体;
        this.状态 = 状态;
    }
}

[Serializable]
public struct 网络数据标识
{
    public int 网络ID;
    public int 类型;
    public int 编号;

    public 网络数据标识(int 网络ID, 数据类型 类型,int 编号)
    {
        this.网络ID = 网络ID;
        this.类型 = (int)类型;
        this.编号 = 编号;
    }

    public 网络数据标识 发送(数据类型 类型)
    {
        return new 网络数据标识(网络ID, 类型, 编号);
    }
}
//"192.168.137.1"       本地
//"192.168.31.121"      家
//"192.168.43.47"       手机热点
public class 网络系统 : MonoBehaviour
{
    public static 网络属性 属性 = 网络属性.联机;
    public static float 更新速度 = 0.06f;
    private static string[] 外端IP = new string[]
    {
        //"192.168.31.121"
        //"192.168.31.58"
        "192.168.137.1"
        //"192.168.43.47"
    };
    public static int 端口 = 6666;
    public static int 编号 = 1;
    public static int 服务器端口 = 5555;


    public static Socket 本地;
    public static Queue<全局ID> 未实例物体 = new Queue<全局ID>();
    public static Queue<全局状态> 未开关物体 = new Queue<全局状态>();

    public static Queue<string> 消息台 = new Queue<string>();
    public Transform 消息台t;
    private Text 文本框;
    private Scrollbar 滑动条;


    private void Awake()
    {
        if (属性 == 网络属性.单机)
            return;
        文本框 = 消息台t.GetChild(0).GetComponent<Text>();
        滑动条 = 消息台t.GetChild(1).GetComponent<Scrollbar>();

        本地 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        本地.Bind(new IPEndPoint(IPAddress.Any, 端口));

        new Thread(接受信息).Start();
        print("联机程序启动成功");
    }

    private void Update()
    {
        if (未实例物体.Count > 0)
        {
            全局ID 名称 = 未实例物体.Dequeue();
            网络物体身份.网络ID注册(Instantiate((GameObject)Resources.Load(名称.预制体路径)), 名称.网络ID, 名称.编号);
        }
        if (未开关物体.Count > 0)
        {
            全局状态 状态 = 未开关物体.Dequeue();
            状态.物体.SetActive(状态.状态);
        }

        if (消息台.Count != 0)
        {
            文本框.text += 消息台.Dequeue() + "\n";
            滑动条.value = 0;
        }       //UI显示调试信息
        if (消息台.Count != 0) Debug.Log(消息台.Dequeue());
    }

    public static void 发送信息(网络数据标识 ID, byte[] 信息)
    {
        byte[] IDt = 同步系统.STB(ID);
        byte[] 数据 = new byte[IDt.Length + 信息.Length];
        Buffer.BlockCopy(IDt, 0, 数据, 0, IDt.Length);
        Buffer.BlockCopy(信息, 0, 数据, IDt.Length, 信息.Length);

        foreach (string 地址 in 外端IP)
            本地.SendTo(数据, new IPEndPoint(IPAddress.Parse(地址), 服务器端口));
        print(ID.编号 + ID.网络ID + "发送" + (数据类型)ID.类型);
    }
    private static void 接受信息()
    {
        byte[] 信息 = new byte[1024 * 1024 * 3];
        EndPoint point = new IPEndPoint(IPAddress.Any, 0);
        print("开始接受信息");
        while (true)
        {
            int length = 本地.ReceiveFrom(信息, ref point);
            byte[] 信息t = new byte[length];
            Buffer.BlockCopy(信息, 0, 信息t, 0, length);
            new Thread(处理信息).Start(信息t);
        }
    }
    private static void 处理信息(object 信息)
    {
        byte[] 原数据 = (byte[])信息;
        byte[] 标识t = new byte[12];
        Buffer.BlockCopy(原数据, 0, 标识t, 0, 12);
        byte[] 数据t = new byte[原数据.Length - 12];
        Buffer.BlockCopy(原数据, 12, 数据t, 0, 原数据.Length - 12);

        网络数据标识 标识 = 同步系统.BTS<网络数据标识>(标识t, 标识t.Length);
        if (标识.类型 != 0)
        {
            for (int i = 0; i < 3 && !网络物体身份.网络物体清单.ContainsKey(标识.编号.ToString() + 标识.网络ID); i++)
            {
                Thread.Sleep(500);
            }
            网络物体身份.网络物体清单[标识.编号.ToString() + 标识.网络ID].同步工具[(数据类型)标识.类型].本地更新(数据t);
        }
        else
            未实例物体.Enqueue(new 全局ID(Encoding.Default.GetString(数据t), 标识.网络ID, 标识.编号));
        print(标识.编号.ToString() + 标识.网络ID + "接受" + (数据类型)标识.类型);
    }
}
