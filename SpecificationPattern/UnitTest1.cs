using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;

namespace SpecificationPattern
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var persons = new List<Person>
            {
                new Person { Id = 1, Name = "", Age = 33, IsAdult = true },
                new Person { Id = 2, Name = "Mary", Age = 15, IsAdult = false }
            };

            var filters = new List<Filter>
            {
                new Filter { Field = nameof(Person.Name), Operator = FilterOperator.IsEmpty }
                //new Filter { Field = nameof(Person.Id), Value = 1, Operator = FilterOperator.Equal },
                //new Filter { Field = nameof(Person.Name), Value = "John", Operator = FilterOperator.Equal },
                //new Filter { Field = nameof(Person.IsAdult), Value = true },
                //new Filter { Field = nameof(Person.Age), Value = 30, Operator = "lte" },
            };

            var result = persons
                .AsQueryable()
                .Where(new FilterSpecification<Person>(filters).ToExpression())
                .ToList();


            result.Should().HaveCount(1);
            //result.Should().HaveCount(1);
            //result.Should().HaveCount(2);
            result.Should().Contain(persons.First());
            //result.Should().Contain(persons.Last());
        }
    }

    record Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public bool IsAdult { get; set; }
    }

    record ListRequest
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public List<Sort> Sort { get; set; } = new List<Sort>();
        public List<Filter> Filter { get; set; } = new List<Filter>();
    }

    record Filter
    {
        public string Field { get; set; }
        public object Value { get; set; }
        public string Operator { get; set; }
    }

    record Sort
    {
        public string Dir { get; set; }
        public string Field { get; set; }
    }

    /*--- SPECIFIACTION PATTERN ---*/

    public static class FilterOperator
    {
        public const string Equal = "eq"; // ✅
        public const string NotEqual = "neq"; // ✅
        public const string IsNull = "isnull"; // ✅
        public const string IsNotNull = "isnotnull"; // ✅
        public const string LowerThan = "lt"; // ✅
        public const string LowerThanOrEqual = "lte"; // ✅
        public const string GreaterThan = "gt"; // ✅
        public const string GreaterThanOrEqual = "gte"; // ✅
        public const string StartsWith = "startswith";
        public const string EndsWith = "endswith";
        public const string Contains = "contains";
        public const string DoesNotContains = "doesnotcontain";
        public const string ContainsList = "containslist";
        public const string DoesNotContainsList = "doesnotcontainlist";
        public const string IsEmpty = "isempty"; // ✅
        public const string IsNotEmpty = "isnotempty"; // ✅
        public const string InDay = "inday";
        public const string NotInDay = "ninday";
        public const string GreaterThanDay = "gtinday";
        public const string GreaterThanOrEqualDay = "gteinday";
        public const string LowerThanDay = "ltinday";
        public const string LowerThanOrEqualDay = "lteinday";
    }

    abstract class Specification<T>
    {
        public abstract Expression<Func<T, bool>> ToExpression();

        public bool IsSatisfiedBy(T entity)
        {
            Func<T, bool> predicate = ToExpression().Compile();
            return predicate(entity);
        }
    }

    class FilterSpecification<T> : Specification<T>
    {
        private readonly List<Filter> _filter;
        private readonly ParameterExpression _parameter;

        public FilterSpecification(List<Filter> filter)
        {
            _filter = filter;
            _parameter = Expression.Parameter(typeof(T));
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var expression = _filter
                .Select(filter => GenerateExpression(filter))
                .Aggregate(GenerateIdentityExpression(), (expression, currentExpression) =>
                {
                    return Expression.AndAlso(expression, currentExpression);
                });


            return Expression.Lambda<Func<T, bool>>(expression, _parameter);
        }

        private static BinaryExpression GenerateIdentityExpression()
        {
            return Expression.Equal(Expression.Constant(true), Expression.Constant(true));
        }

        private BinaryExpression GenerateExpression(Filter filter) {
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

                //"ct" => Expression.Call(left, typeof(string).GetMethod("Contains"), right),
                _ => GenerateIdentityExpression(),
            };;
        }
    }
}