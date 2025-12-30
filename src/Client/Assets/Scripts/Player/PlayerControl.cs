using System;
using GameShared.Net;
using TMPro;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Collider collider;
    public Rigidbody rigidbody;
    public TextMeshProUGUI PlayerName;
    private void Start()
    {
        collider = GetComponent<Collider>(); 
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        
    }
}