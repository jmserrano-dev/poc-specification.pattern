using FluentAssertions;
using Xunit;

namespace SpecificationPattern;

public class FilterSpecificationTest
{
    [Fact]
    public void SimpleFiltering()
    {
        var persons = new List<Person>
        {
            new Person { Id = 1, Name = "John", Age = 33, IsAdult = true },
            new Person { Id = 2, Name = "Mary", Age = 15, IsAdult = false }
        };

        var filters = new ListRequest
        {
            Skip = 0,
            Take = 100,
            Sort = new List<Sort>(),
            Filter = new Filter
            {
                Logic = FilterLogic.And,
                Filters = new List<Filter>
                {
                    new Filter { Field = nameof(Person.Name), Value = "John", Operator = FilterOperator.Contains },
                    new Filter { Field = nameof(Person.Age), Value = 10, Operator = FilterOperator.GreaterThan }
                }
            }
        };

        var result = persons
           .AsQueryable()
           .Where(FilterSpecification<Person>.Create(filters.Filter).ToExpression())
           .ToList();

        result.Should().HaveCount(1);
        result.Should().Contain(persons.First());
    }

    [Fact]
    public void ComplexFiltering()
    {
        var persons = new List<Person>
        {
            new Person { Id = 1, Name = "John", Age = 33, IsAdult = true },
            new Person { Id = 2, Name = "Mary", Age = 15, IsAdult = false }
        };

        var request = new ListRequest
        {
            Skip = 0,
            Take = 100,
            Sort = new List<Sort>(),
            Filter = new Filter
            {
                Logic = FilterLogic.Or,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Logic = FilterLogic.And,
                        Filters = new List<Filter>
                        {
                            new Filter { Field = nameof(Person.Name), Value = "John", Operator = FilterOperator.Contains },
                            new Filter { Field = nameof(Person.Age), Value = 30, Operator = FilterOperator.GreaterThan }
                        }
                    },
                    new Filter
                    {
                        Logic = FilterLogic.And,
                        Filters = new List<Filter>
                        {
                            new Filter { Field = nameof(Person.Name), Value = "Mary", Operator = FilterOperator.Contains },
                            new Filter { Field = nameof(Person.Age), Value = 30, Operator = FilterOperator.LowerThan }
                        }
                    }
                }
            }
        };

        var result = persons
           .AsQueryable()
           .Where(FilterSpecification<Person>.Create(request.Filter).ToExpression())
           .ToList();

        result.Should().HaveCount(2);
        result.Should().Contain(persons.First());
        result.Should().Contain(persons.Last());
    }
}