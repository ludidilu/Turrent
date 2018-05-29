using System;
using System.Collections.Generic;
using bt;

namespace Turrent_lib
{
    internal class GetSummonPosConditionNode : ConditionNode<BattleCore, bool, AiSummonData>
    {
        private int turrentTargetScore;

        private int baseTargetScore;

        public override bool Enter(Func<int, int> _getRandomValueCallBack, BattleCore _t, bool _u, AiSummonData _v)
        {
            Turrent[] turrentArr = _u ? _t.mTurrent : _t.oTurrent;

            int id = _t.GetCard(_u, _v.uid);

            IUnitSDS unitSDS = BattleCore.GetUnitData(id);

            for (int i = 0; i < BattleConst.MAP_HEIGHT; i++)
            {
                if (Array.IndexOf(unitSDS.GetRow(), i) != -1)
                {
                    for (int m = 0; m < BattleConst.MAP_WIDTH; m++)
                    {
                        int unitPos = i * BattleConst.MAP_WIDTH + m;

                        bool canSet = true;

                        for (int n = 0; n < unitSDS.GetPos().Length; n++)
                        {
                            int posFix = unitSDS.GetPos()[n];

                            int x = posFix % BattleConst.MAP_WIDTH;

                            if (m + x < BattleConst.MAP_WIDTH)
                            {
                                int pos = unitPos + posFix;

                                if (turrentArr[pos] != null)
                                {
                                    canSet = false;

                                    break;
                                }
                            }
                            else
                            {
                                canSet = false;

                                break;
                            }
                        }

                        if (canSet)
                        {
                            int score = BattlePublicTools.GetUnitAttackTargetScore(_t, _u, id, unitPos, turrentTargetScore, baseTargetScore);

                            if (score > 0)
                            {
                                _v.summonPosList.Add(new KeyValuePair<int, int>(unitPos, score));
                            }
                        }
                    }
                }
            }

            return _v.summonPosList.Count > 0;
        }
    }
}
