using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Задача_8
{
    class PointEntry
    {
        public int Degree
        {
            get
            {
                return Near.Count;
            }
        }
        public List<PointEntry> Near { get; set; }
        public PointEntry()
        {
            Near = null;
        }
    }
}
Листинг программы:
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Задача_8
{
    class Program
    {
        public static Dictionary<int, PointEntry> _dict = new Dictionary<int, PointEntry>();
        public static byte[][] save;
        public static int u = 0;
        public static byte[][] ReadFromFile(string file_name)
        {
            int count1 = 0, count2 = 0;
            using (FileStream sf = new FileStream(file_name, FileMode.OpenOrCreate)) { }
            using (StreamReader reader = new StreamReader(file_name))
            {
                while (reader.Peek() != -1)
                {
                    count1++;
                    string str = Regex.Replace(reader.ReadLine().Trim(), @"\s+", " ");
                    if (str != "")
                    {
                        if (count2 == 0)
                        {
                            string[] chisla = str.Split(new char[] { ' ' });
                            count2 = chisla.Length;
                        }
                    }
                    else count1--;
                }
            }

            byte[][] arr = new byte[count1][];
            using (FileStream sf = new FileStream(file_name, FileMode.OpenOrCreate)) { }
            using (StreamReader reader = new StreamReader(file_name))
            {
                try
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = new byte[count2];
                        string str = Regex.Replace(reader.ReadLine().Trim(), @"\s+", " ");
                        if (str != "")
                        {
                            string[] chisla = str.Split(new char[] { ' ' });
                            if (chisla.Length != count2) throw new Exception();
                            for (int j = 0; j < arr[i].Length; j++)
                            {
                                arr[i][j] = Convert.ToByte(chisla[j]);
                                if (arr[i][j] > 1) throw new Exception();
                            }
                        }
                        else i--;
                    }

                    for (int i = 0; i < arr[0].Length; i++)
                    {
                        int count = 0;
                        for (int j = 0; j < arr.Length; j++) count += arr[j][i];
                        if (count != 2) throw new Exception();
                    }
                }
                catch { Console.WriteLine($"Ошибка чтения из файла. Проверьте файл для ввода: в нем должна быть матрица из единиц и нулей\nГде строки - это вершины графа, а столбцы - ребра графа"); }
            }
            return arr;
        }

        public static PointEntry ToPoint(byte[][] arr)
        {
            Dictionary<int, PointEntry> dict = new Dictionary<int, PointEntry>();
            for (int i = 0; i < arr.Length; i++)
            {
                dict.Add(i, new PointEntry());
                dict[i].Near = new List<PointEntry>();
            }
            for (int i = 0; i < arr[0].Length; i++)
            {
                int[] ver = new int[2];
                byte count = 0;
                for (int j = 0; j < arr.Length; j++)
                    if (arr[j][i] == 1) ver[count++] = j;

                dict[ver[0]].Near.Add(dict[ver[1]]);
                dict[ver[1]].Near.Add(dict[ver[0]]);
            }

            _dict = dict;
            //foreach (PointEntry point in dict.Values) if (point.Degree % 2 == 1) return point;
            return dict[0];
        }

        public static bool EulerParse(byte[][] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                int count = (from chisl in arr[i] where chisl == 1 select chisl).Count();
                if (count % 2 == 1) return false;
            }
            return true;
        }

        public static bool EulerWay(List<int> ways, PointEntry point)
        {
            int count = 0;

            int key = 0;
            foreach (KeyValuePair<int, PointEntry> pair in _dict)
                if (pair.Value == point)
                {
                    key = pair.Key;
                    break;
                }

            foreach (PointEntry next in point.Near)
            {
                int num = 0;
                foreach (KeyValuePair<int, PointEntry> pair in _dict)
                    if (pair.Value == next)
                    {
                        num = pair.Key;
                        break;
                    }
                int way = 0;
                for (int i = 0; i < save[0].Length; i++)
                    if (save[key][i] == 1 && save[num][i] == 1)
                    {
                        way = i;
                        break;
                    }

                if (!ways.Contains(way))
                {
                    List<int> help = new List<int>(ways);
                    help.Add(way);
                    if (EulerWay(help, next))
                    {
                        Console.WriteLine($"{key} => ");
                        return true;
                    }
                    else count++;
                }
            }
            if (ways.Count == save[0].Length)
            {
                Console.WriteLine($"\n{key} => ");
                u = key;
                return true;
            }
            else return false;
        }
        static void Main(string[] args)
        {
            string input_f = "input.txt";
            byte[][] matr = ReadFromFile(input_f);
            save = matr;
            if (matr.Length != 0)
            {
                if (EulerParse(matr))
                {
                    PointEntry first = ToPoint(matr);
                    List<int> ways = new List<int>();
                    EulerWay(ways, first);
                    Console.WriteLine($"{u}");
                }
                else Console.WriteLine("Введенный граф не является Эйлеровым");
            }
            else Console.WriteLine("Граф не введен. Перезапустите программу, введя граф в файле или использовав ручной ввод");
            Console.ReadKey();
        }
    }
}
