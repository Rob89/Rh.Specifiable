using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Rh.Specifiable.Specifications;

namespace Rh.Specifiable.Expressions
{
    public class SpecificationExpander : ExpressionVisitor
    {
        protected override System.Linq.Expressions.Expression VisitUnary(UnaryExpression node)
        {
            var declaringType = node.Method?.DeclaringType;
            if (declaringType != null
                && declaringType.IsGenericType
                && declaringType.GetGenericTypeDefinition() == typeof(Specification<>))
            {
                if (node.Method.Name == "op_Implicit")
                {
                    var method = typeof(Specification<>).MakeGenericType(declaringType.GetGenericArguments()).GetMethod(nameof(Specification<object>.ToExpression));
                    if (node.Operand.NodeType == ExpressionType.Not)
                    {
                        var operand = ((UnaryExpression)node.Operand).Operand;
                        var expanded = ExpandConversion(Visit(operand), method);
                        if (expanded != null)
                        {
                            var negate = typeof(ExpressionExtensions).GetMethod(nameof(ExpressionExtensions.Negate)).MakeGenericMethod(declaringType.GetGenericArguments());
                            return (System.Linq.Expressions.Expression)negate.Invoke(null, new object[] { expanded }); ;
                        }
                    }
                    else if (node.Operand.NodeType == ExpressionType.Call)
                    {
                        var expression = Visit(node.Operand);
                        var expanded = ExpandConversion(expression, method);
                        if (expanded != null)
                        {
                            return expanded;
                        }
                    }
                    else
                    {
                        var expanded = ExpandConversion(node.Operand, method);
                        if (expanded != null)
                        {
                            return expanded;
                        }
                        throw new InvalidOperationException($"Cannot convert the specification ({node.Type.Name}) to an expression");
                    }
                }
            }
            return base.VisitUnary(node);
        }

        protected override System.Linq.Expressions.Expression VisitMethodCall(MethodCallExpression node)
        {
            var declaringType = node.Method.DeclaringType;
            if (declaringType != null
                && declaringType.IsGenericType
                && declaringType.GetGenericTypeDefinition() == typeof(Specification<>))
            {
                if (node.Method.Name == nameof(Specification<object>.ToExpression))
                {
                    var expanded = ExpandConversion(Visit(node.Object), node.Method);
                    if (expanded != null)
                    {
                        return expanded;
                    }
                }
                else if (node.Method.Name == nameof(Specification<object>.Negate))
                {
                    var value = GetValue((MemberExpression)node.Object);
                    var negate = typeof(Specification<>).MakeGenericType(declaringType.GetGenericArguments()).GetMethod(nameof(Specification<object>.Negate));
                    var specification = negate.Invoke(value, null);
                    return System.Linq.Expressions.Expression.Constant(specification, specification.GetType());
                }
                else
                {
                    throw new InvalidOperationException(
                        $"{node.Method.Name} cannot be used within an expression");
                }
            }
            return base.VisitMethodCall(node);
        }

        private System.Linq.Expressions.Expression ExpandConversion(System.Linq.Expressions.Expression node, MethodInfo method)
        {
            var newExpression = node as NewExpression;
            if (newExpression != null)
            {
                var parameters = GetArgumentValues(newExpression.Arguments);
                var specification = newExpression.Constructor.Invoke(parameters);
                return (System.Linq.Expressions.Expression)method.Invoke(specification, null);
            }
            var memberExpression = node as MemberExpression;
            if (memberExpression != null)
            {
                return (System.Linq.Expressions.Expression)method.Invoke(GetValue(memberExpression), null);
            }
            var constantExpression = node as ConstantExpression;
            if (constantExpression != null)
            {
                return (System.Linq.Expressions.Expression)method.Invoke(constantExpression.Value, null);
            }
            return null;
        }

        private object[] GetArgumentValues(IReadOnlyList<System.Linq.Expressions.Expression> arguments)
        {
            if (arguments?.Count == 0)
            {
                return null;
            }
            var results = new object[arguments.Count];
            for (var i = 0; i < arguments.Count; i++)
            {
                var constant = arguments[i] as ConstantExpression;
                if (constant != null)
                {
                    results[i] = constant.Value;
                    continue;
                }

                var member = arguments[i] as MemberExpression;
                if (member != null)
                {
                    results[i] = GetValue(member);
                    continue;
                }
                throw new InvalidOperationException("Invalid argument to specification constructor.");
                
            }
            return results;
        }

        // http://stackoverflow.com/a/2616980/1402923
        public static object GetValue(System.Linq.Expressions.Expression expression)
        {
            var objectMember = System.Linq.Expressions.Expression.Convert(expression, typeof(object));
            var getterLambda = System.Linq.Expressions.Expression.Lambda<Func<object>>(objectMember);
            return getterLambda.Compile().Invoke();
        }
    }
}
