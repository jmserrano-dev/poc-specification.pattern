using System.Linq.Expressions;

namespace SpecificationPattern
{
    class FilterSpecification<T> : Specification<T>
    {
        private readonly Filter _filter;
        private readonly ParameterExpression _parameter;

        private FilterSpecification(Filter filter)
        {
            _filter = filter;
            _parameter = Expression.Parameter(typeof(T));
        }

        public static FilterSpecification<T> Create(Filter filter)
        {
            return new FilterSpecification<T>(filter);
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var expression = Generate(_filter);
            return Expression.Lambda<Func<T, bool>>(expression, _parameter);
        }

        private Expression Generate(Filter filter)
        {
            if (!filter.Filters.Any())
                return GenerateIdentityExpression(filter.Logic);

            return GroupingWithLogicOperator(filter.Filters.Select((currentFilter) =>
            {
                return currentFilter.Filters.Any()
                    ? Generate(currentFilter)
                    : GroupingWithLogicOperator(filter.Filters.Select(GenerateExpression), filter);
            }), filter);
        }

        private static Expression GenerateIdentityExpression(string logic)
        {
            return logic == FilterLogic.And
                ? Expression.Equal(Expression.Constant(true), Expression.Constant(true))
                : Expression.Equal(Expression.Constant(true), Expression.Constant(false));
        }

        private Expression GenerateExpression(Filter filter)
        {
            var propertyExpression = Expression.Property(_parameter, filter.Field);
            var valueExpression = Expression.Constant(filter.Value);
            var nullExpression = Expression.Constant(null);
            var emptyExpression = Expression.Constant("");

            return filter.Operator switch
            {
                FilterOperator.Equal => Expression.Equal(propertyExpression, valueExpression),
                FilterOperator.NotEqual => Expression.NotEqual(propertyExpression, valueExpression),
                FilterOperator.GreaterThan => Expression.GreaterThan(propertyExpression, valueExpression),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(propertyExpression, valueExpression),
                FilterOperator.LowerThan => Expression.LessThan(propertyExpression, valueExpression),
                FilterOperator.LowerThanOrEqual => Expression.LessThanOrEqual(propertyExpression, valueExpression),
                FilterOperator.IsNull => Expression.Equal(propertyExpression, nullExpression),
                FilterOperator.IsNotNull => Expression.NotEqual(propertyExpression, nullExpression),
                FilterOperator.IsEmpty => Expression.Equal(propertyExpression, emptyExpression),
                FilterOperator.IsNotEmpty => Expression.NotEqual(propertyExpression, emptyExpression),
                FilterOperator.Contains => Expression.Call(propertyExpression, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, valueExpression),
                FilterOperator.DoesNotContains => Expression.Not(Expression.Call(propertyExpression, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, valueExpression)),
                FilterOperator.StartsWith => Expression.Call(propertyExpression, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, valueExpression),
                FilterOperator.EndsWith => Expression.Call(propertyExpression, typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!, valueExpression),
                FilterOperator.ContainsList => Expression.Call(propertyExpression, typeof(List<string>).GetMethod("Contains", new[] { typeof(string) })!, valueExpression),
                FilterOperator.DoesNotContainsList => Expression.Not(Expression.Call(propertyExpression, typeof(List<string>).GetMethod("Contains", new[] { typeof(string) })!, valueExpression)),
                FilterOperator.InDay => Expression.AndAlso(
                    Expression.GreaterThanOrEqual(propertyExpression, valueExpression),
                    Expression.LessThan(propertyExpression, Expression.Call(valueExpression, typeof(DateTime).GetMethod("AddDays", new[] { typeof(double) })!, Expression.Constant(1.0)))),
                FilterOperator.NotInDay =>Expression.OrElse(
                    Expression.LessThan(propertyExpression, valueExpression),
                    Expression.GreaterThanOrEqual(propertyExpression, Expression.Call(valueExpression, typeof(DateTime).GetMethod("AddDays", new[] { typeof(double) })!, Expression.Constant(1.0)))),
                FilterOperator.GreaterThanDay => Expression.GreaterThan(propertyExpression, Expression.Call(valueExpression, typeof(DateTime).GetMethod("AddDays", new[] { typeof(double) })!, Expression.Constant(1.0))),
                FilterOperator.GreaterThanOrEqualDay => Expression.GreaterThanOrEqual(propertyExpression, valueExpression),
                FilterOperator.LowerThanDay => Expression.LessThan(propertyExpression, valueExpression),
                FilterOperator.LowerThanOrEqualDay => Expression.LessThanOrEqual(propertyExpression, Expression.Call(valueExpression, typeof(DateTime).GetMethod("AddDays", new[] { typeof(double) })!, Expression.Constant(1.0))),
                _ => GenerateIdentityExpression(filter.Logic),
            };
        }

        private static Expression GroupingWithLogicOperator(IEnumerable<Expression> source, Filter filter)
        {
            return source.Aggregate(GenerateIdentityExpression(filter.Logic), (expresion, currentExpression) =>
            {
                return filter.Logic == FilterLogic.And
                    ? Expression.AndAlso(expresion, currentExpression)
                    : Expression.OrElse(expresion, currentExpression);
            });
        }
    }
}