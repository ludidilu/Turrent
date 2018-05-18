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
    }
}
