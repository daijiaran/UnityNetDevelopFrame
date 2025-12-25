using System;            // 基础系统库
using System.Net;        // 【关键】修复 EndPoint 报错必须要有这一行
using System.Threading;  // 用于 Thread.Sleep
using LiteNetLib;        // 网络库
using LiteNetLib.Utils;  // 网络工具
using GameShared.Net;    // 你的共享代码

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Initializing Game Server...");

        // 1. 初始化核心网络组件
        EventBasedNetListener listener = new EventBasedNetListener();
        NetManager server = new NetManager(listener);
        
        // 2. 配置包处理器
        NetPacketProcessor packetProcessor = new NetPacketProcessor();
        
        // 3. 订阅业务逻辑
        packetProcessor.SubscribeReusable<LoginRequestPacket, NetPeer>((packet, peer) =>
        {
            // peer.ToString() 会自动输出 IP 地址和端口，不需要显式调用 EndPoint
            Console.WriteLine($"Received Login Request from {peer}: {packet.Username}");
            
            // 模拟验证逻辑...
        });

        // 4. 绑定底层网络事件
        listener.ConnectionRequestEvent += request =>
        {
            if (server.ConnectedPeersCount < 100)
                request.AcceptIfKey("MyGameKey");
            else
                request.Reject();
        };

        // 【修复2】注意这里增加了 'byte channel' 参数
        // 现在的签名是: (peer, reader, channel, deliveryMethod)
        listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) =>
        {
            try 
            {
                // 将原始数据交给 Processor 处理
                packetProcessor.ReadAllPackets(reader, peer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing packet: {ex.Message}");
            }
        };

        // 5. 启动服务器
        server.Start(9050);
        Console.WriteLine("Server started on port 9050. Press ESC to quit.");

        // 6. 服务器主循环
        const int TickRate = 60;
        const int MsPerTick = 1000 / TickRate;

        while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Escape)
        {
            var startTime = DateTime.Now;

            server.PollEvents();

            var executionTime = (DateTime.Now - startTime).TotalMilliseconds;
            var sleepTime = MsPerTick - (int)executionTime;

            if (sleepTime > 0)
            {
                Thread.Sleep(sleepTime);
            }
        }
        
        server.Stop();
    }
}