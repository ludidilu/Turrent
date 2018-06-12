using System.Collections.Generic;

namespace Turrent_lib
{
    public enum DamageType
    {
        PHYSIC,
        MAGIC,
    }

    public interface ITurrentSDS
    {
        int GetCd();
        KeyValuePair<int, int>[][] GetAttackTargetPos();
        KeyValuePair<int, int>[][] GetAttackSplashPos();
        int GetAttackGap();
        int GetAttackDamage();
        DamageType GetAttackDamageType();
        bool GetCanAttackBase();

        int GetAttackDamageAdd();
        int GetAttackDamageAddMax();
        int GetAttackDamageAddGap();
    }
}
