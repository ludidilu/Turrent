using System.Collections.Generic;

namespace Turrent_lib
{
    public enum DamageType
    {
        PHYSIC,
        MAGIC,
    }

    public enum UpdateType
    {
        ALWAYS_UPDATE,
        CD_UPDATE,
        DO_NOT_UPDATE,
    }

    public interface ITurrentSDS
    {
        UpdateType GetUpdateType();
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
