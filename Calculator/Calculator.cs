using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum OperationType
{
    Add,
    Substract,

    Divide,
    Multiply,

    Exponent,
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

        public bool Failed => leftVarStartIndex == -1 || rightVarEndIndex == -1;

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
                        if (input[prevIndex] == '-')
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
                    {
                        if (input[nextIndex] != '-')
                            rightVarEndIndex = operatorIndex + (i - 1);
                    }
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

            if (i == 0)
            {
                if (IsOperation(input[i]) && input[i] != '-')
                {
                    Debug.Log($"VerifyInput: Invalid formula, starting with an operation at index {0}", LogType.Error);
                    return false;
                }
            }
            else if (input[i] == '.')
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
            else if (input[i] == '(')
            {
                if (nextIndex < input.Length && input[nextIndex] == ')')
                {
                    Debug.Log($"VerifyInput: Incorrect usage of brackets. Found empty brackets at {nextIndex}", LogType.Error);
                    return false;
                }

                if (prevIndex >= 0 && (IsDigit(input[prevIndex]) || input[prevIndex] == ')'))
                {
                    Debug.Log($"VerifyInput: Found an implied multiplication at index {i}, starting verification again.");
                    input = input.Insert(i, "*");
                    return VerifyInput(ref input);
                }
            }
            else if (input[i] == ')' && nextIndex < input.Length)
            {
                if (IsDigit(input[nextIndex]))
                {
                    Debug.Log($"VerifyInput: Incorrect usage of brackets. Closing bracket followed by a number at index {nextIndex}", LogType.Error);
                    return false;
                }
            }

            if (!containsDigits && IsDigit(input[i]))
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
        bool negativeFailsafe = false;
        if (input.Contains('('))
        {
            Debug.Log("MainLoop: Input contains brackets, handle them first");
            int[] bracketIndexes = GetBracketIndexes(input);

            startIndex = bracketIndexes[0];
            endIndex = bracketIndexes[1];
            hasBrackets = true;

            // Check if we need to start our operation search further
            if (input[startIndex + 1] == '-')
                negativeFailsafe = true;
        }

        List<Operation> operations = new List<Operation>();

        //We do +1 to start at numbers and possibly +2 to skip the - negative number sign not to be confused with the operation -
        for (int i = startIndex + (negativeFailsafe ? 2 : 1); i < endIndex; i++)
        {
            if (IsOperation(input[i]))
            {
                switch (input[i])
                {
                    case '^':
                        operations.Add(new Operation(3, OperationType.Exponent, i));
                        break;
                    case '*':
                        operations.Add(new Operation(2, OperationType.Multiply, i));
                        break;
                    case '/':
                        operations.Add(new Operation(2, OperationType.Divide, i));
                        break;
                    case '+':
                        operations.Add(new Operation(1, OperationType.Add, i));
                        break;
                    case '-':
                        operations.Add(new Operation(1, OperationType.Substract, i));
                        break;

                }
            }
        }

        operations = operations
            .OrderByDescending(i => i.orderWeight)
            .ThenBy(i => i.operatorIndex)
            .ToList();

        // We have brackets and we couldnt find any operations in them
        if (hasBrackets && operations.Count == 0)
        {
            Debug.Log("MainLoop: Unnecessary brackets detected");
            input = RemoveBrackets(input, startIndex, endIndex);
            MainLoop(input);
            return;
        }

        if (operations.Count != 0)
        {
            GetOperationVarsData operationVarsData = GetOperationVars(input, operations[0].operatorIndex);

            if (operationVarsData.Failed)
            {
                Debug.Log($"MainLoop: Error retrieving operationVarsData, terminating loop.");
                return;
            }
            operations[0].SetVars(operationVarsData.vars);

            string result = operations[0].Calculate().ToString();

            if (result.Contains(","))
                result = result.Replace(",", ".");

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
        int count = endIndex - startIndex;
        input = input.Remove(startIndex, count);
        Debug.Log($"ReplaceFormulaWithResult: Removed formula {input.Insert(startIndex, "[]")}");
        input = input.Insert(startIndex, result);
        Debug.Log($"ReplaceFormulaWithResult: Inserted result: {input}\n");

        return input;
    }

    string RemoveBrackets(string input, int openIndex, int closeIndex)
    {
        Debug.Log($"RemoveBrackets: Input received: {input}");
        input = input.Remove(closeIndex, 1).Remove(openIndex, 1);
        Debug.Log($"RemoveBrackets: Brackets from input removed: {input}\n");

        return input;
    }
}