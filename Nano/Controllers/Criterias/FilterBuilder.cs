using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nano.Controllers.Criterias.Enums;
using Nano.Controllers.Criterias.Interfaces;

namespace Nano.Controllers.Criterias
{
    /// <summary>
    /// Filter Builder
    /// </summary>
	internal class FilterBuilder
    {
        private readonly Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>
        {
            { "Trim", typeof(string).GetMethod("Trim", new Type[0]) },
            { "ToLower", typeof(string).GetMethod("ToLower", new Type[0]) },
            { "Contains", typeof(string).GetMethod("Contains") },
            { "EndsWith", typeof(string).GetMethod("EndsWith", new[] { typeof(string) }) },
            { "StartsWith", typeof(string).GetMethod("StartsWith", new[] { typeof(string) }) }
        };

	    //private readonly MethodInfo trimMethod = typeof(string).GetMethod("Trim", new Type[0]);
	    //private readonly MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", new Type[0]);
     //   private readonly MethodInfo stringContainsMethod = typeof(string).GetMethod("Contains");
	    //private readonly MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
	    //private readonly MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
	    private readonly Dictionary<Operation, Func<Expression, Expression, Expression, Expression>> expressions;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal FilterBuilder()
	    {
            this.expressions = new Dictionary<Operation, Func<Expression, Expression, Expression, Expression>>
            {
                { Operation.Equal, (member, constant, constant2) => Expression.Equal(member, constant) },
                { Operation.NotEqual, (member, constant, constant2) => Expression.NotEqual(member, constant) },
                { Operation.GreaterThan, (member, constant, constant2) => Expression.GreaterThan(member, constant) },
                { Operation.GreaterThanOrEqualTo, (member, constant, constant2) => Expression.GreaterThanOrEqual(member, constant) },
                { Operation.LessThan, (member, constant, constant2) => Expression.LessThan(member, constant) },
                { Operation.LessThanOrEqualTo, (member, constant, constant2) => Expression.LessThanOrEqual(member, constant) },
                { Operation.Contains, (member, constant, constant2) => this.GetContainsExpression(member, constant) },
                { Operation.StartsWith, (member, constant, constant2) => Expression.Call(member,  this.methods["StartsWith"], constant) },
                { Operation.EndsWith, (member, constant, constant2) => Expression.Call(member,  this.methods["EndsWith"], constant) },
                { Operation.Between, this.GetBetweenExpression },
                { Operation.In, (member, constant, constant2) => this.GetContainsExpression(member, constant) },
                { Operation.IsNull, (member, constant, constant2) => Expression.Equal(member, Expression.Constant(null)) },
                { Operation.IsNotNull, (member, constant, constant2) => Expression.NotEqual(member, Expression.Constant(null)) },
                { Operation.IsEmpty, (member, constant, constant2) => Expression.Equal(member, Expression.Constant(string.Empty)) },
                { Operation.IsNotEmpty, (member, constant, constant2) => Expression.NotEqual(member, Expression.Constant(string.Empty)) },
                { Operation.IsNullOrWhiteSpace, (member, constant, constant2) => this.GetIsNullOrWhiteSpaceExpression(member) },
                { Operation.IsNotNullNorWhiteSpace, (member, constant, constant2) => this.GetIsNotNullNorWhiteSpaceExpression(member) }
            };
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <typeparam name="T">Type used in the <see cref="Expression{TDelegate}"/>.</typeparam>
        /// <param name="filter">The <see cref="Filter"/>.</param>
        /// <returns>The <see cref="Expression{TDelegate}"/></returns>
		public Expression<Func<T, bool>> GetExpression<T>(Filter filter) 
            where T : class
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var logical = OperationLogical.And;
            var parameter = Expression.Parameter(typeof(T), "x");

            Expression expression = null;

            foreach (var statement in filter.Statements)
            {
                var expr = statement.Property.Contains("[") && statement.Property.Contains("]")
                    ? GetArrayExpression(parameter, statement) 
                    : GetExpression(parameter, statement);

                expression = expression == null ? expr : this.GetCombinedExpression(expression, expr, logical);
                logical = statement.OperationLogical;
            }

            return Expression.Lambda<Func<T, bool>>(expression ?? Expression.Constant(true), parameter);
        }

