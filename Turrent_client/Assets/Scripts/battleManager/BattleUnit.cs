using UnityEngine;
using UnityEngine.UI;
using Turrent_lib;
using System.Collections.Generic;

public class BattleUnit : MonoBehaviour
{
    private const string TEXT_FIX = "{0}  {1}/{2}";

    [SerializeField]
    private float sizeFix = 10;

    [SerializeField]
    private Text text;

    [SerializeField]
    private BattleTurrent turrentRes;

    private UnitSDS sds;

    private List<Turrent> list;

    private bool isMine;

    public int unitWidth;

    public int unitHeight;

    public void Init(bool _isMine, List<Turrent> _list)
    {
        isMine = _isMine;

        list = _list;

        sds = list[0].parent.sds as UnitSDS;

        unitWidth = 0;

        unitHeight = 0;

        for (int i = 0; i < sds.pos.Length; i++)
        {
            int pos = sds.pos[i];

            int nowX = pos % BattleConst.MAP_WIDTH;

            int nowY = pos / BattleConst.MAP_WIDTH;

            if (nowX > unitWidth)
            {
                unitWidth = nowX;
            }

            if (nowY > unitHeight)
            {
                unitHeight = nowY;
            }
        }

        unitWidth += 1;

        unitHeight += 1;

        float screenWidth = (transform.root as RectTransform).sizeDelta.x;

        float width = screenWidth / BattleConst.MAP_WIDTH;

        (transform as RectTransform).sizeDelta = new Vector2(width * unitWidth - sizeFix, width * unitHeight - sizeFix);
    }

    public void Refresh()
    {
        text.text = string.Format(TEXT_FIX, sds.name, list[0].parent.hp, sds.hp);
    }
}
