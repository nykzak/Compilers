using System;
using System.Collections.Generic;
using System.IO;

namespace var4
{


    /*  
<if stmt> ::= if  <log_expr> then <stmt>;
<stmt> ::= <command>
<command> ::= <term>(<term>)
<log expr> ::= <expr> = <expr>
<expr> ::= <term>{.<term>} 
<term> ::= id
   */

    class Program
    {
        
        private static List<String> keyArray = new List<String> {"="};
        private static List<String> dArray = new List<String> { ".", ":", ";", "'", "(", ")", "," };
        private static List<String> tokens = new List<String> { };
        private static string errorMessage = "";
        private static int tokenPosition = 0;


        //Main выводит код на паскаль , показывает ошибки
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("pascal.txt");
            string str = lines[0];
            Console.WriteLine("Код на Pascal:");
            Console.WriteLine(str);
            if (getTokens(str))
            {
                if (ifStatement())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Ошибок не найдено");
                    Console.ReadLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(errorMessage);
                    Console.ReadLine();

                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(errorMessage);
                Console.ReadLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
       
        //Возвращение следующего токена
        static string scan()
        {
            if (tokenPosition >= tokens.Count) return "";
            return tokens[tokenPosition];
        }


        //Функция получает токена с строки , помещаются в список и проверяются на лекс.
        private static bool getTokens(string str)
        {
            string token = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (keyArray.Contains(str[i].ToString()) || dArray.Contains(str[i].ToString()) || Char.IsDigit(str[i]) || Char.IsLetter(str[i]) || str[i] == ' ')
                {


                    if (Char.IsDigit(str[i]) || Char.IsLetter(str[i])) token += str[i];
                    else
                    {
                        if (str[i] == ' ')
                        {
                            if (token != "") { tokens.Add(token); token = ""; }
                        }
                        else
                        {
                            if (token != "") tokens.Add(token);
                            token = str[i].ToString();
                            tokens.Add(token);
                            token = "";

                        }
                    }
                }
                else
                {
                    string errorLine = "";
                    for (int k = 0; k <= i; k++) errorLine += str[k];
                    errorMessage += "Ваш код не может содержать данный символ: " + str[i] + " в " + errorLine;
                    return false;
                }
            }
            return true;
        }

        //<if statement> ::= if <logical expr> then <term>;
        private static bool ifStatement()
        {
            int currentPostion = tokenPosition;
            string errorLine = "";
            string token = "";
            token = scan();
            

            if (token.ToLower() == "if")
            {
                tokenPosition++;
                token = scan();
                if (tokenPosition >= tokens.Count) { errorMessage += "\nОшибка, отсутствует logical expression"; return false; }

                currentPostion = tokenPosition;


                if (logicalExpr(token))
                {
                    if (tokenPosition >= tokens.Count) { errorMessage += "\nОшибка, отсутствует THEN"; return false; }

                    token = scan();
                    if (token.ToLower() == "then")
                    {
                        tokenPosition++;
                        token = scan();
                        if (tokenPosition >= tokens.Count) { errorMessage += "\nОшибка, отсутствует expression после THEN"; return false; }

                        currentPostion = tokenPosition;

                        if (stmt(token))
                        {

                            if (tokenPosition >= tokens.Count) { errorMessage += "\nОшибка, отсутствует ;"; return false; }

                            token = scan();
                            if (token == ";") return true;
                            else
                            {
                                errorMessage += "\nотсутствует ;";
                                return false;
                            }
                        }
                        else
                        {
                            for (int i = currentPostion; i < tokenPosition; i++) errorLine += tokens[i] + " ";
                            errorMessage += "\nОшибка был найден после THEN в " + errorLine;
                            return false;
                        }

                    }
                    else
                    {
                        errorMessage += "\nОжидался THEN но " + token + " был найден.";
                        return false;
                    }
                }
                else
                {
                    for (int i = currentPostion; i < tokenPosition; i++) errorLine += tokens[i] + " ";
                    errorMessage += "\nОшибка был найден в boolean expression в " + errorLine;
                    return false;
                }


            }
            else
            {
                errorMessage += "\nОжидался IF но " + token + " был найден.";
                return false;
            }


            return true;
        }

