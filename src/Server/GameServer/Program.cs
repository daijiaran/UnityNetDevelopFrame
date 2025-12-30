using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

class Program
{
    // 字典：IP地址字符串 -> 玩家数据包
    static public Dictionary<string, UserPacket> Players = new Dictionary<string, UserPacket>();
    
    // 【新增】字典：IP地址字符串 -> 客户端的 EndPoint (用于发送数据)
    static public Dictionary<string, EndPoint> ClientEndPoints = new Dictionary<string, EndPoint>();

    static void Main(string[] args)
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9050);
        socket.Bind(localEndPoint);

        Console.WriteLine("服务端已启动...");

        byte[] buffer = new byte[1024];

        while (true)
        {
            EndPoint remoteClient = new IPEndPoint(IPAddress.Any, 0);

            // 接收数据
            int receivedLength = socket.ReceiveFrom(buffer, ref remoteClient);
            string clientKey = remoteClient.ToString();

            try 
            {
                // 截取有效数据
                byte[] validBytes = new byte[receivedLength];
                Array.Copy(buffer, validBytes, receivedLength);

                // 反序列化
                UserPacket newPacket = new UserPacket(validBytes);

                Console.WriteLine($"收到 {clientKey} ({newPacket.Name}) 的位置: {newPacket.X:F2}, {newPacket.Y:F2}, {newPacket.Z:F2}");

                // 更新玩家数据
                if (Players.ContainsKey(clientKey))
                {
                    Players[clientKey] = newPacket;
                }
                else
                {
                    Players.Add(clientKey, newPacket);
                    Console.WriteLine($"新玩家加入: {newPacket.Name}");
                }

                // 【新增】更新或保存客户端的 EndPoint
                if (!ClientEndPoints.ContainsKey(clientKey))
                {
                    ClientEndPoints.Add(clientKey, remoteClient);
                }
                else
                {
                    ClientEndPoints[clientKey] = remoteClient;
                }

                // 【新增】广播：把收到的这个包发给其他所有客户端
                foreach (var kvp in ClientEndPoints)
                {
                    string targetKey = kvp.Key;
                    EndPoint targetEndPoint = kvp.Value;

                    // 不发给自己 (可选，通常客户端有本地预测，不需要服务器发回给自己)
                    if (targetKey != clientKey)
                    {
                        socket.SendTo(validBytes, targetEndPoint);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"错误: {e.Message}");
            }
        }
    }
}