using Fantasy.DataStructure;
using NLog.Targets;
using System;
using System.Buffers;
using System.IO;
using System.Net;
#pragma warning disable CS8625
#pragma warning disable CS8618

namespace Fantasy.Core.Network
{
    public abstract partial class AServerNetwork
    {
        /// <summary>
        /// 抽象的网络通道基类。
        /// </summary>
        protected abstract class ANetworkChannel : ISessionable
        {
            /// <summary>
            /// 获取通道的唯一标识 ID。
            /// </summary>
            public uint ChannelId { get; private set; }
            /// <summary>
            /// 获取或设置通道所属的场景。
            /// </summary>
            public Scene Scene { get; protected set; }
            /// <summary>
            /// 获取通道所属的网络 ID。
            /// </summary>
            public long NetworkId { get; private set; }
            /// <summary>
            /// 获取通道是否已经被释放。
            /// </summary>
            public bool IsDisposed { get; protected set; }
            /// <summary>
            /// 获取通道的远程终端点。
            /// </summary>
            public EndPoint RemoteEndPoint { get; protected set; }
            ///// <summary>
            ///// 获取通道的数据包解析器。
            ///// </summary>
            //public APacketParser _packetParser { get; protected set; }

            /// <summary>
            /// 当通道被释放时触发的事件。
            /// </summary>
            public abstract event Action OnDispose;

            /// <summary>
            /// 当通道接收到内存流数据包时触发的事件。
            /// </summary>
            public abstract event Action<APackInfo> OnReceiveMemoryStream;

            /// <summary>
            /// 网络通信包解析器
            /// </summary>
            private APacketParser _packetParser;

            /// <summary>
            /// 初始化抽象网络通道基类的新实例。
            /// </summary>
            /// <param name="scene">通道所属的场景。</param>
            /// <param name="id">通道的唯一标识 ID。</param>
            /// <param name="network">通道所属的网络。</param>
            protected ANetworkChannel(Scene scene, uint id, ANetwork network)
            {
                ChannelId = id;
                Scene = scene;
                NetworkId = network.NetworkId;
                _packetParser = APacketParser.CreatePacketParser(network.NetworkTarget);
            }
            /// <summary>
            /// 从循环缓冲区解析数据包。
            /// </summary>
            /// <param name="buffer">循环缓冲区。</param>
            /// <param name="packInfo">解析得到的数据包信息。</param>
            /// <returns>如果成功解析数据包，则返回 true；否则返回 false。</returns>
            protected bool UnPack(CircularBuffer buffer, out APackInfo packInfo)
            {
                return _packetParser.UnPack(buffer, out packInfo);
            }
            /// <summary>
            /// 从内存块解析数据包。
            /// </summary>
            /// <param name="memoryOwner">内存块的所有者。</param>
            /// <param name="packInfo">解析得到的数据包信息。</param>
            /// <returns>如果成功解析数据包，则返回 true；否则返回 false。</returns>
            protected bool UnPack(IMemoryOwner<byte> memoryOwner, out APackInfo packInfo)
            {
                return _packetParser.UnPack(memoryOwner, out packInfo);
            }


            /// <summary>
            /// 释放通道资源。
            /// </summary>
            public virtual void Dispose()
            {
                NetworkThread.Instance.RemoveChannel(NetworkId, ChannelId);

                ChannelId = 0;
                Scene = null;
                NetworkId = 0;
                IsDisposed = true;
                RemoteEndPoint = null;

                if (_packetParser != null)
                {
                    _packetParser.Dispose();
                    _packetParser = null;
                }
            }
        }
    }

}