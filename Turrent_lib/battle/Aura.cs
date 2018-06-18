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

        internal static void Init(BattleCore _battleCore, Unit _unit, int _auraID, AuraRegisterType _registerType, int _nowTime)
        {
            IAuraSDS sds = BattleCore.GetAuraData(_auraID);

            List<int> ids = new List<int>();

            int id = RegisterAura(_battleCore, _unit, sds, _registerType);

            ids.Add(id);

            SuperEventListener.SuperFunctionCallBack3<Unit, Unit, int> dele = delegate (int _index, Unit _triggerUnit, Unit _otherUnit, int _time)
            {
                if (_triggerUnit == _unit)
                {
                    for (int i = 0; i < ids.Count; i++)
                    {
                        _battleCore.eventListener.RemoveListener(ids[i]);
                    }
                }
            };

            id = _battleCore.eventListener.AddListener(BattleConst.DIE, dele);

            ids.Add(id);

            if (_registerType == AuraRegisterType.EFFECT)
            {
                id = _battleCore.eventListener.AddListener(BattleConst.BE_CLEAN, dele);

                ids.Add(id);
            }

            if (sds.GetTime() > 0)
            {
                int overTime = _nowTime + sds.GetTime();

                SuperEventListener.SuperFunctionCallBack1<int> dele2 = delegate (int _index, int _time)
                {
                    if (_time >= overTime)
                    {
                        for (int i = 0; i < ids.Count; i++)
                        {
                            _battleCore.eventListener.RemoveListener(ids[i]);
                        }
                    }
                };

                id = _battleCore.eventListener.AddListener(BattleConst.TIME_OVER, dele2);

                ids.Add(id);
            }

            for (int i = 0; i < sds.GetRemoveEventNames().Length; i++)
            {
                id = _battleCore.eventListener.AddListener(sds.GetRemoveEventNames()[i], dele);

                ids.Add(id);
            }
        }

        private static int RegisterAura(BattleCore _battleCore, Unit _unit, IAuraSDS _sds, AuraRegisterType _registerType)
        {
            int result;

            switch (_sds.GetAuraType())
            {
                case AuraType.ADD_INT:

                    SuperEventListener.SuperFunctionCallBackV2<int, Unit, Unit> dele1 = delegate (int _index, ref int _result, Unit _triggerUnit, Unit _otherUnit)
                    {
                        if (CheckAuraIsBeSilenced(_battleCore, _unit, _registerType) && CheckAuraTrigger(_battleCore, _unit, _triggerUnit, _sds))
                        {
                            _result += _sds.GetEffectData()[0];
                        }
                    };

                    result = _battleCore.eventListener.AddListener(_sds.GetEventName(), dele1, _sds.GetPriority());

                    break;

                case AuraType.SET_INT:

                    SuperEventListener.SuperFunctionCallBackV2<int, Unit, Unit> dele3 = delegate (int _index, ref int _result, Unit _triggerHero, Unit _otherUnit)
                    {
                        if (CheckAuraIsBeSilenced(_battleCore, _unit, _registerType) && CheckAuraTrigger(_battleCore, _unit, _triggerHero, _sds))
                        {
                            _result = _sds.GetEffectData()[0];
                        }
                    };

                    result = _battleCore.eventListener.AddListener(_sds.GetEventName(), dele3, _sds.GetPriority());

                    break;

                case AuraType.MULTI_INT:

                    SuperEventListener.SuperFunctionCallBackV2<int, Unit, Unit> dele4 = delegate (int _index, ref int _result, Unit _triggerHero, Unit _otherUnit)
                    {
                        if (CheckAuraIsBeSilenced(_battleCore, _unit, _registerType) && CheckAuraTrigger(_battleCore, _unit, _triggerHero, _sds))
                        {
                            _result = (int)(0.001f * _sds.GetEffectData()[0] * _result);
                        }
                    };

                    result = _battleCore.eventListener.AddListener(_sds.GetEventName(), dele4, _sds.GetPriority());

                    break;

                case AuraType.CAST_SKILL:

                    SuperEventListener.SuperFunctionCallBack3<Unit, Unit, int> dele2 = delegate (int _index, Unit _triggerUnit, Unit _otherUnit, int _time)
                    {
                        if (CheckAuraIsBeSilenced(_battleCore, _unit, _registerType) && CheckAuraTrigger(_battleCore, _unit, _triggerUnit, _sds))
                        {
                            switch (_sds.GetEffectTarget())
                            {
                                case AuraTarget.OWNER:

                                    Effect.UnitTakeEffect(_battleCore, _unit, _sds.GetEffectData(), _time);

                                    break;

                                case AuraTarget.TRIGGER:

                                    Effect.UnitTakeEffect(_battleCore, _triggerUnit, _sds.GetEffectData(), _time);

                                    break;

                                case AuraTarget.OTHER:

                                    if (_otherUnit != null)
                                    {
                                        Effect.UnitTakeEffect(_battleCore, _otherUnit, _sds.GetEffectData(), _time);
                                    }

                                    break;
                            }
                        }
                    };

                    result = _battleCore.eventListener.AddListener(_sds.GetEventName(), dele2);

                    break;

                default:

                    throw new Exception("Unknown AuraType:" + _sds.GetAuraType().ToString());
            }

            return result;
        }

        private static bool CheckAuraIsBeSilenced(BattleCore _battleCore, Unit _unit, AuraRegisterType _registerType)
        {
            if (_registerType == AuraRegisterType.AURA)
            {
                int canTrigger = 1;

                _battleCore.eventListener.DispatchEvent<int, Unit>(BattleConst.TRIGGER_BORN_AURA, ref canTrigger, _unit);

                if (canTrigger < 1)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckAuraTrigger(BattleCore _battleCore, Unit _unit, Unit _triggerUnit, IAuraSDS _sds)
        {
            switch (_sds.GetTrigger())
            {
                case AuraTrigger.OWNER:

                    return _triggerUnit == _unit;

                case AuraTrigger.ENEMY:

                    return _triggerUnit.isMine != _unit.isMine;

                case AuraTrigger.COL_ENEMY:

                    if (_triggerUnit.isMine != _unit.isMine)
                    {
                        Turrent[] turrents = _triggerUnit.isMine ? _battleCore.mTurrent : _battleCore.oTurrent;

                        int x = _unit.pos % BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            if (posFix >= BattleConst.MAP_WIDTH)
                            {
                                return false;
                            }

                            int nowX = x + posFix % BattleConst.MAP_WIDTH;

                            nowX = BattleConst.MAP_WIDTH - 1 - nowX;

                            for (int m = 0; m < BattleConst.MAP_HEIGHT; m++)
                            {
                                int targetPos = m * BattleConst.MAP_WIDTH + nowX;

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null && turrent.parent == _triggerUnit)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;

                case AuraTrigger.FRONT_ENEMY:

                    if (_triggerUnit.isMine != _unit.isMine)
                    {
                        Turrent[] turrents = _triggerUnit.isMine ? _battleCore.mTurrent : _battleCore.oTurrent;

                        int x = _unit.pos % BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            if (posFix >= BattleConst.MAP_WIDTH)
                            {
                                return false;
                            }

                            int nowX = x + posFix % BattleConst.MAP_WIDTH;

                            nowX = BattleConst.MAP_WIDTH - 1 - nowX;

                            for (int m = 0; m < BattleConst.MAP_HEIGHT; m++)
                            {
                                int targetPos = m * BattleConst.MAP_WIDTH + nowX;

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null)
                                {
                                    if (turrent.parent == _triggerUnit)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    return false;

                case AuraTrigger.OWNER_NEIGHBOUR_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Turrent[] turrents = _unit.isMine ? _battleCore.mTurrent : _battleCore.oTurrent;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            int nowPos = _unit.pos + posFix;

                            int nowX = nowPos % BattleConst.MAP_WIDTH;

                            int targetPos;

                            if (nowX > 0)
                            {
                                targetPos = nowPos - 1;

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null && turrent.parent == _triggerUnit)
                                {
                                    return true;
                                }
                            }

                            if (nowX < BattleConst.MAP_WIDTH - 1)
                            {
                                targetPos = nowPos + 1;

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null && turrent.parent == _triggerUnit)
                                {
                                    return true;
                                }
                            }

                            targetPos = nowPos - BattleConst.MAP_WIDTH;

                            if (targetPos >= 0)
                            {
                                Turrent turrent = turrents[targetPos];

                                if (turrent != null && turrent.parent == _triggerUnit)
                                {
                                    return true;
                                }
                            }

                            targetPos = nowPos + BattleConst.MAP_WIDTH;

                            if (targetPos < BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT)
                            {
                                Turrent turrent = turrents[targetPos];

                                if (turrent != null && turrent.parent == _triggerUnit)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;

                case AuraTrigger.OWNER_ALLY:

                    return _triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine;

                case AuraTrigger.OWNER_ROW_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Turrent[] turrents = _unit.isMine ? _battleCore.mTurrent : _battleCore.oTurrent;

                        int y = _unit.pos / BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            if (posFix % BattleConst.MAP_WIDTH != 0)
                            {
                                continue;
                            }

                            int nowY = y + posFix / BattleConst.MAP_WIDTH;

                            for (int m = 0; m < BattleConst.MAP_WIDTH; m++)
                            {
                                int targetPos = nowY * BattleConst.MAP_WIDTH + m;

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null && turrent.parent == _triggerUnit)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;

                case AuraTrigger.OWNER_COL_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Turrent[] turrents = _unit.isMine ? _battleCore.mTurrent : _battleCore.oTurrent;

                        int x = _unit.pos % BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            if (posFix >= BattleConst.MAP_WIDTH)
                            {
                                return false;
                            }

                            int nowX = x + posFix % BattleConst.MAP_WIDTH;

                            for (int m = 0; m < BattleConst.MAP_HEIGHT; m++)
                            {
                                int targetPos = m * BattleConst.MAP_WIDTH + nowX;

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null && turrent.parent == _triggerUnit)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;

                case AuraTrigger.OWNER_FRONT_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Turrent[] turrents = _unit.isMine ? _battleCore.mTurrent : _battleCore.oTurrent;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            if (posFix >= BattleConst.MAP_WIDTH)
                            {
                                return false;
                            }

                            int nowPos = _unit.pos + posFix;

                            for (int m = 1; m < BattleConst.MAP_HEIGHT; m++)
                            {
                                int targetPos = nowPos - m * BattleConst.MAP_WIDTH;

                                if (targetPos < 0)
                                {
                                    break;
                                }

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null)
                                {
                                    if (turrent.parent == _triggerUnit)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    return false;

                case AuraTrigger.OWNER_BACK_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Turrent[] turrents = _unit.isMine ? _battleCore.mTurrent : _battleCore.oTurrent;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            if (posFix >= BattleConst.MAP_WIDTH)
                            {
                                return false;
                            }

                            int nowPos = _unit.pos + posFix;

                            for (int m = 1; m < BattleConst.MAP_HEIGHT; m++)
                            {
                                int targetPos = nowPos + m * BattleConst.MAP_WIDTH;

                                if (targetPos >= BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT)
                                {
                                    break;
                                }

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null)
                                {
                                    if (turrent.parent == _triggerUnit)
                                    {
                                        return true;
                                    }
                                    else if (turrent.parent != _unit)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    return false;

                case AuraTrigger.OWNER_BESIDE_ALLY:

                    if (_triggerUnit != _unit && _triggerUnit.isMine == _unit.isMine)
                    {
                        Turrent[] turrents = _unit.isMine ? _battleCore.mTurrent : _battleCore.oTurrent;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            int nowPos = _unit.pos + posFix;

                            int nowX = nowPos % BattleConst.MAP_WIDTH;

                            if (nowX > 0)
                            {
                                int targetPos = nowPos - 1;

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null && turrent.parent == _triggerUnit)
                                {
                                    return true;
                                }
                            }

                            if (nowX < BattleConst.MAP_WIDTH - 1)
                            {
                                int targetPos = nowPos + 1;

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null && turrent.parent == _triggerUnit)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;

                case AuraTrigger.ROW_ENEMY:

                    if (_triggerUnit.isMine != _unit.isMine)
                    {
                        Turrent[] turrents = _triggerUnit.isMine ? _battleCore.mTurrent : _battleCore.oTurrent;

                        int y = _unit.pos / BattleConst.MAP_WIDTH;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            if (posFix % BattleConst.MAP_WIDTH != 0)
                            {
                                continue;
                            }

                            int nowY = y + posFix / BattleConst.MAP_WIDTH;

                            for (int m = 0; m < BattleConst.MAP_WIDTH; m++)
                            {
                                int targetPos = nowY * BattleConst.MAP_WIDTH + m;

                                Turrent turrent = turrents[targetPos];

                                if (turrent != null && turrent.parent == _triggerUnit)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;

                case AuraTrigger.POS_ENEMY:

                    if (_triggerUnit.isMine != _unit.isMine)
                    {
                        Turrent[] turrents = _triggerUnit.isMine ? _battleCore.mTurrent : _battleCore.oTurrent;

                        for (int i = 0; i < _unit.sds.GetPos().Length; i++)
                        {
                            int posFix = _unit.sds.GetPos()[i];

                            int nowPos = _unit.pos + posFix;

                            int nowX = nowPos % BattleConst.MAP_WIDTH;

                            int nowY = nowPos / BattleConst.MAP_WIDTH;

                            nowPos = BattleConst.MAP_WIDTH - 1 - nowPos;

                            int targetPos = nowY * BattleConst.MAP_WIDTH + nowX;

                            Turrent turrent = turrents[targetPos];

                            if (turrent != null && turrent.parent == _triggerUnit)
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
