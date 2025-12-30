using System;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : SingelBase<GameRoot>
{
   private void Awake()
   {
      Init();
   }

   public StartGamePanel StartGamePanel;
   public NetworkManager NetworkManager;
   
   private void Start()
   {
     GameObject starpanel  = Instantiate(Resources.Load<GameObject>("Prefabs/StartGamePanel"));
     StartGamePanel = starpanel.GetComponent<StartGamePanel>();
     StartGamePanel.transform.SetParent(GetParent());
     StartGamePanel.GameStarteEvent += joinGame;
   }

   public void joinGame(String PlayerName)
   {
       GameObject networkManager = Instantiate(Resources.Load<GameObject>("Prefabs/NetworkManager"));
       NetworkManager = networkManager.GetComponent<NetworkManager>();
       NetworkManager.transform.SetParent(GetParent());
       NetworkManager.GameStart(PlayerName);
   }


   public Transform GetParent()
   {
       foreach (Transform child in transform)
       {
           if (child.transform.name == "Canvas")
           {
               return child.transform;
           }
       }
       return null;
   }
   
   
}
