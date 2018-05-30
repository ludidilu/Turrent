using UnityEngine;
using System.IO;
using superFunction;
using Connection;
using System;
using tuple;
using System.Collections;
using Turrent_proto;
using Google.Protobuf;

public class BattleOnline : UIPanel
{
    [SerializeField]
    private GameObject btPVP;

    [SerializeField]
    private GameObject btPVE;

    [SerializeField]
    private GameObject btCancel;

    [SerializeField]
    private GameObject btQuit;

    private Client client;

    public override void Init()
    {
        base.Init();

        client = new Client();

        client.Init(ConfigDictionary.Instance.ip, ConfigDictionary.Instance.port, ReceivePushData, Disconnect);
    }

    private void ReceivePushData(BinaryReader _br)
    {
        ScPackageTag tag = (ScPackageTag)_br.ReadInt32();

        switch (tag)
        {
            case ScPackageTag.BattleData:

                SuperFunction.Instance.DispatchEvent(BattleView.battleManagerEventGo, BattleManager.BATTLE_RECEIVE_DATA, _br);

                break;

            case ScPackageTag.PlayerState:

                PlayerStateMessage message = PlayerStateMessage.Parser.ParseFrom(_br.BaseStream);

                ReceiveReplyData(message);

                break;
        }
    }

    private void ReceiveReplyData(PlayerStateMessage _message)
    {
        SetState(_message.PlayerState);
    }

    private void SetState(PlayerStateEnum _state)
    {
        switch (_state)
        {
            case PlayerStateEnum.Battle:

                btPVP.SetActive(true);

                btPVE.SetActive(true);

                btCancel.SetActive(false);

                btQuit.SetActive(true);

                SuperFunction.Instance.AddOnceEventListener(BattleView.battleManagerEventGo, BattleManager.BATTLE_QUIT, BattleOver);

                SuperFunction.Instance.AddEventListener<MemoryStream, Action<BinaryReader>>(BattleView.battleManagerEventGo, BattleManager.BATTLE_SEND_DATA, SendBattleAction);

                UIManager.Instance.ShowInParent<BattleView>(new Tuple<bool, int, IEnumerator>(false, 0, null), uid);

                break;

            case PlayerStateEnum.Free:

                btPVP.SetActive(true);

                btPVE.SetActive(true);

                btCancel.SetActive(false);

                btQuit.SetActive(true);

                break;

            case PlayerStateEnum.Searching:

                btPVP.SetActive(false);

                btPVE.SetActive(false);

                btCancel.SetActive(true);

                btQuit.SetActive(false);

                break;
        }
    }

    public void EnterPVE()
    {
        BattleManagerActionMessage message = new BattleManagerActionMessage();

        message.BattleManagerAction = BattleManagerActionEnum.Pve;

        message.BattleId = 2;

        SendAction<PlayerStateMessage>(CsPackageTag.BattleManagerAction, message.ToByteArray(), ReceiveReplyData);
    }

    public void EnterPVP()
    {
        BattleManagerActionMessage message = new BattleManagerActionMessage();

        message.BattleManagerAction = BattleManagerActionEnum.Pvp;

        SendAction<PlayerStateMessage>(CsPackageTag.BattleManagerAction, message.ToByteArray(), ReceiveReplyData);
    }

    public void CancelPVP()
    {
        BattleManagerActionMessage message = new BattleManagerActionMessage();

        message.BattleManagerAction = BattleManagerActionEnum.CancelSearching;

        SendAction<PlayerStateMessage>(CsPackageTag.BattleManagerAction, message.ToByteArray(), ReceiveReplyData);
    }

    private void SendAction<T>(CsPackageTag _tag, byte[] _bytes, Action<T> _callBack) where T : IMessage<T>, new()
    {
        Action<BinaryReader> dele = delegate (BinaryReader _br)
        {
            MessageParser<T> parser = new MessageParser<T>(() => new T());

            T result = parser.ParseFrom(_br.BaseStream);

            _callBack(result);
        };

        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)_tag);

                bw.Write(_bytes);

                client.Send(ms, dele);
            }
        }
    }

    private void SendBattleAction(int _index, MemoryStream _ms, Action<BinaryReader> _callBack)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)CsPackageTag.BattleData);

                bw.Write(_ms.GetBuffer(), 0, (int)_ms.Length);

                client.Send(ms, _callBack);
            }
        }
    }

    public override void OnEnter()
    {
        btPVP.SetActive(false);

        btPVE.SetActive(false);

        btCancel.SetActive(false);

        btQuit.SetActive(false);

        client.Connect(ConnectOver);
    }

    private void ConnectOver(bool _b)
    {
        if (_b)
        {
            LoginMessage loginMessage = new LoginMessage();

            loginMessage.Name = ConfigDictionary.Instance.uid.ToString();

            SendAction<PlayerStateMessage>(CsPackageTag.Login, loginMessage.ToByteArray(), ReceiveReplyData);
        }
        else
        {
            UIManager.Instance.Hide(uid);
        }
    }

    private void Disconnect()
    {
        Quit();
    }

    public void BattleOver(int _index)
    {
        SuperFunction.Instance.RemoveEventListener<MemoryStream, Action<BinaryReader>>(BattleView.battleManagerEventGo, BattleManager.BATTLE_SEND_DATA, SendBattleAction);
    }

    public void Quit()
    {
        client.Close();

        UIManager.Instance.Hide(uid);
    }

    void Update()
    {
        client.Update();
    }
}
