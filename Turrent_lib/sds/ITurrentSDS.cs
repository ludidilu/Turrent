using System.Collections.Generic;

namespace Turrent_lib
{
    public interface ITurrentSDS
    {
        int GetCd();
        KeyValuePair<int, int>[][] GetAttackTargetPos();
        KeyValuePair<int, int>[][] GetAttackSplashPos();
        int GetAttackGap();
        int GetAttackDamage();
        bool GetCanAttackBase();
    }
}
