using System.Collections.Generic;
using System.IO;

class PlayerUnitManager
{
    public static PlayerUnitManager Instance;

    private Dictionary<int, PlayerUnit> dic = new Dictionary<int, PlayerUnit>();

    public void Login(int _uid, PlayerUnit _unit)
    {
        PlayerUnit unit;

        if (dic.TryGetValue(_uid, out unit))
        {
            unit.Kick();

            dic[_uid] = _unit;
        }
        else
        {
            dic.Add(_uid, _unit);
        }
    }

    public void Logout(int _uid)
    {
        if (dic.ContainsKey(_uid))
        {
            dic.Remove(_uid);
        }
    }

    public void SendData(int _uid, bool _isPush, MemoryStream _ms)
    {
        PlayerUnit unit;

        if (dic.TryGetValue(_uid, out unit))
        {
            unit.SendData(_isPush, _ms);
        }
    }
}
