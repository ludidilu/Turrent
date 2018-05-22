using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using superRaycast;
using screenScale;
using superFunction;
using superGraphicRaycast;
using superSequenceControl;
using System.Collections;
using gameObjectFactory;
using superTween;
using publicTools;
using Turrent_lib;

public partial class BattleManager : MonoBehaviour
{
    public const string BATTLE_START = "battleStart";

    public const string BATTLE_QUIT = "battleQuit";

    public const string BATTLE_SEND_DATA = "battleSendData";

    public const string BATTLE_RECEIVE_DATA = "battleReceiveData";

    [HideInInspector]
    public GameObject eventGo;

    private Battle_client battle = new Battle_client();

    public void Init(GameObject _eventGo)
    {
        eventGo = _eventGo;

        Dictionary<int, UnitSDS> unitDic = StaticData.GetDic<UnitSDS>();

        Dictionary<int, TurrentSDS> turrentDic = StaticData.GetDic<TurrentSDS>();

        BattleCore.Init(unitDic, turrentDic);

        battle.Init(SendData, RefreshData);

        SuperFunction.Instance.AddEventListener<BinaryReader>(eventGo, BATTLE_RECEIVE_DATA, ReceiveData);
    }

    private void SendData(MemoryStream _ms, Action<BinaryReader> _callBack)
    {
        SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_SEND_DATA, _ms, _callBack);
    }

    private void ReceiveData(int _index, BinaryReader _br)
    {
        battle.ClientGetPackage(_br);
    }

    private void RefreshData()
    {
    }

    public void RequestRefreshData()
    {

    }
}
