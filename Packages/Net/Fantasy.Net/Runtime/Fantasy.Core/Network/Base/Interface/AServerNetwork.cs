using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Core.Network
{
    /// <summary>
    /// 抽象服务端网络基类
    /// </summary>
    public abstract partial class AServerNetwork : ANetwork
    {
        /// <summary>
        /// 初始化抽象服务端网络基类的新实例。
        /// </summary>
        /// <param name="scene">场景对象。</param>
        /// <param name="networkType">网络类型。</param>
        /// <param name="networkProtocolType">网络协议类型。</param>
        /// <param name="networkTarget">网络目标类型。</param>
        protected AServerNetwork(Scene scene, NetworkType networkType, NetworkProtocolType networkProtocolType, NetworkTarget networkTarget) : base(scene, networkType, networkProtocolType, networkTarget)
        {
        }
    }
}
