namespace SpecificationPattern
{
    record Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public bool IsAdult { get; set; }

        public List<string> Hobbies { get; set; }
        
        public DateTime Date { get; set; }
    }
}