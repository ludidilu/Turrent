using UnityEngine;
using UnityEngine.UI;
using Turrent_lib;
using System.Collections.Generic;

public class BattleUnit : MonoBehaviour
{
    [SerializeField]
    private float sizeFix = 30;

    [SerializeField]
    private Text text;

    [SerializeField]
    private BattleTurrent turrentRes;

    [SerializeField]
    private RectTransform container;

    [SerializeField]
    private RectTransform hpBar;

    [SerializeField]
    private RectTransform turrentContainer;

    private Unit unit;

    private UnitSDS sds;

    private bool isMine;

    private float bgWidth;

    private List<BattleTurrent> turrentList = new List<BattleTurrent>();

    private BattleManager battleManager;

    public void Init(BattleManager _battleManager, bool _isMine, Unit _unit, int _pos)
    {
        battleManager = _battleManager;

        isMine = _isMine;

        unit = _unit;

        sds = unit.sds as UnitSDS;

        text.text = sds.name;

        int unitWidth = 0;

        int unitHeight = 0;

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

        bgWidth = width * unitWidth - sizeFix;

        container.sizeDelta = new Vector2(bgWidth, width * unitHeight - sizeFix);

        if (isMine)
        {
            container.anchoredPosition = new Vector2(width * unitWidth * 0.5f - 0.5f * width, -(width * unitHeight * 0.5f - 0.5f * width));
        }
        else
        {
            container.anchoredPosition = new Vector2(-(width * unitWidth * 0.5f - 0.5f * width), width * unitHeight * 0.5f - 0.5f * width);
        }

        for (int i = 0; i < sds.turrent.Length; i++)
        {
            int turrentID = sds.turrent[i];

            TurrentSDS turrentSDS = StaticData.GetData<TurrentSDS>(turrentID);

            BattleTurrent turrent;

            if (turrentList.Count == i)
            {
                turrent = Instantiate(turrentRes);

                turrent.transform.SetParent(turrentContainer, false);

                turrentList.Add(turrent);
            }
            else
            {
                turrent = turrentList[i];
            }

            int pos = sds.pos[i];

            turrent.transform.position = battleManager.GetPos(isMine, _pos + pos);

            turrent.gameObject.SetActive(!string.IsNullOrEmpty(turrentSDS.icon));
        }

        while (turrentList.Count > sds.turrent.Length)
        {
            Destroy(turrentList[turrentList.Count - 1].gameObject);

            turrentList.RemoveAt(turrentList.Count - 1);
        }

        Refresh();
    }

    public void Refresh()
    {
        hpBar.sizeDelta = new Vector2((float)unit.hp / sds.hp * bgWidth, hpBar.sizeDelta.y);
    }
}
