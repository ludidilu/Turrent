using System.IO;
public class TurrentSDS_c {
    public static void Init(TurrentSDS _csv, BinaryReader _br){
        _csv.canAttackBase = _br.ReadBoolean();
        _csv.attackDamage = _br.ReadInt32();
        _csv.attackDamageAdd = _br.ReadInt32();
        _csv.attackDamageAddGap = _br.ReadInt32();
        _csv.attackDamageAddMax = _br.ReadInt32();
        _csv.attackDamageType = _br.ReadInt32();
        _csv.attackGap = _br.ReadInt32();
        _csv.cd = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
        _csv.icon = _br.ReadString();
        int lengthattackSplashPos = _br.ReadInt32();
        _csv.attackSplashPos = new string[lengthattackSplashPos];
        for(int i = 0 ; i < lengthattackSplashPos ; i++){
            _csv.attackSplashPos[i] = _br.ReadString();
        }
        int lengthattackTargetPos = _br.ReadInt32();
        _csv.attackTargetPos = new string[lengthattackTargetPos];
        for(int i = 0 ; i < lengthattackTargetPos ; i++){
            _csv.attackTargetPos[i] = _br.ReadString();
        }
    }
}
