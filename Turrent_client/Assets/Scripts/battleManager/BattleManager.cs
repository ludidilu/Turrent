using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using superFunction;
using Turrent_lib;
using superEnumerator;
using superTween;

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
    private float yFix = 50;

    [SerializeField]
    private Text mMoney;

    [SerializeField]
    private Text oMoney;

    [SerializeField]
    private BattleClickGo myPosRes;

    [SerializeField]
    private BattleClickGo oppPosRes;

    [SerializeField]
    private BattleCard cardRes;

    [SerializeField]
    private BattleUnit unitRes;

    [SerializeField]
    private BattleBase baseRes;

    [SerializeField]
    private BattleMouseDownGo clickBg;

    [SerializeField]
    private LineRenderer arrowRes;

    [HideInInspector]
    public GameObject eventGo;

    private BattleClickGo[] mPosArr = new BattleClickGo[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

    private BattleClickGo[] oPosArr = new BattleClickGo[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

    private BattleBase mBase;

    private BattleBase oBase;

    private Turrent[] mTurrent;

    private Turrent[] oTurrent;

    private Dictionary<int, BattleCard> cards = new Dictionary<int, BattleCard>();

    private Dictionary<int, BattleUnit> mTurrentDic = new Dictionary<int, BattleUnit>();

    private Dictionary<int, BattleUnit> oTurrentDic = new Dictionary<int, BattleUnit>();

    private Battle_client battle = new Battle_client();

    private int nowSelectedCardUid = -1;

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

        InitClickBg();
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

                (go.transform as RectTransform).anchoredPosition = new Vector2(-0.5f * screenWidth + width * 0.5f + m * width, -0.5f * turrentGap - 0.5f * width - i * width + yFix);

                SuperFunction.SuperFunctionCallBack0 dele = delegate (int _index)
                {
                    ClickMyPos(id);
                };

                SuperFunction.Instance.AddEventListener(go.gameObject, BattleClickGo.EVENT_NAME, dele);
            }
        }

        mBase = Instantiate(baseRes);

        mBase.transform.SetParent(transform, false);

        mBase.gameObject.SetActive(true);

        (mBase.transform as RectTransform).anchoredPosition = new Vector2(-0.5f * screenWidth + width * 0.5f + (float)(BattleConst.MAP_WIDTH - 1) / 2 * width, -0.5f * turrentGap - 0.5f * width - BattleConst.MAP_HEIGHT * width + yFix);

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

                (go.transform as RectTransform).anchoredPosition = new Vector2(0.5f * screenWidth - width * 0.5f - m * width, 0.5f * turrentGap + 0.5f * width + i * width + yFix);

                SuperFunction.SuperFunctionCallBack0 dele = delegate (int _index)
                {
                    ClickOppPos(id);
                };

                SuperFunction.Instance.AddEventListener(go.gameObject, BattleClickGo.EVENT_NAME, dele);
            }
        }

        oBase = Instantiate(baseRes);

        oBase.transform.SetParent(transform, false);

        oBase.gameObject.SetActive(true);

        (oBase.transform as RectTransform).anchoredPosition = new Vector2(0.5f * screenWidth - width * 0.5f - (float)(BattleConst.MAP_WIDTH - 1) / 2 * width, 0.5f * turrentGap + 0.5f * width + BattleConst.MAP_HEIGHT * width + yFix);
    }

    private void InitClickBg()
    {
        SuperFunction.Instance.AddEventListener(clickBg.gameObject, BattleMouseDownGo.EVENT_NAME, BgMouseDown);
    }

    private void BgMouseDown(int _index)
    {
        Debug.Log("BgMouseDown");

        nowSelectedCardUid = -1;

        RefreshSelectedCard();
    }

    private void ClickMyPos(int _id)
    {
        Debug.Log("ClickMyPos:" + _id);

        if (nowSelectedCardUid != -1)
        {
            int result = battle.CheckAddSummon(battle.clientIsMine, nowSelectedCardUid, _id);

            Debug.Log("CheckAddSummon:" + result);

            if (result == -1)
            {
                battle.ClientRequestAddAction(nowSelectedCardUid, _id);
            }
            else
            {
                nowSelectedCardUid = -1;

                RefreshSelectedCard();
            }
        }
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

        Reset();

        InitCards();

        InitTurrent();

        RefreshHpAndMoney();
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

            bool getSelectedCard = false;

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

                SuperFunction.Instance.AddEventListener(card.gameObject, BattleClickGo.EVENT_NAME, dele);

                cards.Add(uid, card);

                if (card.uid == nowSelectedCardUid)
                {
                    getSelectedCard = true;

                    card.SetSelected(true);
                }
            }

            if (!getSelectedCard)
            {
                nowSelectedCardUid = -1;
            }
        }
    }

    private void ClickCard(BattleCard _card)
    {
        Debug.Log("ClickCard");

        nowSelectedCardUid = _card.uid;

        RefreshSelectedCard();
    }

    private void RefreshSelectedCard()
    {
        IEnumerator<BattleCard> enumerator = cards.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            enumerator.Current.SetSelected(enumerator.Current.uid == nowSelectedCardUid);
        }
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
            LineRenderer lr = Instantiate(arrowRes);

            lr.SetPositions(new Vector3[] { mPosArr[0].transform.position, oPosArr[0].transform.position });
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

            if (ss is BattleAttackVO)
            {
                ShowAttack((BattleAttackVO)ss);
            }
        }

        if (needRefresh)
        {
            RefreshData();
        }

        RefreshHpAndMoney();
    }

    private void ShowAttack(BattleAttackVO _vo)
    {
        BattleClickGo[] mArr;

        BattleClickGo[] oArr;

        BattleBase targetBase;

        if (battle.clientIsMine == _vo.isMine)
        {
            mArr = mPosArr;

            oArr = oPosArr;

            targetBase = oBase;
        }
        else
        {
            mArr = oPosArr;

            oArr = mPosArr;

            targetBase = mBase;
        }

        List<GameObject> list = null;

        for (int i = 0; i < _vo.damageData.Count; i++)
        {
            KeyValuePair<int, int> pair = _vo.damageData[i];

            if (pair.Key != -1)
            {
                LineRenderer lr = Instantiate(arrowRes);

                lr.SetPositions(new Vector3[] { mArr[_vo.pos].transform.position, oArr[pair.Key].transform.position });

                if (list == null)
                {
                    list = new List<GameObject>();
                }

                list.Add(lr.gameObject);
            }
            else
            {
                LineRenderer lr = Instantiate(arrowRes);

                lr.SetPositions(new Vector3[] { mArr[_vo.pos].transform.position, targetBase.transform.position });

                if (list == null)
                {
                    list = new List<GameObject>();
                }

                list.Add(lr.gameObject);
            }
        }

        if (list != null)
        {
            Action dele = delegate ()
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Destroy(list[i]);
                }
            };

            SuperTween.Instance.DelayCall(0.1f, dele);
        }
    }

    private void RefreshHpAndMoney()
    {
        if (battle.clientIsMine)
        {
            mBase.SetHp(battle.mBase);

            oBase.SetHp(battle.oBase);

            mMoney.text = battle.mMoney.ToString();

            oMoney.text = battle.oMoney.ToString();
        }
        else
        {
            mBase.SetHp(battle.oBase);

            oBase.SetHp(battle.mBase);

            mMoney.text = battle.oMoney.ToString();

            oMoney.text = battle.mMoney.ToString();
        }
    }
}
