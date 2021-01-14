using UnityEngine;
using UnityEditor;

public static class MyTools
{
    
    [MenuItem("My Tools/1. Add Defaults To Report %F1")]
    static void DEV_AppendDefaultsToReport()
    {
        CSVManager.AppendToReport(
            Random.Range(0, 200).ToString()
        );
        EditorApplication.Beep();
        Debug.Log("<color=green>Report updated!</color>");
    }

    [MenuItem("My Tools/2. Reset Report %F12")]
    static void DEV_ResetReport()
    {
        CSVManager.CreateReport();
        EditorApplication.Beep();
        Debug.Log("<color=orange>The report has been reset...</color>");
    }

    [MenuItem("My Tools/3. Append filtered data %F2")]
    static void DEV_AppendfilteredToReport()
    {
        CSVManager.CreateReport();
        var qwer = GameObject.FindObjectOfType<Filtering>();
        CSVManager.AppendToReport("LPF: " + qwer.LowPassFilterFactor.ToString());
        CSVManager.AppendToReport("Ignore: " + qwer.IgnoreReadingFactor.ToString());

        CSVManager.AppendFilteredData(qwer.filteredData);
        EditorApplication.Beep();
        Debug.Log("<color=green>Report updated!</color>");
    }

}
