using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionTrigger : MonoBehaviour
{

    Player player;

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    void OnTriggerEnter(Collider hittenCollider)
    {
        if (hittenCollider.tag == "Rock")
            player.RockHit();

        if (hittenCollider.tag == "Tree")
            player.TreeHit(hittenCollider.transform);

        if (hittenCollider.tag == "TreeOutside")
            player.BypassTree();

        if (hittenCollider.tag == "JumpBoard")
        {
            player.flyTimes++;
            player.PlaySkiingSFX(false);
            player.isJumping = true;
        }

        if (hittenCollider.tag == "Ground")
            player.PlaySkiingSFX(true);

        if (hittenCollider.tag == "Coin")
            player.AcquireCoin(hittenCollider.transform);
    }
}