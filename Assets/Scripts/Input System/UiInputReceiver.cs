using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UiInputReceiver : InputReceiver
{
    [SerializeField] UnityEvent clickEvent;
    public override void OnInputReceived()
    {
        foreach (var handler in inputHandlers)
        {
            handler.ProcessInput(Input.mousePosition, gameObject, () => clickEvent.Invoke());
        }
    }
}
