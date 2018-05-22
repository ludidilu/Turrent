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
    private float cardGap = 5;

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

    [SerializeField]
    private BattleCard cardRes;

    [SerializeField]
    private BattleUnit unitRes;

    [HideInInspector]
    public GameObject eventGo;

    private BattleClickGo[] myPosArr = new BattleClickGo[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

    private BattleClickGo[] oppPosArr = new BattleClickGo[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

    private Turrent[] mTurrent;

    private Turrent[] oTurrent;

    private Dictionary<int, BattleCard> cards = new Dictionary<int, BattleCard>();

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

        InitPos();
    }

    private void InitPos()
    {
        float screenWidth = (transform.root as RectTransform).sizeDelta.x;

        float screenHeight = (transform.root as RectTransform).sizeDelta.y;

        float width = screenWidth / BattleConst.MAP_WIDTH;

        for (int i = 0; i < BattleConst.MAP_HEIGHT; i++)
        {
            for (int m = 0; m < BattleConst.MAP_WIDTH; m++)
            {
                int id = i * BattleConst.MAP_WIDTH + m;

                BattleClickGo go = Instantiate(myPosRes);

                go.gameObject.SetActive(true);

                float scale = width / (go.transform as RectTransform).sizeDelta.x;

                go.transform.localScale = new Vector3(scale, scale, 1);

                go.transform.SetParent(transform, false);

                myPosArr[id] = go;

                (go.transform as RectTransform).anchoredPosition = new Vector2(-0.5f * screenWidth + width * 0.5f + m * width, -0.5f * turrentGap - 0.5f * width - i * width);

                SuperFunction.SuperFunctionCallBack0 dele = delegate (int _index)
                {
                    ClickMyPos(id);
                };

                SuperFunction.Instance.AddEventListener(go.gameObject, BattleClickGo.CLICK_EVENT, dele);
            }
        }

        for (int i = 0; i < BattleConst.MAP_HEIGHT; i++)
        {
            for (int m = 0; m < BattleConst.MAP_WIDTH; m++)
            {
                int id = i * BattleConst.MAP_WIDTH + m;

                BattleClickGo go = Instantiate(oppPosRes);

                go.gameObject.SetActive(true);

                float scale = width / (go.transform as RectTransform).sizeDelta.x;

                go.transform.localScale = new Vector3(scale, scale, 1);

                go.transform.SetParent(transform, false);

                myPosArr[id] = go;

                (go.transform as RectTransform).anchoredPosition = new Vector2(0.5f * screenWidth - width * 0.5f - m * width, 0.5f * turrentGap + 0.5f * width + i * width);

                SuperFunction.SuperFunctionCallBack0 dele = delegate (int _index)
                {
                    ClickOppPos(id);
                };

                SuperFunction.Instance.AddEventListener(go.gameObject, BattleClickGo.CLICK_EVENT, dele);
            }
        }
    }

    private void ClickMyPos(int _id)
    {
        Debug.Log("ClickMyPos:" + _id);
    }

    private void ClickOppPos(int _id)
    {
        Debug.Log("ClickOppPos:" + _id);
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

        InitCards();
    }

    private void InitCards()
    {
        List<int> handCards = battle.GetHandCards();

        if (handCards.Count > 0)
        {
            float screenWidth = (transform.root as RectTransform).sizeDelta.x;

            float screenHeight = (transform.root as RectTransform).sizeDelta.y;

            Vector2 size = (cardRes.transform as RectTransform).sizeDelta;

            float x = (screenWidth - handCards.Count * size.x - (handCards.Count - 1) * cardGap) * 0.5f + size.x * 0.5f - 0.5f * screenWidth;

            float y = -0.5f * screenHeight + size.y * 0.5f;

            for (int i = 0; i < handCards.Count; i++)
            {
                int uid = handCards[i];

                int id = battle.GetCard(uid);

                BattleCard card = Instantiate(cardRes);

                card.gameObject.SetActive(true);

                card.Init(uid, id);

                card.transform.SetParent(transform, false);

                (card.transform as RectTransform).anchoredPosition = new Vector2(x + i * (size.x + cardGap), y);

                SuperFunction.SuperFunctionCallBack0 dele = delegate (int _index)
                {
                    ClickCard(card);
                };

                SuperFunction.Instance.AddEventListener(card.gameObject, BattleClickGo.CLICK_EVENT, dele);

                cards.Add(uid, card);
            }
        }
    }

    private void ClickCard(BattleCard _card)
    {

    }

    private void InitTurrent()
    {
        mTurrent = battle.GetMyTurrent();

        oTurrent = battle.GetOppTurrent();

        Dictionary<Unit, List<Turrent>> dic = new Dictionary<Unit, List<Turrent>>();

        for (int i = 0; i < mTurrent.Length; i++)
        {
            Turrent t = mTurrent[i];

            if (t != null)
            {
                List<Turrent> list;

                if (!dic.TryGetValue(t.parent, out list))
                {
                    list = new List<Turrent>();

                    dic.Add(t.parent, list);
                }
            }
        }
    }
}
