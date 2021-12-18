using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnAdditiveScoreText : MonoBehaviour
{
    Player player;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void ReturnThisText()
    {
        player.ReturnGameobjectToPool(transform);
    }
}
