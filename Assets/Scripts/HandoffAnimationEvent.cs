using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandoffAnimationEvent : MonoBehaviour
{
    public GiantController controller;

    public void HandoffEvent()
    {
        controller.ThrowRock();
    }
}
