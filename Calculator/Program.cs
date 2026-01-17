using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Calculator calc = new Calculator();
        string input;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to string only calculator!\nExample input: ((8 + 5) *6)/2\n\n");
            input = Console.ReadLine();
            Console.WriteLine();
            if (calc.VerifyInput(ref input))
            {
                calc.MainLoop(input);
            }
            else
            {
                Console.WriteLine("\nInvalid input, try again!");
            }
            Console.WriteLine("\n\nPress enter to continue...");
            Console.ReadLine();
        }
    }
}
