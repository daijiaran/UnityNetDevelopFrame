using System;
using System.Collections.Concurrent; // 【1】必须引入这个命名空间
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.MyNetLib
{
    public class NetConect
    {
        
        // 构造函数
        public NetConect()
        {
            // 【新增】这一行非常关键！
            // 绑定到本机任意可用端口（IPAddress.Any, 0）
            // 只有绑定了，Socket 才知道要“打开耳朵”听数据
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        }
        
        
        
        
        // 接受到信息的时候通知 UI 更新
        public Action<String> takeMessage;
        //发送失败的时候
        public Action<String> errCallback;
        
        
        
        //分别接受其他玩家的IP地址，和玩家的包
        public Action<String,UserPacket> takePlayerPacket;
        
        
        
        
        
        //实现网络信息发送的管子
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        
        // 目标IP地址
        private IPEndPoint ServerAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

        // 【2】创建一个线程安全的队列，作为“篮子”
        private ConcurrentQueue<string> _msgQueue = new ConcurrentQueue<string>();

        
        
        
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            // 这里建议加 try-catch 防止没连网报错
            try 
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                socket.SendTo(data, ServerAddress);
            }
            catch (Exception e)
            {
                errCallback.Invoke(e.Message);
            }
        }


        

        /// <summary>
        /// 发送位置信息
        /// </summary>
        /// <param name="packet"></param>
        public void SendPositionPacket(UserPacket packet)
        {
            // 【关键】调用 ToBytes() 变成数组，再发送
            byte[] data = packet.ToBytes();
            
            // 这里建议加 try-catch 防止没连网报错
            try 
            {
                socket.SendTo(data, ServerAddress);
            }
            catch (Exception e)
            {
                errCallback.Invoke(e.Message);
            }
        }

        

        
        
        /// <summary>
        /// 线程方法，开启一个监听服务器信息的线程
        /// </summary>
        public void ReceiveInformation()
        {
            Task.Run(() => 
            {
                byte[] recvBuffer = new byte[1024];
                while (true)
                {
                    try
                    {
                        EndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
                        
                       //获取一个包信息
                       //然后加入队列由unity处理
                        
                        
                        
                        
                        
                        
                        
                        
                        
                    }
                    catch (Exception)
                    {
                        // 处理连接断开或 socket 关闭的情况
                        break;
                    }
                }
            });
        }

        
        
        
        
        /// <summary>
        /// 供 Unity 的 Update 调用实时向服务器发送信息
        /// </summary>
        public void Update()
        {
            // 只要队列里有数据，就取出来
            while (_msgQueue.TryDequeue(out string msg))
            {
                // // 这里是在主线程运行的，所以可以安全地通知 UI
                // takeMessage?.Invoke(msg);
                
                //这里接受到玩家包信息，然后触发takePlayerPacket事件
                
                
                
                
            }
        }
        
        
        
        
        
        
        
        // 建议加一个关闭方法
        public void Close()
        {
            socket.Close();
        }
    }
}