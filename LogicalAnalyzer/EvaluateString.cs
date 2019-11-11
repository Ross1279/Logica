using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalAnalyzer;

public class EvaluateString
{
    
    public static bool evaluate(string expression, ref Dictionary<string, bool> variables)
    {
        char[] tokens = expression.ToCharArray();

        // Stack for numbers: 'values' 
        Stack<bool> values = new Stack<bool>();

        
        // Stack for Operators: 'ops' 
        Stack<char> ops = new Stack<char>();

        for (int i = 0; i < tokens.Length; i++)
        {
            // Current token is a whitespace, skip it 
            if (tokens[i] == ' ')
            {
                continue;
            }

            // Current token is a variable, push it to stack for numbers 
            if (  !isOperator(tokens[i]) )
            {
                StringBuilder sbuf = new StringBuilder();
                // There may be more than one digits in number 
                while (i < tokens.Length && !isOperator(tokens[i]))
                {
                    sbuf.Append(tokens[i++]);
                }

                variables.TryGetValue(sbuf.ToString(),out bool variable);
                values.Push(variable);
                i--;
            }

            // Current token is an opening brace, push it to 'ops' 
            else if (tokens[i] == '(')
            {
                ops.Push(tokens[i]);
            }

            // Closing brace encountered, solve entire brace 
            else if (tokens[i] == ')')
            {
                while (ops.Peek() != '(')
                {
                    char @operator = ops.Pop();
                    if (@operator == '¬')
                    {
                        values.Push(applyOp(@operator, values.Pop()));
                    }
                    else
                    {
                        values.Push(applyOp(@operator, values.Pop(), values.Pop()));
                    }
                }
                ops.Pop();
            }

            // Current token is an operator. 
            else if (tokens[i] == '¬' || tokens[i] == '&' || tokens[i] == '|' || tokens[i] == '>' || tokens[i] == '=')
            {
                // While top of 'ops' has same or greater precedence to current 
                // token, which is an operator. Apply operator on top of 'ops' 
                // to top two elements in values stack 
                while (ops.Count > 0 && hasPrecedence(tokens[i], ops.Peek()))
                {
                    char @operator = ops.Pop();
                    if (@operator == '¬')
                    {
                        values.Push(applyOp(@operator, values.Pop()));
                    }
                    else
                    {
                        values.Push(applyOp(@operator, values.Pop(), values.Pop()));
                    }
                }

                // Push current token to 'ops'. 
                ops.Push(tokens[i]);
            }
        }

        // Entire expression has been parsed at this point, apply remaining 
        // ops to remaining values 
        while (ops.Count > 0)
        {
            char @operator = ops.Pop();
            if (@operator == '¬')
            {
                values.Push(applyOp(@operator, values.Pop()));
            }
            else
            {
                values.Push(applyOp(@operator, values.Pop(), values.Pop()));
            }
        }

        // Top of 'values' contains result, return it 
        return values.Pop();
        // This code is contributed by Shrikant13 
    }

    internal static HashSet<Char> GetVariables(Expressions exp)
    {
        exp.Left = exp.Left.Replace(',', '&');
        exp.Right = exp.Right.Replace(',', '&');
        HashSet<Char> variables = GetVariablesFromString(exp.Left);
        variables = variables.Union(GetVariablesFromString(exp.Right)).ToHashSet();
        return variables;
    }

    internal static HashSet<Char> GetVariablesFromString(String toExtract)
    {
        HashSet<Char> variables = new HashSet<char>();
        for (int i = 0; i < toExtract.Length; i++)
        {
            if (!isOperator(toExtract[i]))
            {
                variables.Add(toExtract[i]);
            }
        }
        return variables;
    }

    public static bool isOperator(char symbol)
    {
        switch ( symbol)
        {
            case '(':
            case ')':
            case '¬':
            case '&':
            case '|':
            case '>':
            case '=':
                return true;
            default:
                return false;
        }
    }

    // Returns true if 'op2' has higher or same precedence as 'op1', 
    // otherwise returns false. 
    public static bool hasPrecedence(char op1, char op2)
    {
        if (op2 == '(' || op2 == ')')
        {
            return false;
        }
        if ((op1 == '&' || op1 == '|') && (op2 == '¬' || op2 == '>' || op2 == '='))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // A utility method to apply an operator 'op' on operands 'a' 
    // and 'b'. Return the result. 
    public static bool applyOp(char op, bool b, bool a)
    {
        switch (op)
        {
            case '&':
                return a && b;
            case '|':
                return a || b;
            case '>':
                return implication(a, b); // Implies
            case '=':
                return biconditional(a, b); // Double implies

        }
        return false;
    }

    private static bool implication(bool a, bool b)
    {
        if ( a && !b ) return false;
        return true;
    }

    private static bool biconditional(bool a, bool b)
    {
        if (a && b) return true;
        if (!a && !b) return true;
        return false;
    }

    public static bool applyOp(char op, bool a)
    {
        switch (op)
        {
            case '¬':
                return !a;
        }
        return false;
    }

    

    
}


