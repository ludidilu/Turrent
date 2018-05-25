using UnityEngine;
using superList;
using UnityEngine.UI;

public class TurrentCreaterTargetCell : SuperListCell
{
    [SerializeField]
    private Text text;

    [SerializeField]
    private Image bg;

    public override void SetSelected(bool _value)
    {
        base.SetSelected(_value);

        bg.color = selected ? Color.gray : Color.white;
    }
}
