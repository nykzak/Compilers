using System;
using System.Text.RegularExpressions;
using System.Linq;


class MainProgram
{
    /* Ввод лексем из нашего программного кода */
    public static String[] KeyWords = new string[]
    {
            "procedure", "begin", "while", "var", "array", "end", "integer", "do", "of",
         "word","and","case","const","div","downto","else","file","for","function","goto","if","in","label",
         "mod","nil","not","or","packed","program","record","repeat","set","then","to","type","until","with"
        };

    public static String[] Separators = new string[]
    {
            ":", ";", "$", ".", ",", "+", "(", ")","[","]","*",">","-","/","<","↑",
    };

    public static String[] TwoSideSeparators = new string[]
    {
            ":=", "..","<=", ">="
    };

    static void Main(string[] args)
    {
        string Buffer = "";
        char[] CodeForAnalize = System.IO.File.ReadAllText("C:/Users/nykza/Desktop/компиляторы/lab1/lab1/lab1/pascal.txt").ToCharArray();

        Console.WriteLine("==========================Код на языке PASCAL==========================\n");

        /* Вывод кода */
        for (int i = 0; i < CodeForAnalize.Length; i++)
        {
            Console.Write(CodeForAnalize[i]);
        }

        /* Вывод ключевых слов */
        Console.WriteLine("\n==========================Ключевые слова==========================\n");
        Console.WriteLine("{0, -25} {1, 9}", "Список", "Тип");

        for (int i = 0; i < CodeForAnalize.Length; i++)
        {
            if (Char.IsLetter(CodeForAnalize[i]))
            {
                Buffer += CodeForAnalize[i];
                if (!Char.IsLetter(CodeForAnalize[i + 1]))
                {
                    if (KeyWords.Contains(Buffer.ToLower()))
                    {
                        Console.WriteLine("{0, -25} {1, 11}", Buffer, "|Ключевое слово|");
                    }
                    Buffer = "";
                }
            }
        }

        /* Вывод Разделителей */
        Console.WriteLine("\n==========================Разделители==========================\n");
        Console.WriteLine("{0, -15} {1, 17}", "Список", "Тип");

        for (int i = 0; i < CodeForAnalize.Length; i++)
        {
            if (Regex.IsMatch(CodeForAnalize[i].ToString(), "[:;$.,+()[\\]\\>\\-/<↑]"))
            {
                Buffer += CodeForAnalize[i];
                if (!Regex.IsMatch(CodeForAnalize[i + 1].ToString(), "[:;$.,+()[\\]\\>\\-/<↑]"))
                {
                    for (int j = 0; j < Buffer.Length; j++)
                    {
                        if (Buffer == ":")
                        {
                            if (Regex.IsMatch(CodeForAnalize[i + 1].ToString(), "[=]"))
                            { }
                            else
                            {
                                Console.WriteLine("{0, -23} {1, 15}",
                             Buffer[j], "|Разделитель|");
                            }
                        }
                         else if (Buffer == ">")
                        {
                            if (Regex.IsMatch(CodeForAnalize[i + 1].ToString(), "[=]"))
                            { }
                            else
                            {
                                Console.WriteLine("{0, -23} {1, 15}",
                             Buffer[j], "|Разделитель|");
                            }
                        }

                        else if (Buffer == "<")
                        {
                            if (Regex.IsMatch(CodeForAnalize[i + 1].ToString(), "[=]"))
                            { }
                            else
                            {
                                Console.WriteLine("{0, -23} {1, 15}",
                             Buffer[j], "|Разделитель|");
                            }
                        }
                        else if
                 (Separators.Contains(Buffer[j].ToString()))
                        {
                            Console.WriteLine("{0, -23} {1, 15}", Buffer[j], "|Разделитель|");
                        }
                    }
                }
            }
            else
            {
                Buffer = "";
            }

        }

        /* Вывод разделителей двухПозиционных */
        Console.WriteLine("\n==========================ДвухПозиционные разделители==========================\n");
        Console.WriteLine("{0, -15} {1, 25}", "Список", "Тип");

        for (int i = 0; i < CodeForAnalize.Length; i++)
        {
            if (Regex.IsMatch(CodeForAnalize[i].ToString(), "[:=..<=>=]"))
            {
                Buffer += CodeForAnalize[i];
                if (!Regex.IsMatch(CodeForAnalize[i + 1].ToString(), "[=.<>]"))
                {
                    for (int Iteration = 0; Iteration < Buffer.Length; Iteration++)
                    {
                        if (TwoSideSeparators.Contains(Buffer.ToString()))
                        {
                            Console.WriteLine("{0, -23} {1, 31}", Buffer, "|ДвухПозиционные разделитель|");
                        }
                        Buffer = "";
                    }
                }
               
            }
            else
            {
                Buffer = "";
            }
        }
        /* Вывод цифр */
        Console.WriteLine("\n==========================Цифры==========================\n");
        Console.WriteLine("{0, -1} {1, 24}", "Список", "Тип");

        for (int i = 0; i < CodeForAnalize.Length; i++)
        {
            if (Char.IsDigit(CodeForAnalize[i]))
            {
                Buffer += CodeForAnalize[i];
                if (!Char.IsDigit(CodeForAnalize[i + 1]))
                {
                    if (Regex.IsMatch(Buffer, @"\d"))
                    {
                        Console.WriteLine("{0, -19} {1, 13}", Buffer, "|Цифра|");
                    }
                }
            }
            else
            {
                Buffer = "";
            }
        }
        /* Вывод идентификаторов */
        Console.WriteLine("\n==========================Идентификаторы==========================\n");
        Console.WriteLine("{0, -15} {1, 19}", "Список", "Тип");
        for (int i = 0; i < CodeForAnalize.Length; i++)
        {
            if (Char.IsLetter(CodeForAnalize[i]))
            {
                Buffer += CodeForAnalize[i];

                if (!Char.IsLetter(CodeForAnalize[i + 1]) && !KeyWords.Contains(Buffer.ToLower()))
                {
                    Console.WriteLine("{0, -25} {1, 13}", Buffer, "|Идентификатор|");
                }
                if (!Char.IsLetter(CodeForAnalize[i + 1]))
                {
                    Buffer = "";
                }
            }
        }
    }
}
 




