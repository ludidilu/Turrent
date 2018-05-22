using Turrent_lib;
using System.IO;
using Turrent_proto;

internal class BattleUnit
{
    private int mPlayer;
    private int oPlayer;

    private Battle_server battle;

    private bool isVsAi;

    internal bool processBattle { private set; get; }

    internal BattleUnit(bool _processBattle)
    {
        processBattle = _processBattle;

        battle = new Battle_server(processBattle);

        battle.ServerSetCallBack(SendData);
    }

    internal void Init(int _mPlayer, int _oPlayer, int[] _mCards, int[] _oCards, bool _isVsAi)
    {
        mPlayer = _mPlayer;
        oPlayer = _oPlayer;

        isVsAi = _isVsAi;

        battle.ServerStart(_mCards, _oCards, isVsAi);
    }

    internal void ReceiveData(int _uid, BinaryReader _br)
    {
        bool isMine = _uid == mPlayer;

        battle.ServerGetPackage(_br, isMine);
    }

    private void SendData(bool _isMine, bool _isPush, MemoryStream _ms)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                if (_isPush)
                {
                    bw.Write((int)ScPackageTag.BattleData);
                }

                bw.Write(_ms.GetBuffer(), 0, (int)_ms.Length);

                if (_isMine && mPlayer != -1)
                {
                    BattleManager.Instance.SendData(mPlayer, _isPush, ms);
                }
                else if (!_isMine && oPlayer != -1)
                {
                    BattleManager.Instance.SendData(oPlayer, _isPush, ms);
                }
            }
        }
    }

    public void Update()
    {
        battle.ServerUpdate();
    }

    internal void Logout(int _uid)
    {
    }
}
