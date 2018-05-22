using UnityEngine;
using UnityEngine.EventSystems;
using superFunction;

public class BattleClickGo : MonoBehaviour, IPointerClickHandler
{
    public const string CLICK_EVENT = "battleCellClick";

    public void OnPointerClick(PointerEventData _data)
    {
        SuperFunction.Instance.DispatchEvent(gameObject, CLICK_EVENT);
    }
}
