using System.Collections.Generic;

namespace Turrent_lib
{
    public struct BattleAttackVO
    {
        public int pos;

        public List<KeyValuePair<int, int>> damageData;

        public BattleAttackVO(int _pos, List<KeyValuePair<int, int>> _damageData)
        {
            pos = _pos;

            damageData = _damageData;
        }
    }
}
