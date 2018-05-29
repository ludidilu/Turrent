using System.Collections;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class StaticData
{
    public static string path;

    public const string datName = "csv.dat";

    public static Dictionary<Type, IDictionary> dic = new Dictionary<Type, IDictionary>();

    private static Dictionary<Type, IList> dicList = new Dictionary<Type, IList>();

    public static T GetData<T>(int _id) where T : CsvBase
    {
        Dictionary<int, T> tmpDic = dic[typeof(T)] as Dictionary<int, T>;

        T data;

        if (!tmpDic.TryGetValue(_id, out data))
        {
            Console.WriteLine(typeof(T).Name + "表中未找到ID为:" + _id + "的行!");
        }

        return data;
    }

    public static Dictionary<int, T> GetDic<T>() where T : CsvBase
    {
        Type type = typeof(T);

        IDictionary data;

        if (!dic.TryGetValue(type, out data))
        {
            Console.WriteLine("not find: " + type);
        }

        return data as Dictionary<int, T>;
    }

    public static List<T> GetList<T>() where T : CsvBase
    {
        Type type = typeof(T);

        IList data;

        if (!dicList.TryGetValue(type, out data))
        {
            Dictionary<int, T> dict = GetDic<T>();

            data = new List<T>();

            IEnumerator<T> enumerator = dict.Values.GetEnumerator();

            while (enumerator.MoveNext())
            {
                data.Add(enumerator.Current);
            }

            dicList.Add(type, data);
        }

        return data as List<T>;
    }

    public static bool IsIDExists<T>(int _id) where T : CsvBase
    {
        Dictionary<int, T> dict = GetDic<T>();

        return dict.ContainsKey(_id);
    }

    public static void Dispose()
    {
        dic = new Dictionary<Type, IDictionary>();
    }

    public static void Load<T>(string _name) where T : CsvBase, new()
    {
        Type type = typeof(T);

        if (dic.ContainsKey(type))
        {
            return;
        }

        Dictionary<int, T> result = new Dictionary<int, T>();

        using (FileStream fs = new FileStream(path + "/" + _name + ".csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (StreamReader reader = new StreamReader(fs))
            {
                int i = 0;

                string lineStr = reader.ReadLine();

                FieldInfo[] infoArr = null;

                while (!string.IsNullOrEmpty(lineStr))
                {
                    if (!lineStr.StartsWith("//"))
                    {
                        if (i == 2)
                        {
                            string[] dataArr = SplitCsvLine(lineStr);

                            infoArr = new FieldInfo[dataArr.Length];

                            for (int m = 1; m < dataArr.Length; m++)
                            {
                                infoArr[m] = type.GetField(dataArr[m]);
                            }
                        }
                        else if (i > 2)
                        {
                            string[] dataArr = SplitCsvLine(lineStr);

                            T csv = new T();

                            csv.ID = int.Parse(dataArr[0]);

                            for (int m = 1; m < infoArr.Length; m++)
                            {
                                FieldInfo info = infoArr[m];

                                if (info != null)
                                {
                                    SetData(info, csv, dataArr[m]);
                                }
                            }

                            csv.Fix();

                            result.Add(csv.ID, csv);
                        }

                        i++;
                    }

                    lineStr = reader.ReadLine();
                }
            }
        }

        dic.Add(type, result);
    }

    private static void SetData(FieldInfo _info, CsvBase _csv, string _data)
    {
        try
        {
            switch (_info.FieldType.Name)
            {
                case "Int32":

                    if (string.IsNullOrEmpty(_data))
                    {
                        _info.SetValue(_csv, 0);
                    }
                    else
                    {
                        _info.SetValue(_csv, int.Parse(_data));
                    }

                    break;

                case "Int64":

                    if (string.IsNullOrEmpty(_data))
                    {
                        _info.SetValue(_csv, 0);
                    }
                    else
                    {
                        _info.SetValue(_csv, long.Parse(_data));
                    }

                    break;

                case "String":

                    _info.SetValue(_csv, FixStringChangeLine(_data));

                    break;

                case "Boolean":

                    _info.SetValue(_csv, _data == "1" ? true : false);

                    break;

                case "Single":

                    if (string.IsNullOrEmpty(_data))
                    {
                        _info.SetValue(_csv, 0);
                    }
                    else
                    {
                        _info.SetValue(_csv, float.Parse(_data));
                    }

                    break;

                case "Double":

                    if (string.IsNullOrEmpty(_data))
                    {
                        _info.SetValue(_csv, 0);
                    }
                    else
                    {
                        _info.SetValue(_csv, double.Parse(_data));
                    }

                    break;

                case "Int16":

                    if (string.IsNullOrEmpty(_data))
                    {
                        _info.SetValue(_csv, 0);
                    }
                    else
                    {
                        _info.SetValue(_csv, short.Parse(_data));
                    }

                    break;

                case "Int32[]":

                    int[] intResult;

                    if (string.IsNullOrEmpty(_data))
                    {
                        intResult = new int[0];
                    }
                    else if (_data == "^")
                    {
                        intResult = new int[1];
                    }
                    else
                    {
                        string[] strArr = _data.Split('$');

                        intResult = new int[strArr.Length];

                        for (int i = 0; i < strArr.Length; i++)
                        {
                            intResult[i] = int.Parse(strArr[i]);
                        }
                    }

                    _info.SetValue(_csv, intResult);

                    break;

                case "Int64[]":

                    long[] longResult;

                    if (string.IsNullOrEmpty(_data))
                    {
                        longResult = new long[0];
                    }
                    else if (_data == "^")
                    {
                        longResult = new long[1];
                    }
                    else
                    {
                        string[] strArr = _data.Split('$');

                        longResult = new long[strArr.Length];

                        for (int i = 0; i < strArr.Length; i++)
                        {
                            longResult[i] = long.Parse(strArr[i]);
                        }
                    }

                    _info.SetValue(_csv, longResult);

                    break;

                case "String[]":

                    string[] stringResult;

                    if (string.IsNullOrEmpty(_data))
                    {
                        stringResult = new string[0];
                    }
                    else if (_data == "^")
                    {
                        stringResult = new string[1];
                    }
                    else
                    {
                        string[] tmpStr = _data.Split('$');

                        stringResult = new string[tmpStr.Length];

                        for (int i = 0; i < tmpStr.Length; i++)
                        {
                            stringResult[i] = FixStringChangeLine(tmpStr[i]);
                        }
                    }

                    _info.SetValue(_csv, stringResult);

                    break;

                case "Boolean[]":

                    bool[] boolResult;

                    if (string.IsNullOrEmpty(_data))
                    {
                        boolResult = new bool[0];
                    }
                    else if (_data == "^")
                    {
                        boolResult = new bool[1];
                    }
                    else
                    {
                        string[] strArr = _data.Split('$');

                        boolResult = new bool[strArr.Length];

                        for (int i = 0; i < strArr.Length; i++)
                        {
                            boolResult[i] = strArr[i] == "1" ? true : false;
                        }
                    }

                    _info.SetValue(_csv, boolResult);

                    break;

                case "Single[]":

                    float[] floatResult;

                    if (string.IsNullOrEmpty(_data))
                    {
                        floatResult = new float[0];
                    }
                    else if (_data == "^")
                    {
                        floatResult = new float[1];
                    }
                    else
                    {
                        string[] strArr = _data.Split('$');

                        floatResult = new float[strArr.Length];

                        for (int i = 0; i < strArr.Length; i++)
                        {
                            floatResult[i] = float.Parse(strArr[i]);
                        }
                    }

                    _info.SetValue(_csv, floatResult);

                    break;

                case "Double[]":

                    double[] doubleResult;

                    if (string.IsNullOrEmpty(_data))
                    {
                        doubleResult = new double[0];
                    }
                    else if (_data == "^")
                    {
                        doubleResult = new double[1];
                    }
                    else
                    {
                        string[] strArr = _data.Split('$');

                        doubleResult = new double[strArr.Length];

                        for (int i = 0; i < strArr.Length; i++)
                        {
                            doubleResult[i] = double.Parse(strArr[i]);
                        }
                    }

                    _info.SetValue(_csv, doubleResult);

                    break;

                case "Int16[]":

                    short[] shortResult;

                    if (string.IsNullOrEmpty(_data))
                    {
                        shortResult = new short[0];
                    }
                    else if (_data == "^")
                    {
                        shortResult = new short[1];
                    }
                    else
                    {
                        string[] strArr = _data.Split('$');

                        shortResult = new short[strArr.Length];

                        for (int i = 0; i < strArr.Length; i++)
                        {
                            shortResult[i] = short.Parse(strArr[i]);
                        }
                    }

                    _info.SetValue(_csv, shortResult);

                    break;

                default:

                    throw new Exception("csv表中的类型不支持反射  setData:" + _info.Name + "   " + _info.FieldType.Name + "   " + _data);
            }
        }
        catch (Exception e)
        {
            string str = "setData:" + _info.Name + "   " + _info.FieldType.Name + "   " + _data + "   " + _data.Length + Environment.NewLine;

            Console.WriteLine(str + "   " + e.ToString());
        }
    }

    private static string FixStringChangeLine(string _str)
    {
        return _str.Replace("\\n", "\n");
    }

    private static readonly Regex reg = new Regex("\".*?\"");

    private static string[] SplitCsvLine(string _str)
    {
        List<KeyValuePair<string, string>> list = null;

        int index = 0;

        MatchEvaluator me = delegate (Match _match)
        {
            if (list == null)
            {
                list = new List<KeyValuePair<string, string>>();
            }

            string result = string.Format("$replace:{0}$", index);

            string str = _match.Value.Substring(1, _match.Length - 2);

            list.Add(new KeyValuePair<string, string>(result, str));

            index++;

            return result;
        };

        string strFix = reg.Replace(_str, me);

        string[] final = strFix.Split(',');

        if (list != null)
        {
            index = 0;

            KeyValuePair<string, string> replaceData = list[index];

            for (int i = 0; i < final.Length; i++)
            {
                string ss = final[i];

                if (ss == replaceData.Key)
                {
                    final[i] = replaceData.Value;

                    index++;

                    if (index < list.Count)
                    {
                        replaceData = list[index];
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return final;
    }
}