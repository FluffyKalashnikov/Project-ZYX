using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSwapper : MonoBehaviour
{
    [SerializeField] private Texture2D arrow;
    [SerializeField] private Texture2D hand;
    public void SwapToArrow()
    {
        Cursor.SetCursor(arrow, Vector3.zero, CursorMode.ForceSoftware);
    }
    public void SwapToHand()
    {
        Cursor.SetCursor(hand, Vector3.zero, CursorMode.ForceSoftware);
    }
}