        //<log expr> ::= <expr> = <expr>
        private static bool logicalExpr(string token)
        {
            int currentPostion = tokenPosition;
            string errorLine = "";

            if (expr(token))
            {
                token = scan();
                if (tokenPosition >= tokens.Count) { errorMessage += "\nОшибка, отсутствует THEN"; return false; }


                switch (token)
                {
                    case "=":
                        tokenPosition++;
                        token = scan();
                        if (tokenPosition >= tokens.Count) { errorMessage += "\nОшибка, ошибка в boolean expression"; return false; }
                        if (token == "=") { errorMessage += "\nОшибка, = уже найдено"; return false; }
                        break;
                    default: return true;
                }

                if (tokenPosition >= tokens.Count) { errorMessage += "\nОшибка, ошибка в boolean expression"; return false; }

                if (expr(token)) return true;
                else
                {
                    for (int i = currentPostion; i < tokenPosition; i++) errorLine += tokens[i] + " ";
                    errorMessage += "\nОшибка в boolean expression " + errorLine;
                    return false;
                }
            }
            else
            {
                errorMessage += "\nОшибка в boolean expression";
                return false;
            }
        }
        //<stmt> ::= <command>
        private static bool stmt(string token)
        {
            if (command(token)) return true;
            else return false;
        }
        //<command> ::= <term>(<term>)
        private static bool command(string token)
        {
            int currentPostion = tokenPosition;
            string errorLine = "";

            if (term(token))
            {
                token = scan();
                if (token == "(")
                {
                    tokenPosition++;
                    token = scan();
                    if (tokenPosition >= tokens.Count) { errorMessage += "\nОшибка, ожидался ID после ."; return false; }
                    if (term(token))
                    {
                        if (tokenPosition >= tokens.Count) { errorMessage += "\nОшибка, ожидался )."; return false; }
                        token = scan();
                        if (token != ")")
                        {
                            for (int i = currentPostion; i < tokenPosition; i++) errorLine += tokens[i] + " ";
                            errorMessage += "\nОшибка в ID " + errorLine + " ожидался ), но " + token + " был найден";
                            return false;
                        }
                        tokenPosition++;
                        token = scan();
                        return true;
                    }
                    else
                    {
                        for (int i = currentPostion; i < tokenPosition; i++) errorLine += tokens[i] + " ";
                        errorMessage += "\nОшибка в ID " + errorLine;
                        return false;
                    }
                }
                else
                {
                    errorMessage += "Ошибка в COMMAND, ожидался ( но " + token + " был найден.";
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        //<expr> ::= <term>{.<term>} 
        private static bool expr(string token)
        {
            if (term(token)) return true;
            else return false;
        }

        //<term> ::= id 
        private static bool term(string token)
        {
            int currentPostion = tokenPosition;
            string errorLine = "";

            for (int i = 0; i < token.Length; i++)
            {
                if (!Char.IsLetter(token[i]))
                {
                    errorMessage += "\nОшибка, ID может содержать только буквы, но " + tokens[tokenPosition][i] + " был найден в " + tokens[tokenPosition];
                    return false;
                }
            }
            tokenPosition++;
            token = scan();
            if (tokenPosition >= tokens.Count) return true;
            if (token == ".")
            {
                tokenPosition++;
                token = scan();
                if (tokenPosition >= tokens.Count) { errorMessage += "\nОшибка, ожидался ID после ."; return false; }
                if (term(token)) return true;
                else
                {
                    for (int i = currentPostion; i < tokenPosition; i++) errorLine += tokens[i] + " ";
                    errorMessage += "\nОшибка в ID " + errorLine;
                    return false;
                }
            }


            return true;
        }

    }
}
