using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter your calculation. Example: ((3 ^ 2 + 3) / 3) - 2 * 3");
                string input = Console.ReadLine();

                if (input == "cls")
                {
                    Console.Clear();
                }

                while(input.Contains(' '))
                {
                    input = StringCalculation(input);
                }

                if (input != "cls")
                {
                    Console.WriteLine("\nRESULT = " + input);
                }
            }
        }

        // OK so this will calculate a formula that is given in string form
        static string StringCalculation(string input)
        {
            string output = input;
            string debug = "";
            
            if (!input.Contains('('))
            {
                input = ForceBrackets(input);
            }

            if (input.Contains('('))
            {
                Console.WriteLine("\n[DEBUG - REMOVEBRACKETS]");
                debug += "-    INPUT = " + input;

                // formula is used for calculating the numbers inside the brackets
                // And test formula is just used if we have to force brackets inside brackets
                // for example (3 * 3 / 2) -> ((3 * 3) / 2)
                string formula;
                string testFormula;

                int startBracket = -1;
                int closingBracket = -1;

                // Find the last case of starting brackets
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i].Equals('('))
                    {
                        debug += "\n-    WE FOUND A STARTING BRACKET AT INDEX " + i;
                        startBracket = i;
                    }
                }

                // Find the first case of closing brackets
                // from after the startBracket index
                for (int i = 0; i < input.Length - startBracket; i++)
                {
                    if (input[startBracket + i].Equals(')'))
                    {
                        debug += "\n-    WE FOUND A CLOSING BRACKET AT INDEX " + (startBracket + i);
                        closingBracket = startBracket + i;
                        break;
                    }
                }

                // string shenanigans, formula will always return a ("number" + "operator" + "number")
                // Also do testFormula FIRST to check if we have to force brackets or not
                testFormula = input.Remove(0, startBracket + 1);
                testFormula = testFormula.Remove(closingBracket - (startBracket + 1));

                if (testFormula.Split(' ').Length > 3)
                {
                    debug = "";
                    input = ForceBrackets(input);
                    debug += "-   INPUT = " + input;
                    // Find the last case of starting brackets
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (input[i].Equals('('))
                        {
                            debug += "\n-    WE FOUND A STARTING BRACKET AT INDEX " + i;
                            startBracket = i;
                        }
                    }
                    // Find the first case of closing brackets
                    // from after the startBracket index
                    for (int i = 0; i < input.Length - startBracket; i++)
                    {
                        if (input[startBracket + i].Equals(')'))
                        {
                            debug += "\n-    WE FOUND A CLOSING BRACKET AT INDEX " + (startBracket + i);
                            closingBracket = startBracket + i;
                            break;
                        }
                    }

                }

                // Remove the starting and closing brackets for output
                output = input.Remove(startBracket, 1);
                debug += "\n-    REMOVING STARTING BRACKET: Input = " + output;

                output = output.Remove(closingBracket - 1, 1);
                debug += "\n-    REMOVING CLOSING BRACKET: Input = " + output;

                // Remove everything except the formula ("number" + "operator" + "number")
                formula = input.Remove(0, startBracket + 1);
                formula = formula.Remove(closingBracket - (startBracket + 1));
                debug += "\n-    FORMULA = " + formula;

                // Now replace the formula from output with the actual number
                output = output.Replace(formula, Calculate(formula));

                Console.WriteLine(debug);
                return output;
            }

            return output;
        }

        #region Calculations

        // This receives only strings like "3 + 3" or "6 * 2"
        static string Calculate(string input)
        {
            string output = input;

            if (input.Contains('+'))
            {
                output = Addition(input);
            }
            if (input.Contains('-'))
            {
                output = Substraction(input);
            }
            if (input.Contains('/'))
            {
                output = Division(input);
            }
            if (input.Contains('*'))
            {
                output = Multiplication(input);
            }
            if (input.Contains('^'))
            {
                output = Exponentiation(input);
            }

            return output;
        }

        static string Division(string input)
        {
            double[] nums = GetNumbers(input);

            return Math.Round(nums[0] / nums[1], 2).ToString();
        }

        static string Multiplication(string input)
        {
            double[] nums = GetNumbers(input);
            
            return Math.Round(nums[0] * nums[1],2).ToString();
        }

        static string Exponentiation(string input)
        {
            double[] nums = GetNumbers(input);

            return Math.Round(Math.Pow(nums[0], nums[1]), 2).ToString();
        }


        static string Addition(string input)
        {
            double[] nums = GetNumbers(input);
            return (nums[0] + nums[1]).ToString();
        }

        static string Substraction(string input)
        {
            double[] nums = GetNumbers(input);
            return (nums[0] - nums[1]).ToString();
        }

        // This receives only strings like "3 + 3" or "6 * 2"
        static double[] GetNumbers(string input)
        {
            List<string> numbers = input.Split(' ').ToList();

            double[] output = { Convert.ToDouble(numbers[0]), Convert.ToDouble(numbers[2]) };

            return output;
        }

        #endregion

        // LMAO this is such a hacky solution but hey, I mean it fucking works XD
        // This calculator worked really well with only brackets, 
        // so the I figured the most low IQ solution is to
        // just make every calculation in brackets (in the right order ofc)
        static string ForceBrackets(string input)
        {
            // Make our input into a list
            List<string> hacky = input.Split(' ').ToList();

            // Trust me we need this even though it's dumb
            bool doBreak = false;

            if (input.Contains('^'))
            {
                for (int i = 0; i < hacky.Count; i++)
                {
                    if (doBreak)
                    {
                        break;
                    }
                    // When we find the FIRST correct operator, force brackets for the numbers
                    if (hacky[i].Contains('^'))
                    {
                        hacky[i - 1] = "(" + hacky[i - 1];
                        hacky[i + 1] = hacky[i + 1] + ")";
                        doBreak = true;
                    }
                }
            }

            if (input.Contains('/') || input.Contains('*'))
            {
                for (int i = 0; i < hacky.Count; i++)
                {
                    if (doBreak)
                    {
                        break;
                    }
                    // When we find the FIRST correct operator, force brackets for the numbers
                    if (hacky[i].Contains('/') || hacky[i].Contains('*'))
                    {
                        hacky[i - 1] = "(" + hacky[i - 1];
                        hacky[i + 1] = hacky[i + 1] + ")";
                        doBreak = true;
                    }
                }
            }

            if (input.Contains('+') || input.Contains('-'))
            {
                for (int i = 0; i < hacky.Count; i++)
                {
                    // This is why we need it
                    if (doBreak)
                    {
                        break;
                    }
                    // When we find the FIRST correct operator, force brackets for the numbers
                    if (hacky[i].Contains('+') || hacky[i].Contains('-'))
                    {
                        hacky[i - 1] = "(" + hacky[i - 1];
                        hacky[i + 1] = hacky[i + 1] + ")";
                        doBreak = true;
                    }
                }
            }

            // Now we're just making input again with the changes XDDDD
            // FML man
            input = "";

            for (int i = 0; i < hacky.Count; i++)
            {
                if (i == 0)
                {
                    input += hacky[i];
                }
                else
                {
                    input += " " + hacky[i];
                }
            }
            return input;
        }
    }

