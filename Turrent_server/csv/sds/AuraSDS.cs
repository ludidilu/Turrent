using Turrent_lib;

public partial class AuraSDS : CsvBase, IAuraSDS
{
    public string eventName;
    public int auraType;
    public int priority;
    public int trigger;
    public int time;
    public int effectTarget;
    public int[] effectData;
    public string[] removeEventNames;

    public string GetEventName()
    {
        return eventName;
    }

    public AuraType GetAuraType()
    {
        return (AuraType)auraType;
    }

    public int GetPriority()
    {
        return priority;
    }

    public AuraTrigger GetTrigger()
    {
        return (AuraTrigger)trigger;
    }

    public int GetTime()
    {
        return time;
    }

    public AuraTarget GetEffectTarget()
    {
        return (AuraTarget)effectTarget;
    }

    public int[] GetEffectData()
    {
        return effectData;
    }

    public string[] GetRemoveEventNames()
    {
        return removeEventNames;
    }
}