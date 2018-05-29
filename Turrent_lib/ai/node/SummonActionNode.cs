using bt;
using System;
using System.Collections.Generic;

namespace Turrent_lib
{
    internal class SummonActionNode : ActionNode<BattleCore, bool, AiSummonData>
    {
        public override bool Enter(Func<int, int> _getRandomValueCallBack, BattleCore _t, bool _u, AiSummonData _v)
        {
            int score = 0;

            for (int i = 0; i < _v.summonPosList.Count; i++)
            {
                score += _v.summonPosList[i].Value;
            }

            int index = _getRandomValueCallBack(score);

            for (int i = 0; i < _v.summonPosList.Count; i++)
            {
                KeyValuePair<int, int> pair = _v.summonPosList[i];

                if (index < pair.Value)
                {
                    _v.pos = pair.Key;

                    break;
                }
                else
                {
                    index -= pair.Value;
                }
            }

            return true;
        }
    }
}
