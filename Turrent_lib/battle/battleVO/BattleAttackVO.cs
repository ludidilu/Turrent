using System.Collections.Generic;

namespace Turrent_lib
{
    public struct BattleAttackVO
    {
        public bool isMine;

        public int pos;

        public List<KeyValuePair<int, int>> damageData;

        public BattleAttackVO(bool _isMine, int _pos, List<KeyValuePair<int, int>> _damageData)
        {
            isMine = _isMine;

            pos = _pos;

            damageData = _damageData;
        }
    }
}
