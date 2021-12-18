using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int xAxisInt;
    public Player player;

    public void OnPointerEnter(PointerEventData eventData)
    {
        player.buttonXAxis = xAxisInt;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        player.buttonXAxis = 0;
        player.ResetTLerp();
    }
}
