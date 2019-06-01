using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class 同步active : 同步系统
{
    public 同步active()
    {
        同步类型 = 数据类型.active;
    }

    public override void 本地更新(byte[] 数据)
    {
        网络系统.未开关物体.Enqueue(new 全局状态(gameObject, Convert.ToBoolean(数据.Length)));
        网络系统.未开关物体.Enqueue(new 全局状态(gameObject, Convert.ToBoolean(数据.Length)));
        网络系统.未开关物体.Enqueue(new 全局状态(gameObject, Convert.ToBoolean(数据.Length)));
    }
    public override void 网络更新()
    {
        网络系统.发送信息(GetComponent<网络物体身份>().网络标识.发送(同步类型),new byte[Convert.ToInt32(gameObject.activeSelf)]);
    }
    public override void 初始化()
    {
        网络更新();
    }
}
