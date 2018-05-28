using System.Collections.Generic;
using UnityEngine;
using superList;
using UnityEngine.UI;
using superFunction;

public class TurrentCreater : MonoBehaviour
{
    [SerializeField]
    private int scoreTimeLong;

    [SerializeField]
    private float attackBaseFix;

    [SerializeField]
    private float attackBaseOnlyFix;

    [SerializeField]
    private SuperList superList;

    [SerializeField]
    private TurrentCreaterField field;

    [SerializeField]
    private GameObject deleteBt;

    [SerializeField]
    private InputField cd;

    [SerializeField]
    private InputField attackGap;

    [SerializeField]
    private InputField attackDamage;

    [SerializeField]
    private InputField liveTime;

    [SerializeField]
    private Text score;

    [SerializeField]
    private Toggle canAttackBase;

    [SerializeField]
    private BattleMouseDownGo outputBg;

    [SerializeField]
    private InputField output;

    private List<KeyValuePair<List<KeyValuePair<int, int>>, List<KeyValuePair<int, int>>>> list = new List<KeyValuePair<List<KeyValuePair<int, int>>, List<KeyValuePair<int, int>>>>();

    private int chooseIndex = -1;

    // Use this for initialization
    void Start()
    {
        field.Init(this);

        field.Hide();

        superList.SetData(list);

        superList.CellClickIndexHandle = Choose;

        cd.onValueChanged.AddListener(ValueChange);

        attackGap.onValueChanged.AddListener(ValueChange);

        attackDamage.onValueChanged.AddListener(ValueChange);

        liveTime.onValueChanged.AddListener(ValueChange);

        canAttackBase.onValueChanged.AddListener(ValueChange);

        SuperFunction.Instance.AddEventListener(outputBg.gameObject, BattleMouseDownGo.EVENT_NAME, OutputBgDown);

        GetScore();
    }

    private void OutputBgDown(int _index)
    {
        outputBg.gameObject.SetActive(false);
    }

    private void ValueChange(string _str)
    {
        GetScore();
    }

    private void ValueChange(bool _b)
    {
        GetScore();
    }

    public void GetScore()
    {
        int cdValue = string.IsNullOrEmpty(cd.text) ? 0 : int.Parse(cd.text);

        int attackGapValue = string.IsNullOrEmpty(attackGap.text) ? 0 : int.Parse(attackGap.text);

        int attackDamageValue = string.IsNullOrEmpty(attackDamage.text) ? 0 : int.Parse(attackDamage.text);

        int liveTimeValue = string.IsNullOrEmpty(liveTime.text) ? 0 : int.Parse(liveTime.text);

        if (attackGapValue == 0 || attackDamageValue == 0)
        {
            score.text = "0";
        }
        else
        {
            float targetNum = 0;

            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var pair = list[i];

                    float num = pair.Key.Count + pair.Value.Count;

                    targetNum += Mathf.Sqrt(num);
                }

                targetNum /= list.Count;

                if (canAttackBase.isOn)
                {
                    targetNum += attackBaseFix;
                }
            }
            else
            {
                if (canAttackBase.isOn)
                {
                    targetNum = attackBaseOnlyFix;
                }
            }

            if (targetNum > 0)
            {
                int scoreValue;

                if (liveTimeValue == 0)
                {
                    scoreValue = (int)(targetNum * attackDamageValue * (((float)scoreTimeLong - cdValue) / attackGapValue));
                }
                else
                {
                    if (liveTimeValue == cdValue)
                    {
                        scoreValue = (int)(targetNum * attackDamageValue * ((float)scoreTimeLong - cdValue) / scoreTimeLong);
                    }
                    else
                    {
                        scoreValue = (int)(targetNum * attackDamageValue * ((int.Parse(liveTime.text) - cdValue) / attackGapValue));
                    }
                }

                score.text = scoreValue.ToString();
            }
            else
            {
                score.text = "0";
            }
        }
    }

    private void Choose(int _index)
    {
        chooseIndex = _index;

        field.Show(list[chooseIndex]);

        deleteBt.SetActive(true);
    }

    public void Add()
    {
        KeyValuePair<List<KeyValuePair<int, int>>, List<KeyValuePair<int, int>>> data = new KeyValuePair<List<KeyValuePair<int, int>>, List<KeyValuePair<int, int>>>(new List<KeyValuePair<int, int>>(), new List<KeyValuePair<int, int>>());

        list.Add(data);

        field.Show(data);

        superList.SetData(list);

        superList.SetSelectedIndex(list.Count - 1);
    }

    public void Delete()
    {
        field.Hide();

        list.RemoveAt(superList.GetSelectedIndex());

        superList.SetData(list);

        superList.SetSelectedIndex(-1);

        GetScore();
    }

    public void Close()
    {
        superList.SetSelectedIndex(-1);

        deleteBt.SetActive(false);
    }

    public void Output()
    {
        string targetStr = string.Empty;

        string splashStr = string.Empty;

        for (int i = 0; i < list.Count; i++)
        {
            var pair = list[i];

            List<KeyValuePair<int, int>> target = pair.Key;

            for (int m = 0; m < target.Count; m++)
            {
                var pos = target[m];

                targetStr += pos.Key + ":" + pos.Value;

                if (m < target.Count - 1)
                {
                    targetStr += "&";
                }
            }

            if (i < list.Count - 1)
            {
                targetStr += "$";
            }

            List<KeyValuePair<int, int>> splash = pair.Value;

            for (int m = 0; m < splash.Count; m++)
            {
                var pos = splash[m];

                splashStr += pos.Key + ":" + pos.Value;

                if (m < splash.Count - 1)
                {
                    splashStr += "&";
                }
            }

            if (i < list.Count - 1)
            {
                splashStr += "$";
            }
        }

        string result = targetStr + "," + splashStr + "," + (string.IsNullOrEmpty(cd.text) ? "0" : cd.text) + "," + (string.IsNullOrEmpty(liveTime.text) ? "0" : liveTime.text) + "," + (string.IsNullOrEmpty(attackGap.text) ? "0" : attackGap.text) + "," + (string.IsNullOrEmpty(attackDamage.text) ? "0" : attackDamage.text) + "," + (canAttackBase.isOn ? "1" : "0") + "," + score.text;

        outputBg.gameObject.SetActive(true);

        output.text = result;
    }
}
