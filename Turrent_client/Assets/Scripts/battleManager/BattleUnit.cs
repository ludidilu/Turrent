using UnityEngine;
using UnityEngine.UI;
using Turrent_lib;
using System.Collections.Generic;

public class BattleUnit : MonoBehaviour
{
    private const string TEXT_FIX = "{0}  {1}/{2}";

    [SerializeField]
    private float sizeFix = 30;

    [SerializeField]
    private Text text;

    [SerializeField]
    private BattleTurrent turrentRes;

    [SerializeField]
    private RectTransform container;

    private Unit unit;

    private UnitSDS sds;

    private bool isMine;

    [HideInInspector]
    public int unitWidth;

    [HideInInspector]
    public int unitHeight;

    public void Init(bool _isMine, Unit _unit)
    {
        isMine = _isMine;

        unit = _unit;

        sds = unit.sds as UnitSDS;

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

        container.sizeDelta = new Vector2(width * unitWidth - sizeFix, width * unitHeight - sizeFix);

        if (isMine)
        {
            container.anchoredPosition = new Vector2(width * unitWidth * 0.5f - 0.5f * width, width * unitHeight * 0.5f - 0.5f * width);
        }
        else
        {
            container.anchoredPosition = new Vector2(-(width * unitWidth * 0.5f - 0.5f * width), -(width * unitHeight * 0.5f - 0.5f * width));
        }

        Refresh();
    }

    public void Refresh()
    {
        text.text = string.Format(TEXT_FIX, sds.name, unit.hp, sds.hp);
    }
}
