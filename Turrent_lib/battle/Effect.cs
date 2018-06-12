namespace Turrent_lib
{
    internal static class Effect
    {
        internal static void UnitTakeEffect(BattleCore _battleCore, Unit _unit, int[] _data, int _time)
        {
            switch ((EffectType)_data[0])
            {
                case EffectType.PHYSIC_DAMAGE:

                    _unit.BePhysicDamaged(null, _data[1], _time);

                    break;

                case EffectType.MAGIC_DAMAGE:

                    _unit.BeMagicDamaged(null, _data[1], _time);

                    break;

                case EffectType.ADD_AURA:

                    Aura.Init(_battleCore, _unit, _data[1], Aura.AuraRegisterType.EFFECT, _time);

                    break;
            }
        }
    }
}
