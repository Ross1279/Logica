using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalAnalyzer
{
    class Program
    {
        static Dictionary<string, bool> variables = new Dictionary<string, bool>();

        public static void Main(string[] args)
        {
            List<Expressions> expressions = new List<Expressions>();
            expressions.Add(new Expressions("((¬q)>(¬r))&((¬r)>(¬p))&((¬p)>(¬q))", "q=r"));
            expressions.Add(new Expressions("((¬p)&(¬q))&((¬p)&(¬r))&((s&t)>p)", "(¬s)|(¬t)"));
            expressions.Add(new Expressions("(p&q)|(r&(¬s))&(s>(¬(p&t)))", "s>(¬t)"));
            expressions.Add(new Expressions("p", "q"));

            // Primero, extraemos las variables de las expresiones
            HashSet<Char> AllVariables = new HashSet<char>();
            foreach (Expressions exp in expressions)
            {
                HashSet<Char> variables = EvaluateString.GetVariables(exp);
                AllVariables = Enumerable.Union(AllVariables, variables).ToHashSet();
            }

            // Ordenamos las variables y las preparamos para imprimir la tabla de verdad
            List<Char> orderedVariables = AllVariables.ToList();
            orderedVariables.Sort();
            string header = "";
            string separator = "";
            foreach (Char variable in orderedVariables)
            {
                header = $"{header}{separator}{variable}";
                separator = ",";
            }
            header = $"{header} |";

            // A las variables se le concatenan las expresiones para el encabezado
            int ctr = 1;
            foreach (Expressions exp in expressions)
            {
                header = $"{header}E{ctr}L|E{ctr}R|";
                ctr++;
            }
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));

            // Se obtiene el numero de ciclos para la tabla de verdad
            int cycles = TwoPowX(orderedVariables.Count);
            
            for (int i = 0; i < cycles; i++)
            {
                ctr = 0;
                foreach (Char variable in orderedVariables)
                {
                    // Asignacion del valor de las variables
                    variables[variable.ToString()] = (i & TwoPowX(ctr)) == TwoPowX(ctr);
                    ctr++;
                }

                string lineToPrint = "";
                separator = "";
                foreach (Char variable in orderedVariables)
                {
                    // Conversion del valor de las variables en 1 (True) o 0 (Falso)
                    int value = variables[variable.ToString()] ? 1 : 0;
                    lineToPrint = $"{lineToPrint}{separator}{value}";
                    separator = ",";
                }

                Console.Write($"{lineToPrint} |");

                bool resultLeft;
                bool resultRigth;

                foreach (Expressions exp in expressions)
                {
                    // Para cada expresion se evalua izquierda y derecha
                    resultLeft = EvaluateString.evaluate(exp.Left, ref variables);
                    int int_left = resultLeft ? 1 : 0;
                    Console.Write($"{int_left}   ");
                    resultRigth = EvaluateString.evaluate(exp.Right, ref variables);
                    int int_right = resultRigth ? 1 : 0;
                    Console.Write($"{int_right}   ");
                    // Se revisa si es consecuencia, basta con que una falle para no considerarla consecuencia
                    exp.IsConsequence(resultLeft, resultRigth);
                }

                Console.Write("\n");
            }

            ctr = 1;
            foreach (Expressions exp in expressions)
            {
                Console.WriteLine($"E{ctr}L is: {exp.Left}");
                Console.WriteLine($"E{ctr}R is: {exp.Right}");
                ctr++;
            }

            // Impresion de resultados finales
            foreach (Expressions exp in expressions)
            {
                if (exp.Result()) Console.WriteLine(exp.Right + " IS a consequence of " + exp.Left);
                else Console.WriteLine(exp.Right + " is NOT a consequence of " + exp.Left);
            }

            Console.ReadLine();
        }

        // Funcion auxiliar para no escribir la funcion Pow y el cast varias veces
        private static int TwoPowX(int X)
        {
            return (int)Math.Pow(2, (X));
        }

    }
}
