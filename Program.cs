using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace Task_22
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "C:\\Users\\Антон\\Desktop\\map.txt";
            if (!File.Exists(path)) {
                Console.WriteLine("Файл не найден");
                return;
            }
            MyHashMap<string,int>tag= new MyHashMap<string,int>();
            string pattern = @"</?[a-zA-Z][a-zA-Z0-9]*>";

            try
            {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    MatchCollection matches = Regex.Matches(line, pattern);
                    foreach (Match match in matches)
                    {
                        string tagg = match.Value;
                        string normalizteg = tagg.Replace("/", "").Replace("<", "").Replace(">", "").ToLower();
                        int currentCount = tag.Get(normalizteg);
                        if (currentCount == default(int))
                        {
                            tag.Put(normalizteg, 1);
                        }
                        else
                        {
                            tag.Put(normalizteg, currentCount + 1);
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Ошибка чтения файла"+ex.Message);
            }
            var set =tag.EntrySet();
            foreach (var t in set) {
                Console.WriteLine($"{t.Key}->{t.Value}");
            }
        }
    }
}
