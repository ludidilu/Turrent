using superFunction;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleMouseDownGo : MonoBehaviour, IPointerDownHandler
{
    public const string EVENT_NAME = "onMouseDown";

    public void OnPointerDown(PointerEventData _data)
    {
        SuperFunction.Instance.DispatchEvent(gameObject, EVENT_NAME);
    }
}
