using System;
using System.Collections.Generic;
using superEvent;

namespace Turrent_lib
{
    internal static class Aura
    {
        internal enum AuraRegisterType
        {
            AURA,
            EFFECT,
            FEATURE,
        }

        internal static void Init(BattleCore _battle, Unit _unit, int _auraID, AuraRegisterType _registerType)
        {
            IAuraSDS sds = BattleCore.GetAuraData(_auraID);

            List<int> ids = new List<int>();

            int id = RegisterAura(_battle, _unit, sds, _registerType);

            ids.Add(id);

            SuperEventListener.SuperFunctionCallBackV1<List<Action>, Unit> dele = delegate (int _index, ref List<Action> _funcList, Unit _triggerUnit)
            {
                if (_triggerUnit == _unit)
                {
                    for (int i = 0; i < ids.Count; i++)
                    {
                        _battle.eventListener.RemoveListener(ids[i]);
                    }
                }
            };

            id = _battle.eventListener.AddListener(BattleConst.DIE, dele);

            ids.Add(id);

            if (_registerType == AuraRegisterType.EFFECT)
            {
                id = _battle.eventListener.AddListener(BattleConst.BE_CLEAN, dele);

                ids.Add(id);
            }
            else
            {
                id = _battle.eventListener.AddListener(BattleConst.REMOVE_BORN_AURA, dele);

                ids.Add(id);
            }

            for (int i = 0; i < sds.GetRemoveEventNames().Length; i++)
            {
                id = _battle.eventListener.AddListener(sds.GetRemoveEventNames()[i], dele);

                ids.Add(id);
            }
        }

        private static int RegisterAura(BattleCore _battle, Unit _hero, IAuraSDS _sds, AuraRegisterType _registerType)
        {
            int result;

            switch (_sds.GetEffectType())
            {
                case AuraType.FIX_INT:

                    SuperEventListener.SuperFunctionCallBackV2<int, Unit, Unit> dele1 = delegate (int _index, ref int _result, Unit _triggerHero, Unit _triggerTargetHero)
                    {
                        if (CheckAuraIsBeSilenced(_battle, _hero, _registerType) && CheckAuraTrigger(_battle, _hero, _triggerHero, _sds) && CheckCondition(_battle, _hero, _triggerHero, _triggerTargetHero, _sds.GetConditionCompare(), _sds.GetConditionType(), _sds.GetConditionData()))
                        {
                            Hero.HeroData heroData = (Hero.HeroData)(_sds.GetEffectData()[0]);

                            if (heroData == Hero.HeroData.DATA)
                            {
                                _result += _sds.GetEffectData()[1];

                            }
                            else
                            {
                                _result += _hero.GetData(heroData) * _sds.GetEffectData()[1];
                            }
                        }
                    };

                    result = _battle.eventListener.AddListener(_sds.GetEventName(), dele1, _sds.GetPriority());

                    break;

                case AuraType.SET_INT:

                    SuperEventListener.SuperFunctionCallBackV2<int, Hero, Hero> dele3 = delegate (int _index, ref int _result, Hero _triggerHero, Hero _triggerTargetHero)
                    {
                        if (CheckAuraIsBeSilenced(_battle, _hero, _registerType) && CheckAuraTrigger(_battle, _hero, _triggerHero, _sds) && CheckCondition(_battle, _hero, _triggerHero, _triggerTargetHero, _sds.GetConditionCompare(), _sds.GetConditionType(), _sds.GetConditionData()))
                        {
                            Hero.HeroData heroData = (Hero.HeroData)(_sds.GetEffectData()[0]);

                            if (heroData == Hero.HeroData.DATA)
                            {
                                _result = _sds.GetEffectData()[1];

                            }
                            else
                            {
                                _result = _hero.GetData(heroData) * _sds.GetEffectData()[1];
                            }
                        }
                    };

                    result = _battle.eventListener.AddListener(_sds.GetEventName(), dele3, _sds.GetPriority());

                    break;

                case AuraType.CAST_SKILL:

                    SuperEventListener.SuperFunctionCallBackV2<LinkedList<KeyValuePair<int, Func<BattleTriggerAuraVO>>>, Hero, Hero> dele2 = delegate (int _index, ref LinkedList<KeyValuePair<int, Func<BattleTriggerAuraVO>>> _funcList, Hero _triggerHero, Hero _triggerTargetHero)
                    {
                        if (CheckAuraIsBeSilenced(_battle, _hero, _registerType) && CheckAuraTrigger(_battle, _hero, _triggerHero, _sds) && CheckCondition(_battle, _hero, _triggerHero, _triggerTargetHero, _sds.GetConditionCompare(), _sds.GetConditionType(), _sds.GetConditionData()))
                        {
                            IEffectSDS effectSDS = Battle.GetEffectData(_sds.GetEffectData()[0]);

                            Func<BattleTriggerAuraVO> func = delegate ()
                            {
                                return AuraCastSkill(_battle, _hero, _triggerHero, _triggerTargetHero, _sds, effectSDS);
                            };

                            if (_funcList == null)
                            {
                                _funcList = new LinkedList<KeyValuePair<int, Func<BattleTriggerAuraVO>>>();
                            }

                            int priority = effectSDS.GetPriority();

                            LinkedListNode<KeyValuePair<int, Func<BattleTriggerAuraVO>>> addNode = new LinkedListNode<KeyValuePair<int, Func<BattleTriggerAuraVO>>>(new KeyValuePair<int, Func<BattleTriggerAuraVO>>(priority, func));

                            LinkedListNode<KeyValuePair<int, Func<BattleTriggerAuraVO>>> node = _funcList.First;

                            if (node == null)
                            {
                                _funcList.AddFirst(addNode);
                            }
                            else
                            {
                                while (true)
                                {
                                    if (priority > node.Value.Key)
                                    {
                                        node = node.Next;

                                        if (node == null)
                                        {
                                            _funcList.AddLast(addNode);

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        _funcList.AddBefore(node, addNode);

                                        break;
                                    }
                                }
                            }
                        }
                    };

                    result = _battle.eventListener.AddListener(_sds.GetEventName(), dele2);

                    break;

                default:

                    throw new Exception("Unknown AuraType:" + _sds.GetEffectType().ToString());
            }

            return result;
        }
    }
}
