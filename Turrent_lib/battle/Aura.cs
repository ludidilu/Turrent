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

            SuperEventListener.SuperFunctionCallBackV2<List<Action>, Unit, Unit> dele = delegate (int _index, ref List<Action> _funcList, Unit _triggerUnit, Unit _otherUnit)
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

        private static int RegisterAura(BattleCore _battle, Unit _unit, IAuraSDS _sds, AuraRegisterType _registerType)
        {
            int result;

            switch (_sds.GetEffectType())
            {
                case AuraType.ADD_INT:

                    SuperEventListener.SuperFunctionCallBackV2<int, Unit, Unit> dele1 = delegate (int _index, ref int _result, Unit _triggerUnit, Unit _otherUnit)
                    {
                        if (CheckAuraIsBeSilenced(_battle, _unit, _registerType) && CheckAuraTrigger(_battle, _unit, _triggerUnit, _sds))
                        {
                            _result += _sds.GetEffectData()[0];
                        }
                    };

                    result = _battle.eventListener.AddListener(_sds.GetEventName(), dele1, _sds.GetPriority());

                    break;

                case AuraType.SET_INT:

                    SuperEventListener.SuperFunctionCallBackV2<int, Unit, Unit> dele3 = delegate (int _index, ref int _result, Unit _triggerHero, Unit _otherUnit)
                    {
                        if (CheckAuraIsBeSilenced(_battle, _unit, _registerType) && CheckAuraTrigger(_battle, _unit, _triggerHero, _sds))
                        {
                            _result = _sds.GetEffectData()[0];
                        }
                    };

                    result = _battle.eventListener.AddListener(_sds.GetEventName(), dele3, _sds.GetPriority());

                    break;

                case AuraType.MULTI_INT:

                    SuperEventListener.SuperFunctionCallBackV2<int, Unit, Unit> dele4 = delegate (int _index, ref int _result, Unit _triggerHero, Unit _otherUnit)
                    {
                        if (CheckAuraIsBeSilenced(_battle, _unit, _registerType) && CheckAuraTrigger(_battle, _unit, _triggerHero, _sds))
                        {
                            _result = (int)(0.001f * _sds.GetEffectData()[0] * _result);
                        }
                    };

                    result = _battle.eventListener.AddListener(_sds.GetEventName(), dele4, _sds.GetPriority());

                    break;

                case AuraType.CAST_SKILL:

                    SuperEventListener.SuperFunctionCallBackV2<List<Action>, Unit, Unit> dele2 = delegate (int _index, ref List<Action> _funcList, Unit _triggerUnit, Unit _otherUnit)
                    {
                        if (CheckAuraIsBeSilenced(_battle, _unit, _registerType) && CheckAuraTrigger(_battle, _unit, _triggerUnit, _sds))
                        {

                        }
                    };

                    result = _battle.eventListener.AddListener(_sds.GetEventName(), dele2);

                    break;

                default:

                    throw new Exception("Unknown AuraType:" + _sds.GetEffectType().ToString());
            }

            return result;
        }

        private static bool CheckAuraIsBeSilenced(BattleCore _battle, Unit _unit, AuraRegisterType _registerType)
        {
            if (_registerType == AuraRegisterType.AURA)
            {
                int canTrigger = 1;

                _battle.eventListener.DispatchEvent<int, Unit>(BattleConst.TRIGGER_BORN_AURA, ref canTrigger, _unit);

                if (canTrigger < 1)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckAuraTrigger(BattleCore _battle, Unit _unit, Unit _triggerUnit, IAuraSDS _sds)
        {
            switch (_sds.GetTrigger())
            {
                case AuraTrigger.OWNER:

                    return _triggerUnit == _unit;

                case AuraTrigger.ENEMY:

                    return _triggerUnit.isMine != _unit.isMine;

                case AuraTrigger.OWNER_NEIGHBOUR_ALLY:

                    List<int> list = BattlePublicTools.GetNeighbourUnit(_battle, _unit);

                    return list.Contains(_triggerUnit.uid);

                case AuraTrigger.OWNER_ALLY:

                    return _triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine;

                case AuraTrigger.OWNER_ROW_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Dictionary<int, bool> dic = new Dictionary<int, bool>();

                        int y = _unit.pos / BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            int nowY = y + posFix / BattleConst.MAP_WIDTH;

                            if (!dic.ContainsKey(nowY))
                            {
                                dic.Add(nowY, false);
                            }
                        }

                        y = _triggerUnit.pos / BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _triggerUnit.sds.GetPos().Length; i++)
                        {
                            int posFix = _triggerUnit.sds.GetPos()[i];

                            int nowY = y + posFix / BattleConst.MAP_WIDTH;

                            if (dic.ContainsKey(nowY))
                            {
                                return true;
                            }
                        }
                    }

                    return false;

                case AuraTrigger.OWNER_COL_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Dictionary<int, bool> dic = new Dictionary<int, bool>();

                        int x = _unit.pos % BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            int nowX = x + posFix % BattleConst.MAP_WIDTH;

                            if (!dic.ContainsKey(nowX))
                            {
                                dic.Add(nowX, false);
                            }
                        }

                        x = _triggerUnit.pos % BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _triggerUnit.sds.GetPos().Length; i++)
                        {
                            int posFix = _triggerUnit.sds.GetPos()[i];

                            int nowX = x + posFix % BattleConst.MAP_WIDTH;

                            if (dic.ContainsKey(nowX))
                            {
                                return true;
                            }
                        }
                    }

                    return false;

                case AuraTrigger.OWNER_FRONT_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Dictionary<int, bool> dic = new Dictionary<int, bool>();

                        int y = _unit.pos / BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            int nowY = y + posFix / BattleConst.MAP_WIDTH;

                            if (nowY >= BattleConst.MAP_WIDTH)
                            {
                                int pos = _unit.pos + posFix - BattleConst.MAP_WIDTH;

                                if (!dic.ContainsKey(pos))
                                {
                                    dic.Add(pos, false);
                                }
                            }
                        }

                        for (int i = 0; i < _triggerUnit.sds.GetPos().Length; i++)
                        {
                            int posFix = _triggerUnit.sds.GetPos()[i];

                            int pos = _triggerUnit.pos + posFix;

                            if (dic.ContainsKey(pos))
                            {
                                return true;
                            }
                        }
                    }

                    return false;

                case AuraTrigger.OWNER_BACK_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Dictionary<int, bool> dic = new Dictionary<int, bool>();

                        int y = _unit.pos / BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            int nowY = y + posFix / BattleConst.MAP_WIDTH;

                            if (nowY < BattleConst.MAP_HEIGHT - 1)
                            {
                                int pos = _unit.pos + posFix + BattleConst.MAP_WIDTH;

                                if (!dic.ContainsKey(pos))
                                {
                                    dic.Add(pos, false);
                                }
                            }
                        }

                        for (int i = 0; i < _triggerUnit.sds.GetPos().Length; i++)
                        {
                            int posFix = _triggerUnit.sds.GetPos()[i];

                            int pos = _triggerUnit.pos + posFix;

                            if (dic.ContainsKey(pos))
                            {
                                return true;
                            }
                        }
                    }

                    return false;

                case AuraTrigger.OWNER_BESIDE_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Dictionary<int, bool> dic = new Dictionary<int, bool>();

                        int x = _unit.pos % BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            int nowX = x + posFix % BattleConst.MAP_WIDTH;

                            if (nowX > 0)
                            {
                                int pos = _unit.pos + posFix - 1;

                                if (!dic.ContainsKey(pos))
                                {
                                    dic.Add(pos, false);
                                }
                            }

                            if (nowX < BattleConst.MAP_WIDTH - 1)
                            {
                                int pos = _unit.pos + posFix + 1;

                                if (!dic.ContainsKey(pos))
                                {
                                    dic.Add(pos, false);
                                }
                            }
                        }

                        for (int i = 0; i < _triggerUnit.sds.GetPos().Length; i++)
                        {
                            int posFix = _triggerUnit.sds.GetPos()[i];

                            int pos = _triggerUnit.pos + posFix;

                            if (dic.ContainsKey(pos))
                            {
                                return true;
                            }
                        }
                    }

                    return false;

                default:

                    throw new Exception("CheckAuraTrigger error:" + _sds.GetTrigger());
            }
        }
    }
}
