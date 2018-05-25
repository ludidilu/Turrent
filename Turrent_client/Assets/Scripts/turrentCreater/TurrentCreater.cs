using System.Collections.Generic;
using UnityEngine;
using superList;
using UnityEngine.UI;

public class TurrentCreater : MonoBehaviour
{
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
    private Text score;

    private int cdValue;

    private int attackGapValue;

    private int attackDamageValue;

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
    }

    private void ValueChange(string _str)
    {

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

        Debug.Log(targetStr + "," + splashStr);
    }
}
