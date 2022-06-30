using System;

namespace Chapter_0001
{
    class Program
    {
        static void Main(string[] args)
        {
            string number;
            int x1, x2;

            do
            {
                Console.WriteLine("請輸入一個數字。");
                number = Console.ReadLine();
            } while (!Int32.TryParse(number, out x1));

            do
            {
                Console.WriteLine("請輸入另一個數字。");
                number = Console.ReadLine();
            } while (!Int32.TryParse(number, out x2));

            var result = x1 * x2;
            Console.WriteLine(result);
            CalculateCharCount();


        }
        static void CalculateCharCount()
        {
            Console.WriteLine("請輸入一行文字：");
            string? line = Console.ReadLine();

            if (line != null)
            {
                int charCount = line.Length;
                Console.WriteLine($"{charCount} 是文字的長度。");
            }
            else
            {
                Console.WriteLine("您沒有輸入任何文字。");
            }
        }
    }
}