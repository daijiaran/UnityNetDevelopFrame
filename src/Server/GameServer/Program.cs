using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

class Program
{
    // 字典：IP地址 -> 玩家数据包
    static public Dictionary<string, UserPacket> Players = new Dictionary<string, UserPacket>();

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

            // 【关键点 A】ReceiveFrom 返回的是接收到的“字节数”
            // buffer 会被填入实际的数据
            int receivedLength = socket.ReceiveFrom(buffer, ref remoteClient);

            // 拿到发送者的唯一标识 (IP:Port)
            string clientKey = remoteClient.ToString();

            try 
            {
                // 【关键点 B】数据截取
                // buffer 可能有 1024 这么大，但只需要前 receivedLength 个字节
                byte[] validBytes = new byte[receivedLength];
                Array.Copy(buffer, validBytes, receivedLength);

                // 【关键点 C】反序列化：把字节变回 UserPacket 对象
                UserPacket newPacket = new UserPacket(validBytes);

                Console.WriteLine($"收到 {clientKey} ({newPacket.Name}) 的位置: {newPacket.X}, {newPacket.Y}, {newPacket.Z}");

                // 【关键点 D】更新或添加玩家
                if (Players.ContainsKey(clientKey))
                {
                    Players[clientKey] = newPacket; // 更新数据
                }
                else
                {
                    Players.Add(clientKey, newPacket); // 新增玩家
                    Console.WriteLine($"新玩家加入: {newPacket.Name}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"数据解析错误，可能是发来的包格式不对: {e.Message}");
            }
        }
    }
    
    
    
}