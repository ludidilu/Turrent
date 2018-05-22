using System.Collections.Generic;
using System.IO;
using Turrent_proto;
using Google.Protobuf;

internal class BattleManager
{
    public static BattleManager Instance;

    private const int PVP_BATTLE_ID = 2;

    private const bool processBattle = true;

    private Queue<BattleUnit> battleUnitPool1 = new Queue<BattleUnit>();

    private Queue<BattleUnit> battleUnitPool2 = new Queue<BattleUnit>();

    private Dictionary<int, BattleUnit> battleDic = new Dictionary<int, BattleUnit>();

    private List<BattleUnit> battleList = new List<BattleUnit>();

    private int lastPlayer = -1;

    internal void Login(int _playerUnit)
    {
        PlayerStateEnum playerState;

        if (lastPlayer == _playerUnit)
        {
            playerState = PlayerStateEnum.Searching;
        }
        else
        {
            if (battleDic.ContainsKey(_playerUnit))
            {
                playerState = PlayerStateEnum.Battle;
            }
            else
            {
                playerState = PlayerStateEnum.Free;
            }
        }

        ReplyClient(_playerUnit, false, playerState);
    }

    internal void Logout(int _playerUnit)
    {
        if (lastPlayer == _playerUnit)
        {
            lastPlayer = -1;
        }
    }

    internal void ReceiveData(int _playerUnit, byte[] _bytes)
    {
        using (MemoryStream ms = new MemoryStream(_bytes))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                CsPackageTag tag = (CsPackageTag)br.ReadInt32();

                switch (tag)
                {
                    case CsPackageTag.BattleData:

                        BattleUnit battleUnit;

                        if (battleDic.TryGetValue(_playerUnit, out battleUnit))
                        {
                            battleUnit.ReceiveData(_playerUnit, br);
                        }
                        else
                        {
                            if (_playerUnit == lastPlayer)
                            {
                                ReplyClient(_playerUnit, false, PlayerStateEnum.Searching);
                            }
                            else
                            {
                                ReplyClient(_playerUnit, false, PlayerStateEnum.Free);
                            }
                        }

                        break;

                    case CsPackageTag.BattleManagerAction:

                        if (battleDic.ContainsKey(_playerUnit))
                        {
                            ReplyClient(_playerUnit, false, PlayerStateEnum.Battle);
                        }
                        else
                        {
                            BattleManagerActionMessage message = BattleManagerActionMessage.Parser.ParseFrom(ms);

                            ReceiveActionData(_playerUnit, message);
                        }
                        break;
                }
            }
        }
    }

    private void ReceiveActionData(int _playerUnit, BattleManagerActionMessage _message)
    {
        BattleUnit battleUnit;

        BattleSDS battleSDS;

        switch (_message.BattleManagerAction)
        {
            case BattleManagerActionEnum.Pvp:

                if (lastPlayer == -1)
                {
                    lastPlayer = _playerUnit;

                    ReplyClient(_playerUnit, false, PlayerStateEnum.Searching);
                }
                else if (lastPlayer == _playerUnit)
                {
                    ReplyClient(_playerUnit, false, PlayerStateEnum.Searching);
                }
                else
                {
                    battleUnit = GetBattleUnit(processBattle);

                    battleList.Add(battleUnit);

                    int tmpPlayer = lastPlayer;

                    lastPlayer = -1;

                    battleDic.Add(_playerUnit, battleUnit);

                    battleDic.Add(tmpPlayer, battleUnit);

                    battleSDS = StaticData.GetData<BattleSDS>(PVP_BATTLE_ID);

                    battleUnit.Init(_playerUnit, tmpPlayer, battleSDS.mCards, battleSDS.oCards, false);

                    ReplyClient(_playerUnit, false, PlayerStateEnum.Battle);

                    ReplyClient(tmpPlayer, true, PlayerStateEnum.Battle);
                }

                break;

            case BattleManagerActionEnum.Pve:

                if (lastPlayer == _playerUnit)
                {
                    lastPlayer = -1;
                }

                int battleID = _message.BattleId;

                battleUnit = GetBattleUnit(processBattle);

                battleList.Add(battleUnit);

                battleDic.Add(_playerUnit, battleUnit);

                battleSDS = StaticData.GetData<BattleSDS>(battleID);

                battleUnit.Init(_playerUnit, -1, battleSDS.mCards, battleSDS.oCards, true);

                ReplyClient(_playerUnit, false, PlayerStateEnum.Battle);

                break;

            case BattleManagerActionEnum.CancelSearching:

                if (lastPlayer == _playerUnit)
                {
                    lastPlayer = -1;
                }

                ReplyClient(_playerUnit, false, PlayerStateEnum.Free);

                break;
        }
    }

    private void ReplyClient(int _uid, bool _isPush, PlayerStateEnum _playerState)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                if (_isPush)
                {
                    bw.Write((int)ScPackageTag.PlayerState);
                }

                PlayerStateMessage message = new PlayerStateMessage();

                message.PlayerState = _playerState;

                bw.Write(message.ToByteArray());

                SendData(_uid, _isPush, ms);
            }
        }
    }

    internal void BattleOver(BattleUnit _battleUnit, int _mPlayer, int _oPlayer)
    {
        if (_mPlayer != -1)
        {
            battleDic.Remove(_mPlayer);
        }

        if (_oPlayer != -1)
        {
            battleDic.Remove(_oPlayer);
        }

        ReleaseBattleUnit(_battleUnit);
    }

    private BattleUnit GetBattleUnit(bool _processBattle)
    {
        BattleUnit battleUnit;

        if (_processBattle && battleUnitPool1.Count > 0)
        {
            battleUnit = battleUnitPool1.Dequeue();
        }
        else if (!_processBattle && battleUnitPool2.Count > 0)
        {
            battleUnit = battleUnitPool2.Dequeue();
        }
        else
        {
            battleUnit = new BattleUnit(_processBattle);
        }

        return battleUnit;
    }

    private void ReleaseBattleUnit(BattleUnit _battleUnit)
    {
        if (_battleUnit.processBattle)
        {
            battleUnitPool1.Enqueue(_battleUnit);
        }
        else
        {
            battleUnitPool2.Enqueue(_battleUnit);
        }
    }

    public void SendData(int _uid, bool _isPush, MemoryStream _ms)
    {
        PlayerUnitManager.Instance.SendData(_uid, _isPush, _ms);
    }

    internal void Update()
    {
        for (int i = 0; i < battleList.Count; i++)
        {
            battleList[i].Update();
        }
    }
}