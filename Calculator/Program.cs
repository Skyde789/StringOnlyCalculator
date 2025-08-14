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

    void ShowResult(string result)
    {
        Console.WriteLine(result);
    }
}

public enum OperationType
{
    Add,
    Substract,
    
    Divide,
    Multiply,

    Exponent,
}
public class Operation {

    double var1, var2;

    public int operatorIndex;
    public int orderWeight = 0;

    public OperationType type;

    public Operation(int weight, OperationType type, int opIndex)
    {
        this.type = type;
        orderWeight = weight;
        this.operatorIndex = opIndex;
    }

    public void SetVars(double v1, double v2)
    {
        var1 = v1;
        var2 = v2;
    }
    public void SetVars(double[] vars)
    {
        var1 = vars[0];
        var2 = vars[1];
    }
    public double Calculate()
    {
        double result = -1;

        switch (type)
        {
            case OperationType.Exponent:
                result = Math.Pow(var1, var2);
                break;
            case OperationType.Multiply:
                result = var1 * var2;
                break;
            case OperationType.Divide:
                result = var1 / var2;
                break;
            case OperationType.Add:
                result = var1 + var2;
                break;
            case OperationType.Substract:
                result = var1 - var2;
                break;
        }
        return result;
    }
}

public class Calculator
{
    #region Is X
    bool IsSymbol(char c)
    {
        return "()+/-*^.".Contains(c);
    }
    bool IsOperation(char c)
    {
        return "+/-*^".Contains(c);
    }
    bool IsDigit(char c)
    {
        return Char.IsDigit(c);
    }
    #endregion

    class GetOperationVarsData
    {
        public double[] vars;
        public int leftVarStartIndex = -1;
        public int rightVarEndIndex = -1;

        public bool Failed => leftVarStartIndex == -1 && rightVarEndIndex == -1;

        public GetOperationVarsData(double[] v, int lI, int rI)
        {
            vars = v;
            leftVarStartIndex = lI;
            rightVarEndIndex = rI;
        }
    }

