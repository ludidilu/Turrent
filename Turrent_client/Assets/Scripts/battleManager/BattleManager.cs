using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using superFunction;
using Turrent_lib;
using superEnumerator;

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

    private BattleClickGo[] mPosArr = new BattleClickGo[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

    private BattleClickGo[] oPosArr = new BattleClickGo[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

    private Turrent[] mTurrent;

    private Turrent[] oTurrent;

    private Dictionary<int, BattleCard> cards = new Dictionary<int, BattleCard>();

    private Dictionary<int, BattleUnit> mTurrentDic = new Dictionary<int, BattleUnit>();

    private Dictionary<int, BattleUnit> oTurrentDic = new Dictionary<int, BattleUnit>();

    private Battle_client battle = new Battle_client();

    private bool initOver;

    public void Init(GameObject _eventGo)
    {
        eventGo = _eventGo;

        Dictionary<int, UnitSDS> unitDic = StaticData.GetDic<UnitSDS>();

        Dictionary<int, TurrentSDS> turrentDic = StaticData.GetDic<TurrentSDS>();

        BattleCore.Init(unitDic, turrentDic);

        battle.Init(SendData, RefreshData, ClientUpdate);

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

                mPosArr[id] = go;

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

                oPosArr[id] = go;

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

        Reset();

        InitCards();

        InitTurrent();
    }

    private void Reset()
    {
        IEnumerator<BattleCard> enumerator = cards.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            Destroy(enumerator.Current.gameObject);
        }

        cards.Clear();

        IEnumerator<BattleUnit> enumerator2 = mTurrentDic.Values.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            Destroy(enumerator2.Current.gameObject);
        }

        mTurrentDic.Clear();

        enumerator2 = oTurrentDic.Values.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            Destroy(enumerator2.Current.gameObject);
        }

        oTurrentDic.Clear();
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

                int id = battle.GetCard(battle.clientIsMine, uid);

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

        Dictionary<Unit, Turrent> dic = new Dictionary<Unit, Turrent>();

        for (int i = 0; i < mTurrent.Length; i++)
        {
            Turrent t = mTurrent[i];

            if (t != null)
            {
                Turrent tt;

                if (!dic.TryGetValue(t.parent, out tt))
                {
                    dic.Add(t.parent, t);
                }
                else if (t.pos < tt.pos)
                {
                    dic[t.parent] = t;
                }
            }
        }

        IEnumerator<KeyValuePair<Unit, Turrent>> enumerator = dic.GetEnumerator();

        while (enumerator.MoveNext())
        {
            BattleUnit unit = Instantiate(unitRes);

            unit.gameObject.SetActive(true);

            unit.transform.SetParent(transform, false);

            unit.Init(true, enumerator.Current.Key);

            (unit.transform as RectTransform).anchoredPosition = (mPosArr[enumerator.Current.Value.pos].transform as RectTransform).anchoredPosition;

            mTurrentDic.Add(enumerator.Current.Value.pos, unit);
        }

        dic.Clear();

        for (int i = 0; i < oTurrent.Length; i++)
        {
            Turrent t = oTurrent[i];

            if (t != null)
            {
                Turrent tt;

                if (!dic.TryGetValue(t.parent, out tt))
                {
                    dic.Add(t.parent, t);
                }
                else if (t.pos < tt.pos)
                {
                    dic[t.parent] = t;
                }
            }
        }

        enumerator = dic.GetEnumerator();

        while (enumerator.MoveNext())
        {
            BattleUnit unit = Instantiate(unitRes);

            unit.gameObject.SetActive(true);

            unit.transform.SetParent(transform, false);

            unit.Init(false, enumerator.Current.Key);

            (unit.transform as RectTransform).anchoredPosition = (oPosArr[enumerator.Current.Value.pos].transform as RectTransform).anchoredPosition;

            oTurrentDic.Add(enumerator.Current.Value.pos, unit);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            List<int> handCards = battle.GetHandCards();

            if (handCards.Count > 0)
            {
                int uid = handCards[0];

                int result = battle.ClientRequestAddAction(uid, 2);

                Debug.Log("result:" + result);
            }
        }
        else if (Input.GetKeyUp(KeyCode.F5))
        {
            battle.ClientRequestRefreshData();
        }
    }

    private void ClientUpdate(SuperEnumerator<ValueType> _superEnumerator)
    {
        bool needRefresh = false;

        while (_superEnumerator.MoveNext())
        {
            needRefresh = true;

            ValueType ss = _superEnumerator.Current;
        }

        if (needRefresh)
        {
            RefreshData();
        }

        if (battle.clientIsMine)
        {
            mHp.text = battle.mBase.ToString();

            oHp.text = battle.oBase.ToString();

            mMoney.text = battle.mMoney.ToString();

            oMoney.text = battle.oMoney.ToString();
        }
        else
        {
            mHp.text = battle.oBase.ToString();

            oHp.text = battle.mBase.ToString();

            mMoney.text = battle.oMoney.ToString();

            oMoney.text = battle.mMoney.ToString();
        }
    }
}
