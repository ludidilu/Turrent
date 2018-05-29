using System;
using System.Collections.Generic;
using bt;

namespace Turrent_lib
{
    internal class GetSummonIdConditionNode : ConditionNode<BattleCore, bool, AiSummonData>
    {
        public override bool Enter(Func<int, int> _getRandomValueCallBack, BattleCore _t, bool _u, AiSummonData _v)
        {
            List<int> handCards = _u ? _t.mHandCards : _t.oHandCards;

            if (handCards.Count == 0)
            {
                return false;
            }

            handCards = new List<int>(handCards);

            int index = _getRandomValueCallBack(handCards.Count);

            int uid = handCards[index];

            int id = _t.GetCard(_u, uid);

            IUnitSDS sds = BattleCore.GetUnitData(id);

            if (sds.GetCost() > (_u ? _t.mMoney : _t.oMoney))
            {
                return false;
            }
            else
            {
                _v.uid = uid;

                return true;
            }
        }
    }
}
