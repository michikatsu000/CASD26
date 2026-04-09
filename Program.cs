using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
namespace Task_23
{
    internal class Program
    {
        public enum VarType {
        Int,
        Float,
        Double,
        Unknown
        }
        public class VariableInfo {
            public VarType Type { get; set; }
            public string Value { get; set; }
            public VariableInfo(VarType type, string value) {
                Type = type;
                Value = value;
            }
        }
        static void CreateExampleFile(string filename)
        {
            string exampleContent = @"int count = 100;
float price = 250;
double distance = 5000;
int count = 200;
string name = test;
int score = 300;";

            File.WriteAllText(filename, exampleContent);
            Console.WriteLine($"Создан файл-пример: {filename}\n");
        }
        static void Main(string[] args)
        {
            string inputPath = "map2.txt";
            string outputPath = "output.txt";

            if (!File.Exists(inputPath)) {
                Console.WriteLine($"Файл {inputPath} не найден");
                CreateExampleFile(inputPath);
            }
            MyHashMap<string,VariableInfo> variabes= new MyHashMap<string,VariableInfo>();

            //Мн-во допустимых типов 
            HashSet<string> validTypes = new HashSet<string>
            {
                "int", "float", "double"
            };

            string content = File.ReadAllText(inputPath);
            // \b -граница слова 
            // группа допустимых слов(int|float|double) |-или
            // \b-граница слова 
            // \s+- (\s-пробел таб или перенос строки )+-один или более
            // потом первая буква или нижнее подчёркивание _
            // \w*- буквы/цифры/_ *-сколько угодно 
            // \s*-сколько угодно пробелов
            // знак равенства =
            // \s*-сколько угодно пробелов 
            // группа (\d+) - \d-цифра + одна и более
            // \s* сколько угодно пробелов
            // ; - конец выражения в языке проги
            string pattern = @"\b(int|float|double)\b\s+([a-zA-Z_]\w*)\s*=\s*(\d+)\s*;";
            var matches=Regex.Matches(content, pattern);
            foreach (Match match in matches) {
            string type = match.Groups[1].Value;
            string name=match.Groups[2].Value;
            string value = match.Groups[3].Value;
                if (!validTypes.Contains(type))
                {
                    Console.WriteLine($"Недопустимый тип");
                    continue;
                }
              VarType type2=VarType.Unknown;
                if (type == "int") type2 = VarType.Int;
                else if (type=="float") type2= VarType.Float;
                else if (type=="double") type2= VarType.Double;
                if (variabes.ContainsKey(name)) {
                    Console.WriteLine($"Переопределение переменной: {name}");
                    continue;
                }
                variabes.Put(name,new VariableInfo(type2,value));
            }
            using (StreamWriter wr = new StreamWriter(outputPath))
            { 
            var entries=variabes.EntrySet();
                foreach (var entry in entries){
                    string typestr = entry.Value.Type.ToString().ToLower();
                    wr.WriteLine($"{typestr}=> {entry.Key}({entry.Value.Value})");
                }
            }
            Console.WriteLine("\nРезультат записан в файл output.txt");
        }
    }
}
