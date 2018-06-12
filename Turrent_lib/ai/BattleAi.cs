using System;
using System.Reflection;
using bt;

namespace Turrent_lib
{
    public static class BattleAi
    {
        private static BtRoot<BattleCore, bool, AiSummonData> btRoot;

        private static AiSummonData aiSummonData;

        public static void Init(string _str)
        {
            btRoot = BtTools.Create<BattleCore, bool, AiSummonData>(_str, Assembly.GetExecutingAssembly().FullName);

            aiSummonData = new AiSummonData();
        }

        public static void Start(BattleCore _battleCore, bool _isMine, Func<int, int> _getRandomValueCallBack, out int _uid, out int _pos)
        {
            aiSummonData.uid = -1;

            aiSummonData.pos = -1;

            btRoot.Enter(_getRandomValueCallBack, _battleCore, _isMine, aiSummonData);

            if (aiSummonData.uid == -1 || aiSummonData.pos == -1)
            {
                _uid = -1;

                _pos = -1;
            }
            else
            {
                _uid = aiSummonData.uid;

                _pos = aiSummonData.pos;
            }

            aiSummonData.summonPosList.Clear();
        }
    }
}
