using Connection;
using System.IO;
using System;
using Turrent_proto;

internal class PlayerUnit : UnitBase
{
    private int uid = -1;

    public override void Kick()
    {
        if (uid != -1)
        {
            BattleManager.Instance.Logout(uid);

            uid = -1;
        }

        base.Kick();
    }

    public override void ReceiveData(byte[] _bytes)
    {
        if (uid != -1)
        {
            BattleManager.Instance.ReceiveData(uid, _bytes);
        }
        else
        {
            using (MemoryStream ms = new MemoryStream(_bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    try
                    {
                        CsPackageTag tag = (CsPackageTag)br.ReadInt32();

                        if (tag == CsPackageTag.Login)
                        {
                            LoginMessage message = LoginMessage.Parser.ParseFrom(ms);

                            uid = int.Parse(message.Name);

                            PlayerUnitManager.Instance.Login(uid, this);

                            BattleManager.Instance.Login(uid);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write(e.ToString());
                    }
                }
            }
        }
    }
}