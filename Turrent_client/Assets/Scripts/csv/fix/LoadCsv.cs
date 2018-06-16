using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
public class LoadCsv {
    public static Dictionary<Type,IDictionary> Init(byte[] _bytes) {
        MemoryStream ms = new MemoryStream(_bytes);
        BinaryReader br = new BinaryReader(ms);
        Dictionary<Type,IDictionary> dic = new Dictionary<Type,IDictionary>();
        Dictionary<int,AuraSDS> AuraSDSDic = new Dictionary<int,AuraSDS>();
        int lengthAuraSDS = br.ReadInt32();
        for(int i = 0 ; i < lengthAuraSDS ; i++){
            AuraSDS unit = new AuraSDS();
            AuraSDS_c.Init(unit,br);
            unit.Fix();
            AuraSDSDic.Add(unit.ID,unit);
        }
        dic.Add(typeof(AuraSDS),AuraSDSDic);
        Dictionary<int,TurrentSDS> TurrentSDSDic = new Dictionary<int,TurrentSDS>();
        int lengthTurrentSDS = br.ReadInt32();
        for(int i = 0 ; i < lengthTurrentSDS ; i++){
            TurrentSDS unit = new TurrentSDS();
            TurrentSDS_c.Init(unit,br);
            unit.Fix();
            TurrentSDSDic.Add(unit.ID,unit);
        }
        dic.Add(typeof(TurrentSDS),TurrentSDSDic);
        Dictionary<int,UnitSDS> UnitSDSDic = new Dictionary<int,UnitSDS>();
        int lengthUnitSDS = br.ReadInt32();
        for(int i = 0 ; i < lengthUnitSDS ; i++){
            UnitSDS unit = new UnitSDS();
            UnitSDS_c.Init(unit,br);
            unit.Fix();
            UnitSDSDic.Add(unit.ID,unit);
        }
        dic.Add(typeof(UnitSDS),UnitSDSDic);
        br.Close();
        ms.Close();
        ms.Dispose();
        return dic;
    }
}
