using System.Collections.Generic;

namespace Turrent_lib
{
    public interface ITurrentSDS
    {
        int GetCd();
        KeyValuePair<int, int>[] GetAttackTargetPos();
        KeyValuePair<int, int>[][] GetAttackDamagePos();
        int GetAttackGap();
        int GetAttackDamage();
    }
}
