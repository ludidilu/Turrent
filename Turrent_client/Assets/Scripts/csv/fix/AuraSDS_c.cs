using System.IO;
public class AuraSDS_c {
    public static void Init(AuraSDS _csv, BinaryReader _br){
        _csv.effectTarget = _br.ReadInt32();
        _csv.effectType = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
        _csv.priority = _br.ReadInt32();
        _csv.time = _br.ReadInt32();
        _csv.trigger = _br.ReadInt32();
        int lengtheffectData = _br.ReadInt32();
        _csv.effectData = new int[lengtheffectData];
        for(int i = 0 ; i < lengtheffectData ; i++){
            _csv.effectData[i] = _br.ReadInt32();
        }
        _csv.eventName = _br.ReadString();
        int lengthremoveEventNames = _br.ReadInt32();
        _csv.removeEventNames = new string[lengthremoveEventNames];
        for(int i = 0 ; i < lengthremoveEventNames ; i++){
            _csv.removeEventNames[i] = _br.ReadString();
        }
    }
}
