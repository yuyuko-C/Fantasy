using Fantasy.DataStructure;
using System;
using System.Buffers;
using System.IO;
using System.Net;

namespace Fantasy.Core.Network
{
    /// <summary>
    /// ����ͻ���������ࡣ
    /// </summary>
    public abstract class AClientNetwork : ANetwork, ISessionable
    {
        /// <summary>
        /// ��ȡ������ͨ��ID��
        /// </summary>
        public uint ChannelId { get; protected set; }

        /// <summary>
        /// �����������ͷ�ʱ�������¼���
        /// </summary>
        public abstract event Action OnDispose;

        /// <summary>
        /// ������ʧ��ʱ�������¼���
        /// </summary>
        public abstract event Action OnConnectFail;

        /// <summary>
        /// ���������ʱ�������¼���
        /// </summary>
        public abstract event Action OnConnectComplete;

        /// <summary>
        /// �����ӶϿ�ʱ�������¼���
        /// </summary>
        public abstract event Action OnConnectDisconnect;

        /// <summary>
        /// �ڽ��յ��ڴ���ʱ�������¼���
        /// </summary>
        public abstract event Action<APackInfo> OnReceiveMemoryStream;

        /// <summary>
        /// ����ͨ�Ű�������
        /// </summary>
        private APacketParser _packetParser;

        /// <summary>
        /// ��ʼ������ͻ�������������ʵ����
        /// </summary>
        /// <param name="scene">��������</param>
        /// <param name="networkType">�������͡�</param>
        /// <param name="networkProtocolType">����Э�����͡�</param>
        /// <param name="networkTarget">����Ŀ�����͡�</param>
        protected AClientNetwork(Scene scene, NetworkType networkType, NetworkProtocolType networkProtocolType, NetworkTarget networkTarget) : base(scene, networkType, networkProtocolType, networkTarget)
        {
            _packetParser = APacketParser.CreatePacketParser(networkTarget);
        }

        /// <summary>
        /// ���ӵ�Զ���նˡ�
        /// </summary>
        /// <param name="remoteEndPoint">Զ���ն˵� <see cref="IPEndPoint"/>��</param>
        /// <param name="onConnectComplete">���ӳɹ�ʱ�Ļص���</param>
        /// <param name="onConnectFail">����ʧ��ʱ�Ļص���</param>
        /// <param name="onConnectDisconnect">���ӶϿ�ʱ�Ļص���</param>
        /// <param name="connectTimeout">���ӳ�ʱʱ�䣨���룩��</param>
        /// <returns>ͨ��ID��</returns>
        public abstract uint Connect(IPEndPoint remoteEndPoint, Action onConnectComplete, Action onConnectFail, Action onConnectDisconnect, int connectTimeout = 5000);

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
        /// �ͷ�������Դ��
        /// </summary>
        public override void Dispose()
        {
            ChannelId = 0;
            _packetParser.Dispose();
            _packetParser = null;
            base.Dispose();
        }
    }
}