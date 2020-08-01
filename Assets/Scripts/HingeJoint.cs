using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeJoint : MonoBehaviour
{
    public GameObject player;
    public HingeJoint2D joint;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        joint.connectedAnchor = player.transform.position;
    }
}
