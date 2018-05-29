using System.IO;
public class UnitSDS_c {
    public static void Init(UnitSDS _csv, BinaryReader _br){
        _csv.cost = _br.ReadInt32();
        _csv.hp = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
        int lengthpos = _br.ReadInt32();
        _csv.pos = new int[lengthpos];
        for(int i = 0 ; i < lengthpos ; i++){
            _csv.pos[i] = _br.ReadInt32();
        }
        int lengthrow = _br.ReadInt32();
        _csv.row = new int[lengthrow];
        for(int i = 0 ; i < lengthrow ; i++){
            _csv.row[i] = _br.ReadInt32();
        }
        int lengthturrent = _br.ReadInt32();
        _csv.turrent = new int[lengthturrent];
        for(int i = 0 ; i < lengthturrent ; i++){
            _csv.turrent[i] = _br.ReadInt32();
        }
        _csv.name = _br.ReadString();
    }
}
