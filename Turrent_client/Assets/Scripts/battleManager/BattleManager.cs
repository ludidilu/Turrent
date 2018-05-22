using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using superFunction;
using Turrent_lib;

public partial class BattleManager : MonoBehaviour
{
    public const string BATTLE_START = "battleStart";

    public const string BATTLE_QUIT = "battleQuit";

    public const string BATTLE_SEND_DATA = "battleSendData";

    public const string BATTLE_RECEIVE_DATA = "battleReceiveData";

    [SerializeField]
    private float turrentGap = 20;

    [SerializeField]
    private Text mMoney;

    [SerializeField]
    private Text oMoney;

    [SerializeField]
    private Text mHp;

    [SerializeField]
    private Text oHp;

    [SerializeField]
    private BattleClickGo myPosRes;

    [SerializeField]
    private BattleClickGo oppPosRes;

    [HideInInspector]
    public GameObject eventGo;

    private BattleClickGo[] myPosArr = new BattleClickGo[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

    private BattleClickGo[] oppPosArr = new BattleClickGo[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

    private Battle_client battle = new Battle_client();

    private bool initOver;

    public void Init(GameObject _eventGo)
    {
        eventGo = _eventGo;

        Dictionary<int, UnitSDS> unitDic = StaticData.GetDic<UnitSDS>();

        Dictionary<int, TurrentSDS> turrentDic = StaticData.GetDic<TurrentSDS>();

        BattleCore.Init(unitDic, turrentDic);

        battle.Init(SendData, RefreshData);

        SuperFunction.Instance.AddEventListener<BinaryReader>(eventGo, BATTLE_RECEIVE_DATA, ReceiveData);

        Debug.Log((transform.root as RectTransform).sizeDelta);

        float screenWidth = (transform.root as RectTransform).sizeDelta.x;

        float screenHeight = (transform.root as RectTransform).sizeDelta.y;

        float width = screenWidth / BattleConst.MAP_WIDTH;

        for (int i = 0; i < BattleConst.MAP_HEIGHT; i++)
        {
            for (int m = 0; m < BattleConst.MAP_WIDTH; m++)
            {
                BattleClickGo go = Instantiate(myPosRes);

                float scale = width / (go.transform as RectTransform).sizeDelta.x;

                go.transform.localScale = new Vector3(scale, scale, 1);

                go.transform.SetParent(transform, false);

                myPosArr[i * BattleConst.MAP_WIDTH + m] = go;

                (go.transform as RectTransform).anchoredPosition = new Vector2(-0.5f * screenWidth + width * 0.5f + m * width, -0.5f * turrentGap - 0.5f * width - i * width);
            }
        }
    }

    public void StartBattle()
    {
        initOver = false;

        battle.ClientRequestRefreshData();
    }

    private void SendData(MemoryStream _ms, Action<BinaryReader> _callBack)
    {
        SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_SEND_DATA, _ms, _callBack);
    }

    private void ReceiveData(int _index, BinaryReader _br)
    {
        if (initOver)
        {
            battle.ClientGetPackage(_br);
        }
    }

    private void RefreshData()
    {
        initOver = true;

        Debug.Log("refreshData");
    }
}
