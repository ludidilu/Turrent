using System;
using System.Collections.Generic;

namespace Turrent_lib
{
    static class BattlePublicTools
    {
        public static void Shuffle<T>(IList<T> _list, Func<int, int> _getRandomValueCallBack)
        {
            int length = _list.Count;

            while (length > 1)
            {
                int index = _getRandomValueCallBack(length);

                int lastIndex = length - 1;

                T data = _list[index];

                T last = _list[lastIndex];

                _list[index] = last;

                _list[lastIndex] = data;

                length--;
            }
        }

        public static int GetUnitAttackTargetScore(BattleCore _battle, bool _isMine, int _id, int _pos, int _turrentScore, int _baseScore)
        {
            int score = 0;

            IUnitSDS unitSDS = BattleCore.GetUnitData(_id);

            for (int i = 0; i < unitSDS.GetPos().Length; i++)
            {
                int pos = _pos + unitSDS.GetPos()[i];

                ITurrentSDS turrentSDS = unitSDS.GetTurrent()[i];

                List<int> result = GetTurrentAttackTargetList(_battle, _isMine, turrentSDS, pos);

                if (result != null)
                {
                    for (int m = 0; m < result.Count; m++)
                    {
                        int targetPos = result[m];

                        if (targetPos < 0)
                        {
                            score += _baseScore;
                        }
                        else
                        {
                            score += _turrentScore;
                        }
                    }
                }
            }

            return score;
        }

        public static List<int> GetTurrentAttackTargetList(BattleCore _battle, bool _isMine, ITurrentSDS _sds, int _pos)
        {
            List<int> result = null;

            int x = _pos % BattleConst.MAP_WIDTH;

            int oppX = BattleConst.MAP_WIDTH - 1 - x;

            Turrent[] oppTurrent = _isMine ? _battle.oTurrent : _battle.mTurrent;

            for (int i = 0; i < _sds.GetAttackTargetPos().Length; i++)
            {
                KeyValuePair<int, int>[] targetPosFixArr = _sds.GetAttackTargetPos()[i];

                bool getTarget = false;

                for (int n = 0; n < targetPosFixArr.Length; n++)
                {
                    KeyValuePair<int, int> targetPosFix = targetPosFixArr[n];

                    int targetX = oppX + targetPosFix.Key;

                    if (targetX >= BattleConst.MAP_WIDTH || targetX < 0)
                    {
                        continue;
                    }

                    int targetY = targetPosFix.Value;

                    int targetPos = targetY * BattleConst.MAP_WIDTH + targetX;

                    Turrent targetTurrent = oppTurrent[targetPos];

                    if (targetTurrent != null)
                    {
                        if (!getTarget)
                        {
                            getTarget = true;

                            result = new List<int>();
                        }

                        result.Add(targetPos);
                    }
                }

                if (getTarget)
                {
                    KeyValuePair<int, int>[] arr = _sds.GetAttackSplashPos()[i];

                    for (int m = 0; m < arr.Length; m++)
                    {
                        KeyValuePair<int, int> targetPosFix = arr[m];

                        int targetX = oppX + targetPosFix.Key;

                        if (targetX >= BattleConst.MAP_WIDTH || targetX < 0)
                        {
                            continue;
                        }

                        int targetY = targetPosFix.Value;

                        int targetPos = targetY * BattleConst.MAP_WIDTH + targetX;

                        Turrent targetTurrent = oppTurrent[targetPos];

                        if (targetTurrent != null)
                        {
                            result.Add(targetPos);
                        }
                    }

                    return result;
                }
            }

            if (_sds.GetCanAttackBase())
            {
                result = new List<int>();

                result.Add(-1);
            }

            return result;
        }
    }
}
