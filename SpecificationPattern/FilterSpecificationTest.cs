using FluentAssertions;
using Xunit;

namespace SpecificationPattern;

public class FilterSpecificationTest
{
    private static readonly List<Person> persons = new()
    {
        new Person {
            Id = 1,
            Name = "John",
            Age = 33,
            IsAdult = true,
            Hobbies = new() { "Footbal", "Padel" },
            Date = new DateTime(2023, 7, 25, 12, 0 , 0)
        },
        new Person {
            Id = 2,
            Name = "Mary",
            Age = 15,
            IsAdult = false,
            Hobbies = new() { "Footbal", "Tennis" },
            Date = new DateTime(2023, 7, 27)
        }
    };

    [Fact]
    public void SimpleFiltering()
    {
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
    public void DoesNotContain()
    {
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
                    new Filter { Field = nameof(Person.Name), Value = "o", Operator = FilterOperator.DoesNotContains },
                }
            }
        };

        var result = persons
           .AsQueryable()
           .Where(FilterSpecification<Person>.Create(filters.Filter).ToExpression())
           .ToList();

        result.Should().HaveCount(1);
        result.Should().Contain(persons.Last());
    }

    [Fact]
    public void ContainsList()
    {
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
                    new Filter { Field = nameof(Person.Hobbies), Value = "Padel", Operator = FilterOperator.ContainsList }
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
    public void DoesNotCContainList()
    {
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
                    new Filter { Field = nameof(Person.Hobbies), Value = "Padel", Operator = FilterOperator.DoesNotContainsList }
                }
            }
        };

        var result = persons
           .AsQueryable()
           .Where(FilterSpecification<Person>.Create(filters.Filter).ToExpression())
           .ToList();

        result.Should().HaveCount(1);
        result.Should().Contain(persons.Last());
    }

    [Fact]
    public void InDay()
    {
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
                    new Filter { Field = nameof(Person.Date), Value = new DateTime(2023, 7, 25), Operator = FilterOperator.InDay }
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
    public void NotInDay()
    {
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
                    new Filter { Field = nameof(Person.Date), Value = new DateTime(2023, 7, 25), Operator = FilterOperator.NotInDay }
                }
            }
        };

        var result = persons
           .AsQueryable()
           .Where(FilterSpecification<Person>.Create(filters.Filter).ToExpression())
           .ToList();

        result.Should().HaveCount(1);
        result.Should().Contain(persons.Last());
    }

    [Fact]
    public void ComplexFiltering()
    {
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