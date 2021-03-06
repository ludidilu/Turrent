﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Connection;
using Turrent_lib;

namespace Turrent_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Connection.Log.Init(Console.WriteLine);

            Turrent_lib.Log.Init(Console.WriteLine);

            ResourceLoad();

            PlayerUnitManager.Instance = new PlayerUnitManager();

            BattleManager.Instance = new BattleManager();

            Server<PlayerUnit> server = new Server<PlayerUnit>();

            server.Start("0.0.0.0", ConfigDictionary.Instance.port, 100, 12000);

            Stopwatch watch = new Stopwatch();

            watch.Start();

            while (true)
            {
                long t0 = watch.ElapsedMilliseconds;

                server.Update();

                BattleManager.Instance.Update();

                long t1 = watch.ElapsedMilliseconds;

                int deltaTime = (int)(t1 - t0);

                int time = BattleConst.TICK_TIME - deltaTime;

                if (time > 0)
                {
                    Thread.Sleep(time);
                }
            }
        }

        private static void ResourceLoad()
        {
            ConfigDictionary.Instance.LoadLocalConfig("local.xml");

            StaticData.path = ConfigDictionary.Instance.table_path;

            StaticData.Load<TurrentSDS>("Turrent");

            StaticData.Load<UnitSDS>("Unit");

            StaticData.Load<AuraSDS>("Aura");

            StaticData.Load<BattleSDS>("Battle");

            Dictionary<int, TurrentSDS> turrendDic = StaticData.GetDic<TurrentSDS>();

            Dictionary<int, UnitSDS> unitDic = StaticData.GetDic<UnitSDS>();

            Dictionary<int, AuraSDS> auraDic = StaticData.GetDic<AuraSDS>();

            BattleCore.Init(unitDic, turrendDic, auraDic);

            string summonStr = File.ReadAllText(ConfigDictionary.Instance.ai_path + "ai_summon.xml");

            BattleAi.Init(summonStr);
        }
    }
}
