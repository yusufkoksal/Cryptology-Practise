public class Person
{
    private static int nextId = 1;

    public Person()
    {

    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BirthDay { get; set; }


    public static void UpdateNextId(List<Person> persons)
    {
        if (persons != null && persons.Any())
        {
            nextId = persons.Max(p => p.Id) + 1;
        }
        else
        {
            nextId = 1;
        }
    }

    public static int GetNextId()
    {
        return nextId++;
    }
}