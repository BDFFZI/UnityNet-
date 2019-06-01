using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class 网络物体身份 : MonoBehaviour
{
    public Dictionary<数据类型, 同步系统> 同步工具 = new Dictionary<数据类型, 同步系统>();
    public 网络数据标识 网络标识;

    private void Awake()
    {
        if (网络系统.属性 == 网络属性.单机)
            return;

        同步系统[] 同步工具t = GetComponents<同步系统>();
        foreach (同步系统 工具 in 同步工具t)
            同步工具.Add(工具.同步类型, 工具);
    }
    private void OnDisable()
    {
        foreach (同步系统 工具 in 同步工具.Values)
            工具.网络更新();
    }
    private void OnEnable()
    {
        if (GetComponent<网络物体身份>().网络标识.编号 == 网络系统.编号)
            foreach (同步系统 工具 in 同步工具.Values)
                工具.初始化();
    }

    public void 网络实例化()
    {
        网络系统.发送信息(网络标识, Encoding.Default.GetBytes(""/*预制体Resources路径*/));
        OnEnable();
    }

    public static volatile Dictionary<string, 网络物体身份> 网络物体清单 = new Dictionary<string, 网络物体身份>();
    public static volatile int 网络物体总计;

    public static 网络物体身份 网络ID注册(GameObject 物体, int 网络物体ID, int 编号)
    {
        网络物体身份 网络身份 = 物体.GetComponent<网络物体身份>();
        网络身份.网络标识.网络ID = 网络物体ID;
        网络身份.网络标识.编号 = 编号;
        网络身份.网络标识.类型 = 0;
        网络物体清单.Add(编号.ToString() + 网络物体ID, 网络身份);
        return 网络身份;
    }
}

[Serializable]
public class 同步工具
{
    public 数据类型 类型;
    public 同步系统 工具;
}
