using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Phrase_Search
{
    public class Program
    {
        public static void Main()
        {
            Console.Write("Введите путь к директории со скриптами: ");

            string path = Console.ReadLine()!;

            Directory.CreateDirectory(path + @"\AllPhrases");
            Directory.CreateDirectory(path + @"\EnglishPhrases");

            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] files = directory.GetFiles();

            foreach (FileInfo file in files)
            {
                GetPhrasesFromFile(directory, file);
            }

            Console.WriteLine("Выполнено");
            Console.ReadLine();
        }
        
        private static void GetPhrasesFromFile(DirectoryInfo directory, FileInfo file)
        {
            string script = GetScriptFromFile(file);

            List<string> allPhrases = GetAllPhrases(script);
            List<string> englishPhrases = GetEnglishPhrases(allPhrases);

            WritePhrasesToFile(directory, file, allPhrases, "AllPhrases");
            WritePhrasesToFile(directory, file, englishPhrases, "EnglishPhrases");
        }
        private static string GetScriptFromFile(FileInfo file)
        {
            string script = "";

            using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    script = streamReader.ReadToEnd();
                }
            }

            return script;
        }
        private static List<string> GetAllPhrases(string script)
        {
            Regex allPhrasesRegex = new Regex("\"[^\"]*((\\\\\")?.*(\\\\\"))?[^\"]*\"");
            MatchCollection allPhrasesMatches = allPhrasesRegex.Matches(script);

            List<string> allPhrases = (from match in allPhrasesMatches select match.Value).ToList();

            for (int i = 0; i < allPhrases.Count; i++)
            {
                allPhrases[i] = allPhrases[i].Replace(Environment.NewLine, " ");
                allPhrases[i] = RemovingMultipleSpaces(allPhrases[i]);
            }

            return allPhrases;
        }
        private static List<string> GetEnglishPhrases(List<string> allPhrases)
        {
            Regex englishPhrasesRegex = new Regex("\"[^А-Яа-я]+\"");

            List<string> englishPhrases = new List<string>();

            foreach (string phrase in allPhrases)
            {
                string s = englishPhrasesRegex.Match(phrase).Value;

                if (!string.IsNullOrEmpty(s))
                {
                    englishPhrases.Add(s);
                }
            }

            return englishPhrases;
        }
        private static void WritePhrasesToFile(DirectoryInfo directory, FileInfo file, List<string> phrases, string directoryName)
        {
            using (FileStream fileStream = new FileStream(directory.FullName + $"\\{directoryName}\\{file.Name}.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    foreach (string phrase in phrases)
                    {
                        streamWriter.WriteLine(phrase);
                    }
                }
            }
        }
        private static string RemovingMultipleSpaces(string str)
        {
            string s1;
            string s2 = str;

            do
            {
                s1 = s2;
                s2 = s1.Replace("  ", " ");
            }
            while (s1 != s2);

            return s2;
        }
    }
}