	    private Expression GetExpression(Expression parameter, IFilterStatement statement, string propertyName = null)
	    {
	        if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

	        var memberName = propertyName ?? statement.Property;
	        var member = this.GetMemberExpression(parameter, memberName);
	        var constant = this.GetConstantExpression(statement.Value);
	        var constant2 = this.GetConstantExpression(statement.Value2);

	        Expression expression = null;
            if (Nullable.GetUnderlyingType(member.Type) != null && statement.Value != null)
	        {
	            member = Expression.Property(member, "Value");
	            expression = Expression.Property(member, "HasValue");
	        }

	        var stringExpression = this.GetStringExpression(member, statement.Operation, constant, constant2);
	        expression = expression != null ? Expression.AndAlso(expression, stringExpression) : stringExpression;

	        if (memberName.Contains("."))
	        {
	            var parentName = memberName.Substring(0, memberName.IndexOf(".", StringComparison.Ordinal));
	            var parentMember = this.GetMemberExpression(parameter, parentName);

                expression = statement.Operation == Operation.IsNull || statement.Operation == Operation.IsNullOrWhiteSpace 
                    ? Expression.OrElse(Expression.Equal(parentMember, Expression.Constant(null)), expression)
                    : Expression.AndAlso(Expression.NotEqual(parentMember, Expression.Constant(null)), expression);
            }

	        return expression;
	    }
	    private Expression GetMemberExpression(Expression parameter, string propertyName)
	    {
	        if (parameter == null)
	            throw new ArgumentNullException(nameof(parameter));

	        if (propertyName == null)
	            throw new ArgumentNullException(nameof(propertyName));

	        while (true)
	        {
	            if (propertyName.Contains("."))
	            {
	                var index = propertyName.IndexOf(".", StringComparison.Ordinal);
	                var subParam = Expression.Property(parameter, propertyName.Substring(0, index));

	                parameter = subParam;
	                propertyName = propertyName.Substring(index + 1);

	                continue;
	            }

	            return Expression.Property(parameter, propertyName);
	        }
	    }
        private Expression GetConstantExpression(object value)
        {
            switch (value)
            {
                case null:
                    return null;

                case string _:
                    return Expression.Call(Expression.Call(Expression.Constant(value), this.methods["Trim"]), this.methods["ToLower"]);
            }

            return Expression.Constant(value);
        }
        private Expression GetIsNullOrWhiteSpaceExpression(Expression member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return Expression.OrElse(
	            Expression.Equal(member, Expression.Constant(null)),
	            Expression.Equal(Expression.Call(member, this.methods["Trim"]), Expression.Constant(string.Empty)));
        }
	    private Expression GetIsNotNullNorWhiteSpaceExpression(Expression member)
	    {
	        if (member == null)
	            throw new ArgumentNullException(nameof(member));

            return Expression.AndAlso(
	            Expression.NotEqual(member, Expression.Constant(null)),
	            Expression.NotEqual(Expression.Call(member, this.methods["Trim"]), Expression.Constant(string.Empty)));
	    }
	    private Expression GetContainsExpression(Expression member, Expression expression)
	    {
	        if (member == null)
	            throw new ArgumentNullException(nameof(member));

	        if (expression == null)
	            throw new ArgumentNullException(nameof(expression));

	        MethodCallExpression contains = null;

	        if (expression is ConstantExpression constant && constant.Value is IList && constant.Value.GetType().IsGenericType)
	        {
	            var type = constant.Value.GetType();
	            var containsInfo = type.GetMethod("Contains", new[] { type.GetGenericArguments()[0] });
	            contains = Expression.Call(constant, containsInfo, member);
	        }

	        return contains ?? Expression.Call(member, this.methods["Contains"], expression);
	    }
        private Expression GetBetweenExpression(Expression member, Expression constant, Expression constant2)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (constant == null)
                throw new ArgumentNullException(nameof(constant));

            if (constant2 == null)
                throw new ArgumentNullException(nameof(constant2));

            var value = expressions[Operation.GreaterThanOrEqualTo].Invoke(member, constant, null);
            var value2 = expressions[Operation.LessThanOrEqualTo].Invoke(member, constant2, null);

            return this.GetCombinedExpression(value, value2, OperationLogical.And);
        }
	    private Expression GetCombinedExpression(Expression expression1, Expression expression2, OperationLogical connector)
	    {
	        if (expression1 == null)
	            throw new ArgumentNullException(nameof(expression1));

	        if (expression2 == null)
	            throw new ArgumentNullException(nameof(expression2));

            return connector == OperationLogical.And 
                ? Expression.AndAlso(expression1, expression2) 
                : Expression.OrElse(expression1, expression2);
	    }
	    private Expression GetStringExpression(Expression member, Operation operation, Expression constant, Expression constant2)
	    {
	        if (member == null)
                throw new ArgumentNullException(nameof(member));
	       
            if (member.Type != typeof(string))
	            return this.expressions[operation].Invoke(member, constant, constant2);

	        switch (operation)
	        {
	            case Operation.Equal:
	            case Operation.NotEqual:
	            case Operation.Contains:
	            case Operation.StartsWith:
	            case Operation.EndsWith:
	            case Operation.GreaterThan:
	            case Operation.GreaterThanOrEqualTo:
	            case Operation.LessThan:
	            case Operation.LessThanOrEqualTo:
	            case Operation.Between:
	            case Operation.IsEmpty:
	            case Operation.IsNotNull:
	            case Operation.IsNotEmpty:
	            case Operation.In:
	                return Expression.AndAlso(
                        Expression.NotEqual(member, Expression.Constant(null)), 
                        this.expressions[operation].Invoke(Expression.Call(Expression.Call(member, this.methods["StartsWith"]), this.methods["ToLower"]), constant, constant2));

                case Operation.IsNull:
	            case Operation.IsNullOrWhiteSpace:
	            case Operation.IsNotNullNorWhiteSpace:
	                return this.expressions[operation].Invoke(member, constant, constant2);

                default:
	                return this.expressions[operation].Invoke(member, constant, constant2);
	        }
        }
        private Expression GetArrayExpression(Expression parameter, IFilterStatement statement)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            var baseName = statement.Property.Substring(0, statement.Property.IndexOf("[", StringComparison.Ordinal));
            var type = parameter.Type.GetProperty(baseName).PropertyType.GetGenericArguments()[0];
            var name = statement.Property.Replace(baseName, "").Replace("[", "").Replace("]", "");
            var methodInfo = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "Any" && m.GetParameters().Length == 2).MakeGenericMethod(type);

            var member = this.GetMemberExpression(parameter, baseName);
            var itemParameter = Expression.Parameter(type, "i");
            var expression = Expression.Lambda(GetExpression(itemParameter, statement, name), itemParameter);

            return Expression.Call(methodInfo, member, expression);
        }
    }
}