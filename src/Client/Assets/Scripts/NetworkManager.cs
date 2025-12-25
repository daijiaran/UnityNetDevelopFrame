using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using GameShared.Net;

public class NetworkManager : MonoBehaviour
{
    private NetManager _client;
    private EventBasedNetListener _listener;
    private NetPacketProcessor _packetProcessor;
    
    //定义一个复用的数据写入器，避免每次发送都 new
    private NetDataWriter _dataWriter;

    void Start()
    {
        _listener = new EventBasedNetListener();
        _packetProcessor = new NetPacketProcessor();
        _dataWriter = new NetDataWriter(); // 初始化写入器
        _client = new NetManager(_listener);

        // 启动客户端
        _client.Start();
        // 连接本地服务器
        _client.Connect("localhost", 9050, "MyGameKey");

        // 【事件回调】
        // 使用 _ 忽略不需要的参数，消除 IDE 警告
        _listener.NetworkReceiveEvent += (peer, reader, _, _) =>
        {
            // 读取数据包
            _packetProcessor.ReadAllPackets(reader, peer);
        };
        
        _listener.PeerConnectedEvent += peer =>
        {
            // 如果 peer.EndPoint 报错，直接使用 peer.Port 或 peer.ToString()
            // peer.ToString() 在 LiteNetLib 中通常会返回 "IP:Port"
            Debug.Log($"Connected to Server: {peer.Address}:{peer.Port}");
            SendLogin();
        };
    }

    void Update()
    {
        if (_client != null)
            _client.PollEvents();
    }

    void SendLogin()
    {
        LoginRequestPacket login = new LoginRequestPacket 
        { 
            Username = "Player1", 
            Password = "SecretPassword" 
        };
        
        if (_client.FirstPeer != null)
        {
            //手动两步发送法（适用于所有版本）
            
            // 1. 重置写入器，清空旧数据
            _dataWriter.Reset();
            
            // 2. 让 PacketProcessor 把对象序列化进 _dataWriter
            // 注意：这里必须用 Write，不要用 WriteNetSerializable
            // Write 会自动写入包头 ID，这样服务器才能认出这是 LoginRequestPacket
            _packetProcessor.Write(_dataWriter, login);
            
            // 3. 通过 Peer 发送底层的 byte 数组
            _client.FirstPeer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }
    }

    void OnDestroy()
    {
        if (_client != null)
            _client.Stop();
    }
}