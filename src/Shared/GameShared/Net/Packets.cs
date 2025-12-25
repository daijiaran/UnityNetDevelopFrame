using LiteNetLib.Utils;

namespace GameShared.Net
{
    // 定义包类型枚举，用于快速路由
    public enum PacketType : byte
    {
        LoginRequest,
        LoginResponse,
        PlayerPosition
    }

    // 自定义网络向量结构
    public struct NetVector3 : INetSerializable
    {
        public float X, Y, Z;

        // 构造函数
        public NetVector3(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        // 实现序列化：写入数据到网络流
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
            writer.Put(Z);
        }

        // 实现反序列化：从网络流读取数据
        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetFloat();
            Y = reader.GetFloat();
            Z = reader.GetFloat();
        }
    }

    // 登录请求包
    public class LoginRequestPacket : INetSerializable
    {
        public string Username;
        public string Password; // 实际项目中请勿明文传输密码

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Username);
            writer.Put(Password);
        }

        public void Deserialize(NetDataReader reader)
        {
            Username = reader.GetString();
            Password = reader.GetString();
        }
    }
}