using System.Collections.Generic;
using UnityEngine;
using Turrent_lib;
using superFunction;

public class TurrentCreaterField : MonoBehaviour
{
    [SerializeField]
    private TurrentCreaterCell cellRes;

    [SerializeField]
    private float yFix;

    private TurrentCreater parent;

    private Dictionary<KeyValuePair<int, int>, TurrentCreaterCell> cellDic = new Dictionary<KeyValuePair<int, int>, TurrentCreaterCell>();

    public KeyValuePair<List<KeyValuePair<int, int>>, List<KeyValuePair<int, int>>> data = new KeyValuePair<List<KeyValuePair<int, int>>, List<KeyValuePair<int, int>>>(new List<KeyValuePair<int, int>>(), new List<KeyValuePair<int, int>>());

    public void Init(TurrentCreater _parent)
    {
        parent = _parent;

        float fieldWidth = (transform as RectTransform).rect.width;

        float width = fieldWidth / (BattleConst.MAP_WIDTH * 2 - 1);

        float scale = width / (cellRes.transform as RectTransform).sizeDelta.x;

        for (int i = 0; i < BattleConst.MAP_HEIGHT; i++)
        {
            for (int m = 0; m < BattleConst.MAP_WIDTH * 2 - 1; m++)
            {
                TurrentCreaterCell cell = Instantiate(cellRes);

                cell.x = m - (BattleConst.MAP_WIDTH - 1);

                cell.y = i;

                cell.gameObject.SetActive(true);

                cell.transform.SetParent(transform, false);

                cell.transform.localScale = new Vector3(scale, scale, 1);

                (cell.transform as RectTransform).anchoredPosition = new Vector2(0.5f * fieldWidth - width * 0.5f - m * width, 0.5f * width + i * width + yFix);

                cellDic.Add(new KeyValuePair<int, int>(cell.x, cell.y), cell);

                SuperFunction.SuperFunctionCallBack0 dele = delegate (int _index)
                {
                    ClickCell(cell);
                };

                SuperFunction.Instance.AddEventListener(cell.gameObject, BattleClickGo.EVENT_NAME, dele);
            }
        }
    }

    private void ClickCell(TurrentCreaterCell _cell)
    {
        if (_cell.state == TurrentCreaterCell.State.NULL)
        {
            _cell.SetState(TurrentCreaterCell.State.TARGET);

            data.Key.Add(new KeyValuePair<int, int>(_cell.x, _cell.y));
        }
        else if (_cell.state == TurrentCreaterCell.State.TARGET)
        {
            _cell.SetState(TurrentCreaterCell.State.SPLASH);

            data.Key.Remove(new KeyValuePair<int, int>(_cell.x, _cell.y));

            data.Value.Add(new KeyValuePair<int, int>(_cell.x, _cell.y));
        }
        else
        {
            _cell.SetState(TurrentCreaterCell.State.NULL);

            data.Value.Remove(new KeyValuePair<int, int>(_cell.x, _cell.y));
        }

        parent.GetScore();
    }

    public void Show(KeyValuePair<List<KeyValuePair<int, int>>, List<KeyValuePair<int, int>>> _data)
    {
        gameObject.SetActive(true);

        data = _data;

        Refresh();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Refresh()
    {
        foreach (var cell in cellDic.Values)
        {
            cell.SetState(TurrentCreaterCell.State.NULL);
        }

        for (int i = 0; i < data.Key.Count; i++)
        {
            KeyValuePair<int, int> target = data.Key[i];

            cellDic[target].SetState(TurrentCreaterCell.State.TARGET);
        }

        for (int i = 0; i < data.Value.Count; i++)
        {
            KeyValuePair<int, int> splash = data.Value[i];

            cellDic[splash].SetState(TurrentCreaterCell.State.SPLASH);
        }
    }

    public void Close()
    {
        Hide();

        parent.Close();
    }
}
