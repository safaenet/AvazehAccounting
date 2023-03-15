using System;
using System.Collections.Generic;
using System.Globalization;
using static DotNetStandardCalculator.DotNetStandardCalculatorConstants;

namespace DotNetStandardCalculator
{
    public static class StandardCalculator
    {
        public static double CalculateFromString(string calculateFrom)
        {
            try
            {
                var beginSign = "";
                if (calculateFrom.StartsWith(OPERATOR_ADD)) calculateFrom = calculateFrom.Remove(0, 1);
                else if (calculateFrom.StartsWith(OPERATOR_SUBTRACT))
                {
                    beginSign = OPERATOR_SUBTRACT;
                    calculateFrom = calculateFrom.Remove(0, 1);
                }

                var rpn = PrepareString(calculateFrom);

                var result = Calculate(rpn);
                if (beginSign == OPERATOR_SUBTRACT) result *= -1;
                return result;
            }
            catch { return 0; }
        }

        private static string[] PrepareString(string calculateFrom)
        {
            var split = Utilities.Split(calculateFrom);
            return ShuntingYard.GetRPNAsArrayFromString(string.Join(" ", split));
        }

        public static bool IsCalculatable(string calculateFrom)
        {
            try
            {
                if (calculateFrom.StartsWith(OPERATOR_ADD) || calculateFrom.StartsWith(OPERATOR_SUBTRACT)) calculateFrom = calculateFrom.Remove(0, 1);
                var rpn = PrepareString(calculateFrom);

                if (rpn.Length == 0) return false;
                if (rpn.Length == 1) return double.TryParse(rpn[0], NumberStyles.Any, CultureInfo.InvariantCulture, out _);
                foreach (var token in rpn) if (!double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _) && token != OPERATOR_POW && token != OPERATOR_MULTIPLY && token != OPERATOR_ADD && token != OPERATOR_DIVIDE && token != OPERATOR_SUBTRACT) return false;
                return true;
            }
            catch { return false; }
        }

        public static double Calculate(string[] rpnSum)
        {
            if (rpnSum.Length == 0)
                throw new Exception("");

            if (rpnSum.Length == 1)
            {
                if (double.TryParse(rpnSum[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
                {
                    return parsedValue;
                }
                else
                {
                    throw new Exception("Welp");
                }
            }

            var stack = new Stack<double>();
            foreach(var token in rpnSum)
            {
                var workingNumber = 0D;
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed))
                    stack.Push(parsed);
                else
                {
                    switch(token)
                    {
                        case OPERATOR_POW:
                            workingNumber = stack.Pop();
                            stack.Push(Math.Pow(stack.Pop(), workingNumber));
                            break;
                        case OPERATOR_MULTIPLY:
                            stack.Push(stack.Pop() * stack.Pop());
                            break;
                        case OPERATOR_ADD:
                            stack.Push(stack.Pop() + stack.Pop());
                            break;
                        case OPERATOR_DIVIDE:
                            workingNumber = stack.Pop();
                            stack.Push(stack.Pop() / workingNumber);
                            break;
                        case OPERATOR_SUBTRACT:
                            workingNumber = stack.Pop();
                            stack.Push(stack.Pop() - workingNumber);
                            break;
                        default:
                            throw new Exception("Unknown operator");
                    }
                }

            }

            return stack.Pop();
        }
    }
}
