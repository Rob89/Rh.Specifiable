﻿using System.Linq;
using System.Linq.Expressions;

namespace Rh.Specifiable.Expression
{
    // Taken/modified from:
    // https://blogs.msdn.microsoft.com/meek/2008/12/07/invocationexpression-and-linq-to-entities/
    public class InvocationExpander : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;
        private readonly System.Linq.Expressions.Expression _expansion;
        private readonly InvocationExpander _previous;

        public InvocationExpander() { }

        private InvocationExpander(ParameterExpression parameter, System.Linq.Expressions.Expression expansion, InvocationExpander previous)
        {
            _parameter = parameter;
            _expansion = expansion;
            _previous = previous;
        }

        public InvocationExpander Push(ParameterExpression parameter, System.Linq.Expressions.Expression expansion)
        {
            return new InvocationExpander(parameter, expansion, this);
        }

        protected override System.Linq.Expressions.Expression VisitInvocation(InvocationExpression iv)
        {
            if (iv.Expression.NodeType != ExpressionType.Lambda)
            {
                return base.VisitInvocation(iv);
            }
            var lambda = (LambdaExpression)iv.Expression;
            return lambda
                .Parameters
                .Select((x, i) => new { Parameter = x, Expansion = iv.Arguments[i] })
                .Aggregate(this, (previous, pair) => previous.Push(pair.Parameter, pair.Expansion))
                .Visit(lambda.Body);
        }

        protected override System.Linq.Expressions.Expression VisitParameter(ParameterExpression p)
        {
            var expander = this;
            while (null != expander)
            {
                if (expander._parameter == p)
                {
                    return base.Visit(expander._expansion);
                }
                expander = expander._previous;
            }
            return base.VisitParameter(p);
        }

    }
}
