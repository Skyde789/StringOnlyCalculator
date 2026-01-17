using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Operation
{

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

