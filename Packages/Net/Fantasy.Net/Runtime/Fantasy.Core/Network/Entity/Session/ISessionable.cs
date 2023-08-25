using Fantasy.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Core.Network;

/// <summary>
/// 统一能被Session当做参数的接口类型
/// </summary>
public interface ISessionable
{
    /// <summary>
    /// 场景
    /// </summary>
    Scene Scene { get; }
    /// <summary>
    /// 网络ID
    /// </summary>
    long NetworkId { get; }
    /// <summary>
    /// 渠道ID
    /// </summary>
    uint ChannelId { get; }
    /// <summary>
    /// 消息包接受完成并反序列化后的回调
    /// </summary>
    event Action<APackInfo> OnReceiveMemoryStream;
    /// <summary>
    /// 处理事件
    /// </summary>
    event Action OnDispose;
}