    // Trying to find the 2 numbers left and right of the operator (+-*^/)
    // and returning them to the main loop
    GetOperationVarsData GetOperationVars(string input, int operatorIndex)
    {
        Debug.Log($"GetOperationVars: Trying to find vars from operator: {input[operatorIndex]}");
        string[] var = new string[2];

        int leftVarStartIndex = -1;
        int rightVarEndIndex = -1;

        for (int i = 1; i < input.Length; i++)
        {
            int prevIndex = operatorIndex - i;
            int nextIndex = operatorIndex + i;

            if (leftVarStartIndex == -1)
            {
                if (prevIndex >= 0)
                {
                    if (!IsDigit(input[prevIndex]) && input[prevIndex] != '.')
                    {
                        if(input[prevIndex] == '-')
                            leftVarStartIndex = operatorIndex - (i);
                        else
                            leftVarStartIndex = operatorIndex - (i - 1);
                    }   
                }
                else
                    leftVarStartIndex = 0;
            }

            if (rightVarEndIndex == -1)
            {
                if (nextIndex < input.Length)
                {
                    if (!IsDigit(input[nextIndex]) && input[nextIndex] != '.')
                        rightVarEndIndex = operatorIndex + (i - 1);
                }
                else
                    rightVarEndIndex = input.Length - 1;
            }

            if (leftVarStartIndex != -1 && rightVarEndIndex != -1)
                break;
        }

        if (leftVarStartIndex == -1 || rightVarEndIndex == -1)
        {
            Debug.Log("GetOperationVars: Could not find variables", LogType.Error);
            return null;
        }

        var[0] = input.Substring(leftVarStartIndex, operatorIndex - leftVarStartIndex);
        var[1] = input.Substring(operatorIndex + 1, rightVarEndIndex - operatorIndex);

        Debug.Log($"GetOperationVars: Left side: {var[0]} | Right side: {var[1]}");

        double[] parsedVars = new double[2];
        try
        {
            parsedVars[0] = double.Parse(var[0], CultureInfo.InvariantCulture);
            parsedVars[1] = double.Parse(var[1], CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            Debug.Log($"GetOperationVars: Error trying to parse vars into double", LogType.Error);
        }
        
        return new GetOperationVarsData(parsedVars, leftVarStartIndex, rightVarEndIndex);
    }

    int[] GetBracketIndexes(string input)
    {
        Debug.Log($"GetBracketIndexes: Input: {input}");
        int openingBracketIndex = -1;
        int closingBracketIndex = -1;

        for (int i = input.IndexOf('('); i < input.Length; i++)
        {
            if (input[i] == '(')
            {
                Debug.Log($"GetBracketIndexes: Found opening bracket at index {i}");
                openingBracketIndex = i;
            }
                
            if (input[i] == ')')
            {
                Debug.Log($"GetBracketIndexes: Found closing bracket at index {i}, breaking from loop\n");
                closingBracketIndex = i;
                break;
            }
        }

        return new int[] { openingBracketIndex, closingBracketIndex };
    }

    // Is used to verify the basic user error stuff first
    // There still may be errors after this verification passes
    public bool VerifyInput(ref string input)
    {
        Debug.Log($"VerifyInput: Verifying input: {input}");

        if (input.Contains(" "))
        {
            input = input.Replace(" ", "");
            Debug.Log($"VerifyInput: Removed spaces from input: {input}");
        }
        if (input.Contains(","))
        {
            input = input.Replace(",", ".");
            Debug.Log($"VerifyInput: Converted commas to dots {input}");
        }

        bool containsDigits = false;

        int openingBrackets = input.Count(c => c == '(');
        int closingBrackets = input.Count(c => c == ')');

        if (openingBrackets != closingBrackets)
        {
            Debug.Log($"VerifyInput: Incorrect usage of brackets. Opening Bracket Count = {openingBrackets} | Closing Bracket Count = {closingBrackets} ", LogType.Error);
            return false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            int prevIndex = i - 1;
            int nextIndex = i + 1;
            
            if(i == 0)
            {
                if (IsOperation(input[i]) && input[i] != '-')
                {
                    Debug.Log($"VerifyInput: Invalid formula, starting with an operation at index {0}", LogType.Error);
                    return false;
                }
            }

            if (input[i] == '.')
            {
                if (nextIndex < input.Length && input[nextIndex] == '.')
                {
                    Debug.Log($"VerifyInput: Incorrect usage of dots. Found two in a row at {nextIndex}", LogType.Error);
                    return false;
                }
                if (nextIndex < input.Length && IsSymbol(input[nextIndex]))
                {
                    Debug.Log($"VerifyInput: Found an implied 0 at index {i}, starting verification again.");
                    input = input.Insert(nextIndex, "0");
                    return VerifyInput(ref input);
                }
            }

            if (input[i] == '-')
            {
                if (i == 0)
                {
                    Debug.Log($"VerifyInput: Found an implied 0 - X at index {i}, starting verification again.");
                    input = input.Insert(i, "0");
                    return VerifyInput(ref input);
                }
                if (prevIndex >= 0 && input[prevIndex] == '(')
                {
                    Debug.Log($"VerifyInput: Found an implied 0 - X at index {i}, starting verification again.");
                    input = input.Insert(i, "0");
                    return VerifyInput(ref input);
                }
            }

            if (input[i] == '(')
            {
                if (nextIndex < input.Length && input[nextIndex] == ')')
                {
                    Debug.Log($"VerifyInput: Incorrect usage of brackets. Found empty brackets at {nextIndex}", LogType.Error);
                    return false;
                }

                if (prevIndex >= 0 && IsDigit(input[prevIndex]))
                {
                    Debug.Log($"VerifyInput: Found an implied multiplication at index {i}, starting verification again.");
                    input = input.Insert(i, "*");
                    return VerifyInput(ref input);
                }
            }

            if (input[i] == ')' && nextIndex < input.Length)
            {
                if (IsDigit(input[nextIndex]))
                {
                    Debug.Log($"VerifyInput: Incorrect usage of brackets. Closing bracket followed by a number at index {nextIndex}", LogType.Error);
                    return false;
                }
            }

            if(!containsDigits && IsDigit(input[i]))
                containsDigits = true;

            if (!IsDigit(input[i]) && !IsSymbol(input[i]))
            {
                Debug.Log($"VerifyInput: Found an invalid character at position {i}.", LogType.Error);
                return false;
            }
        }

        if (!containsDigits)
        {
            Debug.Log($"VerifyInput: No digits found.", LogType.Error);
            return false;
        }

        Debug.Log($"VerifyInput: Input verified successfully. Input: {input}\n");
        return true;
    }

    public void MainLoop(string input)
    {
        int startIndex = 0;
        int endIndex = input.Length;
        bool hasBrackets = false;

        if (input.Contains('('))
        {
            Debug.Log("MainLoop: Input contains brackets, handle them first");
            int[] bracketIndexes = GetBracketIndexes(input);
            startIndex = bracketIndexes[0];
            endIndex = bracketIndexes[1];
            hasBrackets = true;
        }

        List<Operation> operations = new List<Operation>();

        // Do startIndex+1 to skip the first char, as it is most likely useless
        // as we want to find the operators, and in the case of bracketFound startindex, it is always a bracket
        for (int i = startIndex + 1; i < endIndex; i++)
        {
            if (IsOperation(input[i]))
            {
                switch (input[i])
                {
                    case '^':
                        operations.Add(new Operation(3,OperationType.Exponent, i));
                        break;
                    case '*':
                        operations.Add(new Operation(2,OperationType.Multiply, i));
                        break;
                    case '/':
                        operations.Add(new Operation(2,OperationType.Divide, i));
                        break;
                    case '+':
                        operations.Add(new Operation(1,OperationType.Add, i));
                        break;
                    case '-':
                        operations.Add(new Operation(1,OperationType.Substract, i));
                        break;

                }
            }
        }

        operations = operations
            .OrderByDescending(i => i.orderWeight) 
            .ThenBy(i => i.operatorIndex)             
            .ToList();

        // We still have stuff outside brackets (possibly)
        // and the only stuff left inside the brackets is not needed to calculate
        if (hasBrackets && operations.Count == 0 || hasBrackets && operations.Count == 1 && operations[0].type == OperationType.Substract)
        {
            Debug.Log("MainLoop: Unnecessary brackets detected");
            input = RemoveBrackets(input, startIndex, endIndex);
            MainLoop(input);
            return;
        }

        if(operations.Count != 0)
        {
            GetOperationVarsData operationVarsData = GetOperationVars(input, operations[0].operatorIndex);

            if (operationVarsData.Failed)
            {
                Debug.Log($"MainLoop: Error retrieving operationVarsData, terminating loop.");
                return;
            }
            operations[0].SetVars(operationVarsData.vars);
            string result = operations[0].Calculate().ToString();
            input = ReplaceFormulaWithResult(input, result, operationVarsData.leftVarStartIndex, operationVarsData.rightVarEndIndex);
            MainLoop(input);
        }
        else
        {
            Console.WriteLine("Result: " + input);
        }
        
    }

    string ReplaceFormulaWithResult(string input, string result, int startIndex, int endIndex)
    {
        endIndex++;
        Debug.Log($"ReplaceFormulaWithResult: Input received: {input}");
        int count = endIndex- startIndex;
        input = input.Remove(startIndex,count);
        Debug.Log($"ReplaceFormulaWithResult: Removed formula {input.Insert(startIndex, "[]")}");
        input = input.Insert(startIndex, result);
        Debug.Log($"ReplaceFormulaWithResult: Inserted result: {input}\n");

        return input;
    }

    string RemoveBrackets(string input, int openIndex, int closeIndex)
    {
        Debug.Log($"RemoveBrackets: Input received: {input}");
        input = input.Remove(closeIndex, 1).Remove(openIndex,1);
        Debug.Log($"RemoveBrackets: Brackets from input removed: {input}\n");

        return input;
    }
}

public enum LogType
{
    Default = 1 << 0,
    Error = 1 << 1,
}

public static class Debug
{
    static LogType showLogsFrom = LogType.Default | LogType.Error;
    public static void Log(string message, LogType type = LogType.Default)
    {
        string prefix = "";
        switch (type)
        {
            case LogType.Default:
                prefix = "[DEBUG] ";
                break;
            case LogType.Error:
                prefix = "[DEBUG ERROR] ";
                break;
            default:
                break;
        }

        if ((showLogsFrom & type) != 0)
            Console.WriteLine(prefix + message);
    }
}