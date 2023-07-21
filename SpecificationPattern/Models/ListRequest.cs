namespace SpecificationPattern
{
    record ListRequest
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public Filter Filter { get; set; } = new Filter();
        public List<Sort> Sort { get; set; } = new List<Sort>();
    }

    record Filter
    {
        public string Field { get; set; }
        public object Value { get; set; }
        public string Operator { get; set; }
        public string Logic { get; set; }
        public List<Filter> Filters { get; set; } = new List<Filter>();
    }

    record Sort
    {
        public string Dir { get; set; }
        public string Field { get; set; }
    }
}