using System;
using System.IO;
using System.Linq;
namespace lab2
{
    class MainProgram
    {
        static bool mainkey = false; //Меняется на true в течении проверок , в ином случае остаётся false и выводит конкретную ошибку
        static string texterror = ""; //При возникновении ошибок происходит запись
        static string[] separators = {"=", ".", ";", "(", ")"}; //Создание массива строк , с разделителями
        static void Main(string[] args) 
        {
            string[] CodeForAnalize = File.ReadAllLines("pascal.txt"); string code = CodeForAnalize[0];
            Console.WriteLine("Программный код на Pascal:");Console.WriteLine(code);Console.WriteLine("\nСписок ошибок:");
            if (lexanalyze(code))
            {
                string firstcode = ""; int a = 0;
                if (lexanalyze(code)) //Лексический анализ
                {
                    if (IfThenChecker(code)) //проверка на присутствие if & then
                    {
                        int Start = code.IndexOf("if") + 2, End = code.IndexOf("then");
                        for (int i = Start; i < End; i++){ firstcode += code[i]; }
                        if (a == 0)
                        {
                            if (logexpr(firstcode)) // проверка на log expr
                            {
                                string secondcode = ""; Start = code.Length;
                                for (int i = End + 4; i < Start; i++)
                                { if (code[i] != ';') secondcode += code[i]; }
                                if (statement(secondcode)) // проверка на statement
                                {
                                    for (int i = 0; i < code.Length; i++)
                                    {
                                        if (code[i] == '(') a++; 
                                        if (code[i] == ')') a--;
                                    }
                                    if (a == 0)
                                    {
                                        if (code[code.Length - 1] != ';')
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("\nНе найдено ; в конце");
                                            Console.ResetColor();
                                        }
                                        else {
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.WriteLine("\nОшибок не обнаружено!");
                                            Console.ResetColor();
                                        } 
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("\nНе найдены скобка ( или )");
                                        Console.ResetColor();
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("\nОшибка Statement после then");
                                    Console.WriteLine(texterror);
                                    Console.ResetColor();
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(texterror);
                                Console.WriteLine("Ошибка в logical expression!");
                                Console.ResetColor();
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(texterror);
                        Console.ResetColor();
                    }
                }
            }
            Console.ReadLine();
        }
        //обработка строки,возвращает без лишних пробелов
        static string fixingSpacing(string mytoken) 
        {
            if (mytoken == "")
            {
                return "";
            }
            int tokPosition1 = 0;
            while (tokPosition1 < mytoken.Length && mytoken[tokPosition1] == ' ' || mytoken[tokPosition1] == '(')
            {
                if (mytoken[tokPosition1] == ' ')
                    mytoken = mytoken.Remove(tokPosition1, 1);
                else tokPosition1++;
                if (mytoken == "") return "";
            }
            if (mytoken == "")
            {
                return "";
            }
            tokPosition1--;
            if (tokPosition1 == mytoken.Length) return "";
            int tokPosition2 = mytoken.Length - 1;
            while (mytoken != "" && tokPosition2 > 0 && mytoken[tokPosition2] == ' ' || mytoken[tokPosition2] == ')')
            {
                if (mytoken[tokPosition2] == ' ') 
                mytoken = mytoken.Remove(tokPosition2, 1);
                tokPosition2--;
                
                if (mytoken == "") return "";
            }
            if (mytoken == "")
            {
                return "";
            }
            tokPosition2++;
            if (tokPosition2 == 0) return "";
            int Brackets = 0, tokPositionEnd = tokPosition1 + 1;
            if (tokPosition1 >= 0 && tokPosition1 < mytoken.Length && mytoken[tokPosition1] == '(')
            {
                while (tokPosition1 >= 0)
                {
                    while (tokPositionEnd < mytoken.Length && (mytoken[tokPositionEnd] != ')' || Brackets != 0))
                    {
                        if (mytoken[tokPositionEnd] == '(') Brackets++;
                        if (mytoken[tokPositionEnd] == ')') Brackets--;
                        tokPositionEnd++;
                    }
                    if (tokPositionEnd == mytoken.Length)
                    {
                        while (tokPosition1 >= 0)
                        {
                            mytoken = mytoken.Remove(tokPosition1, 1);
                            tokPosition1--;
                        }
                        return mytoken;
                    }
                    tokPosition1--; tokPositionEnd++;
                }
                tokPosition1 = 0; tokPosition2 = mytoken.Length - 1;
                while (mytoken[tokPosition1] == '(' && mytoken[tokPosition2] == ')')
                {
                    mytoken = mytoken.Remove(tokPosition2, 1);
                    tokPosition2--;
                    mytoken = mytoken.Remove(tokPosition1, 1);
                }
            }
            else
            {
                Brackets = 0; tokPositionEnd = tokPosition2 - 1;
                if (tokPosition2 < mytoken.Length && tokPosition2 >= 0 && mytoken[tokPosition2] == ')')
                {
                    while (tokPosition2 < mytoken.Length)
                    {
                        while (tokPositionEnd > 0 && (mytoken[tokPositionEnd] != '(' || Brackets != 0))
                        {
                            if (mytoken[tokPositionEnd] == '(') Brackets++;
                            if (mytoken[tokPositionEnd] == ')') Brackets--;
                            tokPositionEnd--;
                        }

                        if (tokPositionEnd == 0)
                        {
                            while (tokPosition2 < mytoken.Length)
                            {
                                mytoken = mytoken.Remove(tokPosition2, 1);
                            }
                            return mytoken;
                        }
                        tokPosition2++;
                        tokPositionEnd--;
                    }
                    tokPosition1 = 0; tokPosition2 = mytoken.Length - 1;
                    while (mytoken[tokPosition1] == '(' && mytoken[tokPosition2] == ')')
                    {
                        mytoken = mytoken.Remove(tokPosition2, 1);
                        tokPosition2--;
                        mytoken = mytoken.Remove(tokPosition1, 1);
                    }
                }
            }
            return mytoken;
        }

        //лекс.анализатор
        static bool lexanalyze(string mytoken) 
        {
            for (int i = 0; i < mytoken.Length; i++)
            {
                if (Char.IsDigit(mytoken[i]) || Char.IsLetter(mytoken[i]) || separators.Contains(mytoken[i].ToString()) || mytoken[i] == ' ') ;
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка в лексическом анализаторе:  " + mytoken[i]);
                    Console.ResetColor();
                    return false;

                }
            }
            return true;
        }
        //проверка на присутстивие if & then
        static bool IfThenChecker(string mytoken) 
        {
            bool iffind = false, thenfind = false;
            for (int i = 0; i < mytoken.Length; i++)
            {
                string word = "";
                while (i < mytoken.Length && mytoken[i] != ' ')
                {
                    word += mytoken[i];
                    i++;
                }
                if (word == "if" && !iffind)
                {
                    if (mytoken[i] == ' ') iffind = true;
                    else return false;
                }
                if (word == "then" && !thenfind)
                {
                    if (mytoken.IndexOf("then") + 4 <= mytoken.Length)
                    {
                        if ((mytoken[mytoken.IndexOf("then") - 1] == ' ') && mytoken[mytoken.IndexOf("then") + 4] == ' ') thenfind = true;
                        else return false;
                    }
                }
            }
            if (iffind && thenfind) return true;
            if (!iffind) texterror += "\nОшибка найдена: if отсутствует!";
            if (!thenfind) texterror += "\nОшибка найдена: then отсутствует!";
            return false;
        }
        //проверка на наличие ошибок , собирая в данной функции всё в переменную
        static bool logexpr(string mytoken) 
        {
            if (mainkey) return false;
            string one = "", two = ""; bool check = false;
            for (int i = 0; i < mytoken.Length; i++)
            {
                if (!check)
                {
                    if (mytoken[i] != ' ') one += mytoken[i];
                    else
                    {
                        two += mytoken[i];
                        two += one;
                        one = "";
                    }
                }
                else one += mytoken[i];
            }
            two += " "; two += one;
            if (!check)
            {
                if (!ForEqual(two)) return false;  
            }
            else
            {
                if (!logexpr(one)) return false;
            }
            return true;
        }
        //проверка на токен = 
        static bool ForEqual(string mytoken) //проверка на присутствие токена = 
        {
            if (mainkey) return false;
            string Word = ""; bool Keys = false;
            for (int i = 0; i < mytoken.Length; i++)
            {
                if (!Keys)
                {
                    if (mytoken[i] != '=') Word += mytoken[i];
                    else
                    {
                        switch (mytoken[i])
                        {
                            case '=':
                                if (mytoken[i + 1] != ' ' || mytoken[i - 1] != ' ')
                                {
                                    mainkey = true;
                                    texterror += "\nпосле = присутствуют лишнии символы";
                                    return false;
                                }
                                else i++;
                                if (!expression(Word))
                                {
                                    return false;
                                }
                                break;
                        }
                        Word = "";
                    }
                }
            }
            if (!expression(Word))
            {
                return false;
            }
            return true;
        }
        //Примечание: Statement ошибка выводиться в общем плане , сообщается о целостности нарушения структуры
        static bool expression(string mytoken)
        {
            if (mainkey) return false;
            mytoken = fixingSpacing(mytoken);
            if (mytoken == "")
            {
                texterror += "\nОшибка! Нет Statement!";
                return false;
            }
            if (!myterm(mytoken)) return false;
            return true;
        }

        // промежуточный вариант между экспрешеном и идентификатором
        static bool myterm(string mytoken)
        {
            if (mainkey) return false;
            if(mytoken == "")
            {
                mainkey = true;
                texterror += "\n Ошибка! Нет Statement";
                return false;
            }
            if (Identifier(mytoken)) return true;
            if (Identifiertwo(mytoken)) return true;
            return false;
        }
        
        //Проверка точек и скобок
        static bool Identifier(string mytoken)
        {
            if (mainkey) return false;
            if (mytoken == "")
            {
                mainkey = true;
                texterror += "\nОшибка! Нет Statement!";
                return false;
            }
            string Word = "";
            for (int i = 0; i < mytoken.Length; i++)
            {
                if (Char.IsLetter(mytoken[i]))
                {
                    Word += mytoken[i];
                }
                if (mytoken[i] == '.')
                {
                    if (Word == "")
                    {
                        mainkey = true;
                        texterror += "\nОшибка:нету identifier перед  " + mytoken;
                        return false;
                    }
                    if (mytoken.Length <= i + 1)
                    {
                        mainkey = true;
                        texterror += "\nОшибка:нету identifier после  " + mytoken;
                        return false;
                    }
                    else
                    {
                        if (!Char.IsLetter(mytoken[i + 1]))
                        {
                            mainkey = true;
                            texterror += "\nОшибка:нету identifier после  " + mytoken;
                            return false;
                        }
                    }
                    Word = "";
                }
                if (mytoken[i] == '(') return false;
                if (mytoken[i] == ' ')
                {
                    mainkey = true;
                    texterror += "\nЗдесь ошибка:" + mytoken;
                    return false;
                }
            }
            return true;
        }
        static bool Identifiertwo(string mytoken)
        {
            if (mainkey) return false;
            if (mytoken == "")
            {
                mainkey = true;
                texterror += "\nОшибка! Нет Statement!";
                return false;
            }
            string Word = ""; int j = 0;
            while (j < mytoken.Length && mytoken[j] != '(')
            {
                Word += mytoken[j];
                j++;
            }
            if (!Identifier(Word)) return false;
            j++;
            Word = ""; int bracketss = 1;
            while (j < mytoken.Length && bracketss != 0)
            {
                if (mytoken[j] == '(' ) bracketss++;
                if (mytoken[j] == ')' ) bracketss--;
                j++;
            }

            if (bracketss > 0)
            {
                mainkey = true;
                texterror = "\nОшибка: отсутствует ) - " + mytoken;
                return false;
            }
            return true;
        }
        //Итоговая проверка
        static bool statement(string mytoken)
        {
            if (mainkey) return false;
            if (mytoken == "")
            {
                mainkey = true;
                texterror += "\nОшибка! нету Statement!";
                return false;
            }
            mytoken = fixingSpacing(mytoken);
            if (Identifier(mytoken)) return true; 
            if (Identifiertwo(mytoken)) return true; 
            mainkey = true;
            return false;
        }
    }
}
