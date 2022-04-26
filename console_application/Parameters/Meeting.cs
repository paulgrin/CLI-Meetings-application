namespace console_application.Parameters
{
    public record Meeting 
    {
        public Meeting()
        {
        }

        public Meeting(int id, string name, string responsiblePerson, string description, Category category, Type type, DateTime startDate, DateTime endDate, List<string> attendees)
        {
            Id = id;
            Name = name;
            ResponsiblePerson = responsiblePerson;
            Description = description;
            Category = category;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            Attendees = attendees;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public Type Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Attendees { get; set; }

    }
}
