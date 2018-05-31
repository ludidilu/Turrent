using System;

namespace Turrent_lib
{
    public enum AuraType
    {
        FIX_INT,
        CAST_SKILL,
        SET_INT,
    }

    public enum AuraTrigger
    {
        OWNER,
        OWNER_NEIGHBOUR_ALLY,
        OWNER_ALLY,
        OWNER_COL_ALLY,
        OWNER_ROW_ALLY,
        OWNER_FRONT_ALLY,
        OWNER_BACK_ALLY,
        OWNER_BESIDE_ALLY,
    }

    public interface IAuraSDS
    {
        string GetEventName();
        AuraType GetEffectType();
        int GetPriority();
        AuraTrigger GetTrigger();
        int GetTime();
        int[] GetEffectData();
        string[] GetRemoveEventNames();
    }
}
