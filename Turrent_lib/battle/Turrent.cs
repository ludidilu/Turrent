using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Turrent_lib
{
    enum TurrentState
    {
        CD,
        FREE,
    }

    public class Turrent
    {
        public Unit parent;

        public ITurrentSDS sds;

        public int pos;

        private TurrentState state;

        private int time;

        private BattleCore battleCore;

        internal void Init(BattleCore _battleCore, Unit _parent, ITurrentSDS _sds, int _pos)
        {
            battleCore = _battleCore;

            parent = _parent;

            sds = _sds;

            pos = _pos;
        }
    }
}
