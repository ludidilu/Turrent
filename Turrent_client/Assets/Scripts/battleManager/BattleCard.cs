using UnityEngine;
using UnityEngine.UI;

public class BattleCard : BattleClickGo
{
    [SerializeField]
    private Text nameText;

    [SerializeField]
    private Text costText;

    [SerializeField]
    private Image bg;

    public int uid { private set; get; }

    public void Init(int _uid, int _id)
    {
        uid = _uid;

        UnitSDS sds = StaticData.GetData<UnitSDS>(_id);

        nameText.text = sds.name;

        costText.text = sds.cost.ToString();
    }

    public void SetSelected(bool _b)
    {
        bg.color = _b ? Color.yellow : Color.white;
    }
}
