using UnityEngine;
using UnityEngine.EventSystems;
using superFunction;

public class BattleClickGo : MonoBehaviour, IPointerClickHandler
{
    public const string EVENT_NAME = "onClick";

    public void OnPointerClick(PointerEventData _data)
    {
        SuperFunction.Instance.DispatchEvent(gameObject, EVENT_NAME);
    }
}
