using System;
using System.Net;
#pragma warning disable CS8625
#pragma warning disable CS8618

namespace Fantasy.Core.Network
{
    /// <summary>
    /// �ͻ���Network���������
    /// </summary>
    public sealed class ClientNetworkComponent : Entity
    {
        /// <summary>
        /// ��ȡ�����Ŀͻ���Network����ʵ����
        /// </summary>
        private AClientNetwork Network { get; set; }
        /// <summary>
        /// ��ȡ��ͻ�����������ĻỰ��
        /// </summary>
        public Session Session { get; private set; }

        /// <summary>
        /// ��ʼ���ͻ������������
        /// </summary>
        /// <param name="networkProtocolType">����Э�����͡�</param>
        /// <param name="networkTarget">����Ŀ�ꡣ</param>
        public void Initialize(NetworkProtocolType networkProtocolType, NetworkTarget networkTarget)
        {
            switch (networkProtocolType)
            {
                case NetworkProtocolType.KCP:
                    {
                        Network = new KCPClientNetwork(Scene, networkTarget);
                        return;
                    }
                case NetworkProtocolType.TCP:
                    {
                        Network = new TCPClientNetwork(Scene, networkTarget);
                        return;
                    }
                default:
                    {
                        throw new NotSupportedException($"Unsupported NetworkProtocolType:{networkProtocolType}");
                    }
            }
        }

        /// <summary>
        /// ���ӵ�ָ����Զ���նˡ�
        /// </summary>
        /// <param name="remoteEndPoint">Զ���ն˵�IP��ַ�Ͷ˿ڡ�</param>
        /// <param name="onConnectComplete">���ӳɹ�ʱ�Ļص���</param>
        /// <param name="onConnectFail">����ʧ��ʱ�Ļص���</param>
        /// <param name="onConnectDisconnect">���ӶϿ�ʱ�Ļص���</param>
        /// <param name="connectTimeout">���ӳ�ʱʱ�䣨���룩��</param>
        public void Connect(IPEndPoint remoteEndPoint, Action onConnectComplete, Action onConnectFail, Action onConnectDisconnect, int connectTimeout = 5000)
        {
            if (Network == null || Network.IsDisposed)
            {
                throw new NotSupportedException("Network is null or isDisposed");
            }

            Network.Connect(remoteEndPoint, () =>
            {
                Session = Session.Create(Network, Network.NetworkMessageScheduler);
                onConnectComplete();
            }, onConnectFail, onConnectDisconnect, connectTimeout);


        }

        /// <summary>
        /// �ͷſͻ��������������������Դ��
        /// </summary>
        public override void Dispose()
        {
            if (Network != null)
            {
                Network.Dispose();
                Network = null;
            }

            Session = null;
            base.Dispose();
        }
    }
}