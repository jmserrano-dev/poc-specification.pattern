using FluentAssertions;
using Xunit;

namespace SpecificationPattern;

public class FilterSpecificationTest
{
    [Fact]
    public void Test1()
    {
        var persons = new List<Person>
            {
                new Person { Id = 1, Name = "John", Age = 33, IsAdult = true },
                new Person { Id = 2, Name = "Mary", Age = 15, IsAdult = false }
            };

        var filters = new List<Filter>
            {
                new Filter { Field = nameof(Person.Name), Value = "hn", Operator = FilterOperator.EndsWith }
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