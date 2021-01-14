using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class CSVManager
{

    private static string reportDirectoryName = "Report";
    private static string reportFileName = "report.csv";
    private static string reportSeparator = ",";
    private static string reportHeader = "Instantaneous BPM";
    private static string timeStampHeader = "time stamp";

    #region Interactions

    /// <summary>
    /// Appends a single string to the defined file.
    /// </summary>
    /// <param name="strings"></param>
    public static void AppendToReport(string strings)
    {
        VerifyDirectory();
        VerifyFile();
        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string finalString = "";
            finalString = strings;
            //for (int i = 0; i < strings.Length; i++)
            //{
            //    if (finalString != "")
            //    {
            //        finalString += reportSeparator;
            //    }
            //    finalString += strings;
            //}
            //finalString += reportSeparator + GetTimeStamp();
            sw.WriteLine(finalString);
        }
    }

    public static void AppendFilteredData(double[] data)
    {
        VerifyDirectory();
        VerifyFile();
        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string finalString = "";
            finalString = "Filtered Data";
            sw.WriteLine(finalString);

            for (int i = 0; i < data.Length; i++)
            {
                sw.WriteLine(data[i].ToString());
            }

        }
        
    }

    /// <summary>
    /// Creates a file starting with the header
    /// </summary>
    public static void CreateReport()
    {
        VerifyDirectory();
        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string finalString = "";
            finalString = reportHeader;
            //for (int i = 0; i < reportHeader.Length; i++)
            //{
            //    if (finalString != "")
            //    {
            //        finalString += reportSeparator;
            //    }
            //    finalString += reportHeader[i];
            //}
            //finalString += reportSeparator + timeStampHeader;
            sw.WriteLine(finalString);
        }
    }

    public static void ReadReport()
    {

        using (var reader = new StreamReader(GetFilePath()))
        {
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                listA.Add(values[0]);
                listB.Add(values[1]);
            }
        }
    }
#endregion


#region Operations

/// <summary>
/// if directory doesn't exist create a new directory
/// </summary>
static void VerifyDirectory()
    {
        string dir = GetDirectoryPath();
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    /// <summary>
    /// if report file doesn't exist create a new file
    /// </summary>
    static void VerifyFile()
    {
        string file = GetFilePath();
        if (!File.Exists(file))
        {
            CreateReport();
        }
    }

    #endregion


    #region Queries

    static string GetDirectoryPath()
    {
        return Application.dataPath + "/" + reportDirectoryName;
    }

    static string GetFilePath()
    {
        return GetDirectoryPath() + "/" + reportFileName;
    }

    static string GetTimeStamp()
    {
        return System.DateTime.UtcNow.ToString();
    }

    #endregion

}
