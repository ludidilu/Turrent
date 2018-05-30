using System;
using UnityEngine;
using UnityEngine.UI;
using superFunction;

public class BattleAlert : MonoBehaviour
{
    [SerializeField]
    private Text text;

    [SerializeField]
    private BattleMouseDownGo bg;

    private Action callBack;

    private void Awake()
    {
        SuperFunction.Instance.AddEventListener(bg.gameObject, BattleMouseDownGo.EVENT_NAME, ClickBg);
    }

    private void ClickBg(int _index)
    {
        gameObject.SetActive(false);

        if (callBack != null)
        {
            callBack();
        }
    }

    public void Show(string _str, Action _callBack)
    {
        callBack = _callBack;

        gameObject.SetActive(true);

        text.text = _str;
    }
}
