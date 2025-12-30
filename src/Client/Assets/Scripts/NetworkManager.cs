using System;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using GameShared.Net;
using Plugins.MyNetLib;

public class NetworkManager : SingelBase<NetworkManager>
{
    
    
    public GameObject PlayerPrefab;
    public PlayerControl playerSelf;
    
    public Dictionary<String, PlayerControl> players;
    
    public bool isJoinServise = false;
    
    private UserPacket userPacket;
    private NetConect  netConect; 
    
    
    
    
    public void Awake()
    {
        // 1. 初始化玩家字典（解决当前的报错）
        players = new Dictionary<string, PlayerControl>();

        // 2. 初始化数据包对象（防止 SendPlayer 报错）
        userPacket = new UserPacket();

        // 3. 初始化网络连接对象（防止 Update 报错）
        netConect = new NetConect();
        
        
        
        // 4. 开启接收线程（否则收不到消息）
        netConect.ReceiveInformation();
        Init();
    }


    
    
    //先创建自己的玩家
    public void GameStart(String name)
    {
        GameObject player = Instantiate(PlayerPrefab);
        playerSelf = player.GetComponent<PlayerControl>();
        playerSelf.PlayerName.text = name;
        isJoinServise = true;
        
        netConect.takePlayerPacket+=synchronousOtherPlayer;
    }

    private void Update()
    {
        
        //驱动 NetConect 处理消息队列
        if (netConect != null)
        {
            netConect.Update();
        }
        
        if (isJoinServise)
        {
           SendPlayer();
        }
    }




    public void SendPlayer()
    {
        // 确保 userPacket 不为空
        if (userPacket == null) userPacket = new UserPacket();
        
        userPacket.Name  = playerSelf.PlayerName.text; 
        userPacket.X = playerSelf.transform.position.x;
        userPacket.Y = playerSelf.transform.position.y;
        userPacket.Z = playerSelf.transform.position.z;
        netConect.SendPositionPacket(userPacket);
    }

    public void synchronousOtherPlayer(String IpDetail , UserPacket userPacket)
    {
        
        
        if (!players.ContainsKey(IpDetail))
        {   
            //如果没有这个玩家就创建一个新的玩家并且配置上位置信息
            players.Add(IpDetail,CreatNewPlayer());
            players[IpDetail].PlayerName.text = userPacket.Name;
            Vector3 vtr3 = new Vector3(userPacket.X,userPacket.Y,userPacket.Z);
            players[IpDetail].transform.position = vtr3;
        }
        else
        {
            //如果此玩家存在则直接同步位置信息
            players[IpDetail].PlayerName.text = userPacket.Name;
            Vector3 vtr3 = new Vector3(userPacket.X,userPacket.Y,userPacket.Z);
            players[IpDetail].transform.position = vtr3;
        }
    }


    public PlayerControl CreatNewPlayer()
    {
        GameObject player = Instantiate(PlayerPrefab);
        return player.GetComponent<PlayerControl>();
    }
    
    
    
    
    
    
}