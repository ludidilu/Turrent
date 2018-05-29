using System;
using bt;

namespace Turrent_lib
{
    class CheckDoSummonConditionNode : ConditionNode<BattleCore, bool, AiSummonData>
    {
        private int value;

        public override bool Enter(Func<int, int> _getRandomValueCallBack, BattleCore _t, bool _u, AiSummonData _v)
        {
            return _getRandomValueCallBack(value) == 0;
        }
    }
}
