﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace BackupChecker
{
    class Program
    {

        #region Declarations

        static string folderPath = ConfigurationManager.AppSettings["folderPath"];
        static string fileName = ConfigurationManager.AppSettings["fileName"];
        static string[] directoryNames = ConfigurationManager.AppSettings["directoryNames"].Split(',');
        static string[] fileNames = ConfigurationManager.AppSettings["searchNames"].Split(',');
        static string[] entries;

        #endregion

        static void Main(string[] args)
        {
            getFiles();
        }

        public static void getFiles()
        {
            List<string> latestEntry = new List<string>();
            fileName = folderPath + @"\" + fileName;

            string header = buildString(fileNames, "Site");
            System.IO.File.WriteAllText(fileName, header);

            foreach (string name in directoryNames)
            {
                latestEntry.Clear();
                foreach (string file in fileNames)
                {
                    try
                    {
                        entries = Directory.GetFileSystemEntries(folderPath + @"\" + name, "*" + file + "*", SearchOption.TopDirectoryOnly);
                        latestEntry.Add(getLatestBackup(entries));
                    }
                    catch
                    {
                        entries = null;
                        continue;
                    }
                }

                if (entries != null)
                {
                    string values = buildString(entries, name, latestEntry);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true))
                    {
                        file.WriteLine(values);
                    }

                }

            }

        }

        public static string getLatestBackup(string[] entries)
        {
            DateTime[] latestDate = new DateTime[entries.Count()];
            string fileName;
            string date;
            string[] getDate;
            int count;
            int i = 0;
            int iIndex = 0;

            foreach (string entry in entries)
            {

                fileName = Path.GetFileNameWithoutExtension(entry);
                getDate = fileName.Split('_');

                count = getDate.Count<string>();

                date = getDate[count - 1];
                date = date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);

                latestDate[i] = Convert.ToDateTime(date);
                iIndex = Array.IndexOf(latestDate, latestDate.Max());

                i++;
            }

            return latestDate[iIndex].ToShortDateString();
        }

        public static string buildString(string[] stringArray, string rowTitle, List<string> valueList = null)
        {
            StringBuilder sb = new StringBuilder();

            if (valueList == null)
            {
                sb.Append(rowTitle);

                foreach (string str in stringArray)
                {
                    sb.Append("," + str);
                }
                sb.Append(Environment.NewLine);
            }
            else
            {
                sb.Append(rowTitle);

                foreach (string value in valueList)
                {
                    sb.Append("," + value);
                }
            }

            return sb.ToString();
        }


    }
}