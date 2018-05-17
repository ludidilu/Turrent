using System;
using System.Collections.Generic;

namespace Turrent_lib
{
    public class Unit
    {
        public bool isMine;

        public IUnitSDS sds;

        public int hp;

        private BattleCore battleCore;

        public void Init(BattleCore _battleCore, bool _isMine, IUnitSDS _sds, int _pos)
        {
            battleCore = _battleCore;

            isMine = _isMine;

            sds = _sds;
        }
    }
}
