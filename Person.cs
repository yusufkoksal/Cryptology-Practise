namespace EsiProject2
{
    public class Person
    {
        private static int _nextId = 1;

        public int Id { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDay { get; set; }

        public Person()
        {
            Id = _nextId++;
        }

    }
}
