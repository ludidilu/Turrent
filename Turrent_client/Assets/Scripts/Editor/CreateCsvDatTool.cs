using UnityEditor;

public static class CreateCsvDatTool
{
    [MenuItem("Create csv.dat/Do")]
    public static void Start()
    {
        ResourceLoader.LoadConfigLocal();

        ResourceLoader.LoadTablesLocal();

        CreateCsvDat.Start("/Scripts/csv/fix/");
    }
}
