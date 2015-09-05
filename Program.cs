/*-----------------------------------------
Написать приложение-парсер для параметров, введенных из консоли.
Список правил:
CommandParser.exe [/?] [/help] [-help] [-k key value] [-ping] [-print <print a value>]
CommandParser.exe - вызов приложения без параметров показывает то же, что и вызов помощи
/?, /help,-help - вызов помощи
если введена неверная команда - показать эту команду и сообщение, что команда <command> не поддерживается
Command <command> is not supported, use CommandParser.exe /? to see set of allowed commands
Если введено несколько команд, выполнить их в порядке ввода (кроме команд /?, /help и -help)
-k [key value] - CommandParser.exe -k key1 value1 key2 value 2 - выводит на экран таблицу ключ-значение
key1 - value1
key2 - value2
Если задан только ключ, в качестве значение должно быть слово <null>
-ping - издает звуковой сигнал пишет "Pinging …" в консоли
- print <message> - печатает сообщение <message>
-------------------------------------------*/
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing.Printing;
namespace CommandParser
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                bool commandAccepted = false;
                string fullCommand = "";
                for (int i = 0; i < args.Length; i++)
                {
                    fullCommand = fullCommand.Insert(fullCommand.Length, args[i] + " ");                    
                }
                if ((args.Length == 0) || (Array.IndexOf(args, @"/help") >= 0) ||
                    (Array.IndexOf(args, @"-help") >= 0) ||
                    (Array.IndexOf(args, @"/?") >= 0))
                {
                    Console.WriteLine("CommandParser.exe [/?] [/help] [-help] [-k key value] [-ping] [-print <print a value>]");
                    commandAccepted = true;
                }
                if (Array.IndexOf(args, "-k") >= 0)
                {
                    MatchCollection keys = Regex.Matches(fullCommand, @"key\d?\s\w{0,}");// найти "слова" ключ-значение
                    foreach (Match m in keys)
                    {
                        Match mkeyName = Regex.Match(m.Captures[0].Value, @"\w+"); // найти "слово" название ключа
                        Match mkeyValue = Regex.Match(m.Captures[0].Value, @"\s\w{0,}"); // найти "слово" значение ключа
                        string keyValue = "<null>";
                        if (mkeyValue.Captures[0].Value.Trim() != string.Empty)
                            keyValue = mkeyValue.Captures[0].Value.Trim();
                        Console.WriteLine(mkeyName.Captures[0].Value + " - " + keyValue);
                    }
                    commandAccepted = true;
                }
                if (Array.IndexOf(args, "-ping") >= 0)
                {
                    Console.WriteLine("Pinging...");
                    Console.Beep(911, 800);
                    commandAccepted = true;
                }
                if (Array.IndexOf(args, "-print") >= 0)
                {
                    PrintDocument p = new PrintDocument();
                    Match mprintText = Regex.Match(fullCommand, @"-print[\s\S]+$"); // весь текст до конца, после команды -print
                    string s = mprintText.Captures[0].Value.Substring(7);
                    System.Drawing.Font fmt = new System.Drawing.Font("Arial", 10.7f);
                    System.Drawing.SolidBrush b = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    System.Drawing.PointF f = new System.Drawing.PointF();
                    p.PrintPage += delegate (object o1, PrintPageEventArgs e1)
                    {
                        e1.Graphics.DrawString(s, fmt, b, f);
                    };
                    p.Print();
                    commandAccepted = true;
                }
                if (!commandAccepted)
                    Console.WriteLine("Command <" + fullCommand + "> is not supported, use CommandParser.exe /? to see set of allowed commands");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Woops!\nError here:\n" + ex.Message);
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
