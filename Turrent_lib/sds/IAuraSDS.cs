namespace Turrent_lib
{
    public enum AuraType
    {
        ADD_INT,
        CAST_SKILL,
        SET_INT,
        MULTI_INT,
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
        ENEMY,
    }

    public enum AuraTarget
    {
        OWNER,
        TRIGGER,
        OTHER,
    }

    public enum EffectType
    {
        PHYSIC_DAMAGE,
        MAGIC_DAMAGE,
        ADD_AURA,
    }

    public interface IAuraSDS
    {
        string GetEventName();
        AuraType GetEffectType();
        int GetPriority();
        AuraTrigger GetTrigger();
        int GetTime();
        AuraTarget GetEffectTarget();
        int[] GetEffectData();
        string[] GetRemoveEventNames();
    }
}
