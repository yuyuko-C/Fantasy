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
        /// ���������ͨ�����ࡣ
        /// </summary>
        protected abstract class ANetworkChannel : ISessionable
        {
            /// <summary>
            /// ��ȡͨ����Ψһ��ʶ ID��
            /// </summary>
            public uint ChannelId { get; private set; }
            /// <summary>
            /// ��ȡ������ͨ�������ĳ�����
            /// </summary>
            public Scene Scene { get; protected set; }
            /// <summary>
            /// ��ȡͨ������������ ID��
            /// </summary>
            public long NetworkId { get; private set; }
            /// <summary>
            /// ��ȡͨ���Ƿ��Ѿ����ͷš�
            /// </summary>
            public bool IsDisposed { get; protected set; }
            /// <summary>
            /// ��ȡͨ����Զ���ն˵㡣
            /// </summary>
            public EndPoint RemoteEndPoint { get; protected set; }
            ///// <summary>
            ///// ��ȡͨ�������ݰ���������
            ///// </summary>
            //public APacketParser _packetParser { get; protected set; }

            /// <summary>
            /// ��ͨ�����ͷ�ʱ�������¼���
            /// </summary>
            public abstract event Action OnDispose;

            /// <summary>
            /// ��ͨ�����յ��ڴ������ݰ�ʱ�������¼���
            /// </summary>
            public abstract event Action<APackInfo> OnReceiveMemoryStream;

            /// <summary>
            /// ����ͨ�Ű�������
            /// </summary>
            private APacketParser _packetParser;

            /// <summary>
            /// ��ʼ����������ͨ���������ʵ����
            /// </summary>
            /// <param name="scene">ͨ�������ĳ�����</param>
            /// <param name="id">ͨ����Ψһ��ʶ ID��</param>
            /// <param name="network">ͨ�����������硣</param>
            protected ANetworkChannel(Scene scene, uint id, ANetwork network)
            {
                ChannelId = id;
                Scene = scene;
                NetworkId = network.NetworkId;
                _packetParser = APacketParser.CreatePacketParser(network.NetworkTarget);
            }
            /// <summary>
            /// ��ѭ���������������ݰ���
            /// </summary>
            /// <param name="buffer">ѭ����������</param>
            /// <param name="packInfo">�����õ������ݰ���Ϣ��</param>
            /// <returns>����ɹ��������ݰ����򷵻� true�����򷵻� false��</returns>
            protected bool UnPack(CircularBuffer buffer, out APackInfo packInfo)
            {
                return _packetParser.UnPack(buffer, out packInfo);
            }
            /// <summary>
            /// ���ڴ��������ݰ���
            /// </summary>
            /// <param name="memoryOwner">�ڴ��������ߡ�</param>
            /// <param name="packInfo">�����õ������ݰ���Ϣ��</param>
            /// <returns>����ɹ��������ݰ����򷵻� true�����򷵻� false��</returns>
            protected bool UnPack(IMemoryOwner<byte> memoryOwner, out APackInfo packInfo)
            {
                return _packetParser.UnPack(memoryOwner, out packInfo);
            }


            /// <summary>
            /// �ͷ�ͨ����Դ��
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