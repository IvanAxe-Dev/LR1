﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class CalculatorVisitor : CalculatorBaseVisitor<double>
    {
        public static Dictionary<string, double> tableIdentifier = new Dictionary<string, double>();

        public override double VisitCompileUnit(CalculatorParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitNumberExpr(CalculatorParser.NumberExprContext context)
        {
            var result = double.Parse(context.GetText());
            Debug.WriteLine(result);

            return result;
        }

        //IdentifierExpr
        public override double VisitIdentifierExpr(CalculatorParser.IdentifierExprContext context)
        {
            var result = context.GetText();
            double value;
            //видобути значення змінної з таблиці
            if (tableIdentifier.TryGetValue(result.ToString(), out value))
            {
                return value;
            }
            else
            {
                return 0.0;
            }
        }

        public override double VisitParenthesizedExpr(CalculatorParser.ParenthesizedExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (context.operatorToken.Type == CalculatorLexer.MOD)
            {
                Debug.WriteLine("MOD({0}, {1})", left, right);
                return left % right;
            }
            else
            { // DIV
                Debug.WriteLine("DIV({0}, {1})", left, right);
                return double.Parse((left / right).ToString("F3"));
            }
        }
        public override double VisitParenthesizedExprOneArg(CalculatorParser.ParenthesizedExprOneArgContext context)
        {
            var left = WalkLeft(context);

            if (context.operatorToken.Type == CalculatorLexer.INC)
            {
                //Debug.WriteLine("inc({0})", context);
                return left + 1;
            }
            else // DEC
            {
                //Debug.WriteLine("dec({0})", context);
                return left - 1;
            }
        }
        public override double VisitExponentialExpr(CalculatorParser.ExponentialExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("{0} ^ {1}", left, right);
            return System.Math.Pow(left, right);
        }

        public override double VisitAdditiveExpr(CalculatorParser.AdditiveExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == CalculatorLexer.ADD)
            {
                Debug.WriteLine("{0} + {1}", left, right);
                return left + right;
            }
            else //LabCalculatorLexer.SUBTRACT
            {
                Debug.WriteLine("{0} - {1}", left, right);
                return left - right;
            }
        }

        public override double VisitMultiplicativeExpr(CalculatorParser.MultiplicativeExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);



            if (context.operatorToken.Type == CalculatorLexer.MULTIPLY)
            {
                Debug.WriteLine("{0} * {1}", left, right);
                return double.Parse((left * right).ToString("F10"));
            }
            else //LabCalculatorLexer.DIVIDE
            {
                Debug.WriteLine("{0} / {1}", left, right);
                return double.Parse((left / right).ToString("F10"));
            }
        }

        private double WalkLeft(CalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<CalculatorParser.ExpressionContext>(0));
        }

        private double WalkRight(CalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<CalculatorParser.ExpressionContext>(1));
        }
    }
}

