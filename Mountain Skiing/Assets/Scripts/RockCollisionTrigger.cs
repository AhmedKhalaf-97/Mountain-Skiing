using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockCollisionTrigger : MonoBehaviour
{
    Player player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void OnTriggerEnter(Collider hittenCollider)
    {
        if (!player.gameObject.activeInHierarchy)
            return;

        if (hittenCollider.tag == "Tree")
            player.TreeBrokeAndResetCoroutine(hittenCollider.transform);
    }
}