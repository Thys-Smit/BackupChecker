using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;


namespace BackupChecker
{
    class Program
    {

        #region Declarations

        static string folderPath = ConfigurationManager.AppSettings["folderPath"];
        static string fileName = ConfigurationManager.AppSettings["fileName"];
        static string[] directoryNames = ConfigurationManager.AppSettings["directoryNames"].Split(',');
        static string[] entries;

        #endregion

        static void Main(string[] args)
        {
            getFiles();
        }

        public static void getFiles()
        {
            List<string> latestEntry = new List<string>();
            List<DateTime> entryList = new List<DateTime>();
            fileName = folderPath + @"\" + fileName;

            File.WriteAllText(fileName, "");
            
            foreach (string name in directoryNames)
            {
                string[] fileRegex = ConfigurationManager.AppSettings[name].Split(',');

                latestEntry.Clear();
                //foreach (string file in fileNames)
                //{
                    try
                    {
                        entries = Directory.GetFileSystemEntries(folderPath + @"\" + name,"*.rar", SearchOption.TopDirectoryOnly);
                        foreach (string file in fileRegex)
                        {
                            foreach (string entry in entries)
                            {
                                bool isMatch = Regex.IsMatch(Path.GetFileNameWithoutExtension(entry), file);

                                if (isMatch) entryList.Add(File.GetCreationTime(entry));
                            }

                            if (entryList.Count > 0) latestEntry.Add(entryList.Max().ToShortDateString());
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.ReadLine();
                        entries = null;
                        continue;
                    }
                //}

                if (entries != null)
                {
                    string header = buildString(fileRegex, "Site");
                    string values = buildString(entries, name, latestEntry);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true))
                    {
                        file.WriteLine(header);
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

                try
                {
                    fileName = Path.GetFileNameWithoutExtension(entry);
                
                    getDate = fileName.Split('_');

                    count = getDate.Count<string>();

                    date = getDate[count - 1];
                    date = date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);
                
                    latestDate[i] = Convert.ToDateTime(date);
                    iIndex = Array.IndexOf(latestDate, latestDate.Max()); 
                }
                catch
                {
                    continue;
                }
                
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
                //sb.Append(Environment.NewLine);
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
