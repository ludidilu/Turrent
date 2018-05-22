using System;
using UnityEngine;
using System.IO;
#if USE_ASSETBUNDLE
using wwwManager;
using thread;
using System.Threading;
using assetManager;
using assetBundleManager;
#endif
using gameObjectFactory;

public static class ResourceLoader
{
    private static readonly string[] preloadPrefabs = new string[] {
        "Assets/Resource/prefab/battle/BattleManager.prefab",
        "Assets/Resource/prefab/game/BattleEntrance.prefab",
        "Assets/Resource/prefab/game/BattleOnline.prefab",
        "Assets/Resource/prefab/game/BattleView.prefab",
    };

    private static Action callBack;

    private static int num;

    public static void Load(Action _callBack)
    {
        callBack = _callBack;

        LoadConfig(ConfigLoadOver);
    }

    private static void ConfigLoadOver()
    {
        num = 3;

        LoadTables();

        LoadPrefabs();

        OneLoadOver();
    }

    public static void LoadConfig(Action _callBack)
    {
#if !USE_ASSETBUNDLE

        LoadConfigLocal();

        if (_callBack != null)
        {
            _callBack();
        }
#else
        Action<WWW> dele = delegate (WWW _www)
        {
            ConfigDictionary.Instance.SetData(_www.text);

            if (_callBack != null)
            {
                _callBack();
            }
        };

        WWWManager.Instance.Load("local.xml", dele);
#endif
    }

    public static void LoadConfigLocal()
    {
        ConfigDictionary.Instance.LoadLocalConfig(Path.Combine(Application.streamingAssetsPath, "local.xml"));
    }

    private static void LoadTables()
    {
#if !USE_ASSETBUNDLE

        LoadTablesLocal();

        OneLoadOver();
#else

        WWWManager.Instance.Load(StaticData.datName, GetCsvBytes);
#endif
    }

    private static void GetCsvBytes(WWW _www)
    {
        StaticData.LoadCsvDataFromFile(_www.bytes, OneLoadOver, LoadCsv.Init);
    }

    public static void LoadTablesLocal()
    {
        StaticData.path = ConfigDictionary.Instance.table_path;

        StaticData.Dispose();

        StaticData.Load<UnitSDS>("Unit");

        StaticData.Load<TurrentSDS>("Turrent");
    }

    private static void LoadPrefabs()
    {
#if !USE_ASSETBUNDLE

        GameObjectFactory.Instance.PreloadGameObjects(preloadPrefabs, OneLoadOver);
#else
        Action dele = delegate ()
        {
            GameObjectFactory.Instance.PreloadGameObjects(preloadPrefabs, OneLoadOver);
        };

        AssetManager.Instance.Init(dele);
#endif
    }

    private static void OneLoadOver()
    {
        num--;

        if (num == 0)
        {
            if (callBack != null)
            {
                Action tmpCb = callBack;

                callBack = null;

                tmpCb();
            }
        }
    }
}
