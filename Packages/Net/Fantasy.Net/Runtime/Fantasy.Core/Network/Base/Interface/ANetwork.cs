using System;
using System.Buffers;
using System.IO;
using System.Net;
using Fantasy.Helper;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
#pragma warning disable CS8625
#pragma warning disable CS8618

namespace Fantasy.Core.Network
{
    /// <summary>
    /// ��Ϣ������Ϣ�ṹ��
    /// </summary>
    public struct MessageCacheInfo
    {
        /// <summary>
        /// ��ȡ������ RPC ID��
        /// </summary>
        public uint RpcId;

        /// <summary>
        /// ��ȡ������·�� ID��
        /// </summary>
        public long RouteId;

        /// <summary>
        /// ��ȡ������·������������롣
        /// </summary>
        public long RouteTypeOpCode;

        /// <summary>
        /// ��ȡ��������Ϣ����
        /// </summary>
        public object Message;

        /// <summary>
        /// ��ȡ�������ڴ�����
        /// </summary>
        public MemoryStream MemoryStream;
    }


    /// <summary>
    /// ����������ࡣ
    /// </summary>
    public abstract class ANetwork : IDisposable
    {
        /// <summary>
        /// ��ȡ�����ΨһID��
        /// </summary>
        public long NetworkId { get; private set; }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        public Scene Scene { get; private set; }

        /// <summary>
        /// ��ȡ�����������Ƿ��ѱ��ͷš�
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// ��ȡ�������͡�
        /// </summary>
        public NetworkType NetworkType { get; private set; }

        /// <summary>
        /// ��ȡ����Э�����͡�
        /// </summary>
        public NetworkProtocolType NetworkProtocolType { get; private set; }

        /// <summary>
        /// ��ȡ����Ŀ�����͡�
        /// </summary>
        public NetworkTarget NetworkTarget { get; private set; }

        /// <summary>
        /// ��ȡ������������Ϣ��������
        /// </summary>
        public ANetworkMessageScheduler NetworkMessageScheduler { get; private set; }

        /// <summary>
        /// ��ʼ����������������ʵ����
        /// </summary>
        /// <param name="scene">��������</param>
        /// <param name="networkType">�������͡�</param>
        /// <param name="networkProtocolType">����Э�����͡�</param>
        /// <param name="networkTarget">����Ŀ�����͡�</param>
        protected ANetwork(Scene scene, NetworkType networkType, NetworkProtocolType networkProtocolType, NetworkTarget networkTarget)
        {
            NetworkThread.Instance.AddNetwork(this);

            Scene = scene;
            NetworkType = networkType;
            NetworkTarget = networkTarget;
            NetworkProtocolType = networkProtocolType;
            NetworkId = IdFactory.NextRunTimeId();

#if FANTASY_NET
            if (networkTarget == NetworkTarget.Inner)
            {
                NetworkMessageScheduler = new InnerMessageScheduler();
                return;
            }
#endif

            switch (networkType)
            {
                case NetworkType.Client:
                    {
                        NetworkMessageScheduler = new ClientMessageScheduler();
                        return;
                    }
                case NetworkType.Server:
                    {
                        NetworkMessageScheduler = new OuterMessageScheduler();
                        return;
                    }
            }
        }

        /// <summary>
        /// ����Ϣ��Ϣ������ڴ���
        /// </summary>
        /// <param name="rpcId"></param>
        /// <param name="routeTypeOpCode"></param>
        /// <param name="routeId"></param>
        /// <param name="memoryStream"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected MemoryStream PackMessage(uint rpcId, long routeTypeOpCode, long routeId, MemoryStream memoryStream, object message)
        {
#if FANTASY_NET
            if (NetworkTarget == NetworkTarget.Inner)
            {
                // �����Ƿ��ṩ�ڴ��������Ϣ
                return memoryStream == null
                    ? InnerPacketParser.Pack(rpcId, routeId, message)
                    : InnerPacketParser.Pack(rpcId, routeId, memoryStream);
            }
            else
#endif
            {
                // �����Ƿ��ṩ�ڴ��������Ϣ
                return memoryStream == null
                    ? OuterPacketParser.Pack(rpcId, routeTypeOpCode, message)
                    : OuterPacketParser.Pack(rpcId, routeTypeOpCode, memoryStream);
            }

        }


        /// <summary>
        /// ������Ϣ��
        /// </summary>
        /// <param name="channelId">ͨ�� ID��</param>
        /// <param name="rpcId">RPC ID��</param>
        /// <param name="routeTypeOpCode">·������������롣</param>
        /// <param name="routeId">·�� ID��</param>
        /// <param name="message">��Ϣ����</param>
        public abstract void Send(uint channelId, uint rpcId, long routeTypeOpCode, long routeId, object message);
        /// <summary>
        /// ������Ϣ��
        /// </summary>
        /// <param name="channelId">ͨ�� ID��</param>
        /// <param name="rpcId">RPC ID��</param>
        /// <param name="routeTypeOpCode">·������������롣</param>
        /// <param name="routeId">·�� ID��</param>
        /// <param name="memoryStream">�ڴ�����������Ϣ���ݡ�</param>
        public abstract void Send(uint channelId, uint rpcId, long routeTypeOpCode, long routeId, MemoryStream memoryStream);
        /// <summary>
        /// �Ƴ�ͨ����
        /// </summary>
        /// <param name="channelId">ͨ�� ID��</param>
        public abstract void RemoveChannel(uint channelId);

        /// <summary>
        /// �ͷ���Դ��
        /// </summary>
        public virtual void Dispose()
        {
            // �������߳����Ƴ��������
            NetworkThread.Instance?.RemoveNetwork(NetworkId);

            NetworkId = 0;
            Scene = null;
            IsDisposed = true;
            NetworkType = NetworkType.None;
            NetworkTarget = NetworkTarget.None;
            NetworkProtocolType = NetworkProtocolType.None;
        }
    }
}