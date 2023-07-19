namespace SpecificationPattern
{
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
}