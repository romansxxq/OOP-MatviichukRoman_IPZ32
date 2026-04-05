using System.Net.Cache;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var repo = new ContactRepository();
    //create some contacts
        var contact1 = new Contact { Id = 1, Name = "John Doe", Email = "john.doe@example.com", Phone = "123-456-7890" };
        var contact2 = new Contact { Id = 2, Name = "Jane Smith", Email = "jane.doe@example.com", Phone = "987-654-3210" };
        var contact3 = new Contact { Id = 3, Name = "Bob Johnson", Email = "bob.doe@example.com", Phone = "555-555-5555" };
    //create some groups
        var group1 = new Group { Id = 1, Name = "Friends", Contacts = new List<Contact> { contact1, contact2 } };
        var group2 = new Group { Id = 2, Name = "Family", Contacts = new List<Contact> { contact3 } };
    //create repository and add groups
        repo.Add(group1);
        repo.Add(group2);
        //save to file
        await repo.SaveToFileAsync("contacts.json");
        System.Console.WriteLine("Contacts saved to file.");
        //new repository to load data
        var newRepo = new ContactRepository();
        await newRepo.LoadFromFileAsync("contacts.json");
        //output loaded data
        var groups = newRepo.GetAll();
        foreach (var group in groups)
        {
            Console.WriteLine($"Group: {group.Name}");
            foreach (var contact in group.Contacts)
            {
                Console.WriteLine($"Contact: {contact.Name}, Email: {contact.Email}, Phone: {contact.Phone}");
            }
        }
    }
}
//Contact class
public class Contact
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}
//Group class
public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Contact> Contacts { get; set; } = new List<Contact>();
}
public class ContactRepository
{
    private List<Group> groups = new List<Group>();

    public void Add(Group group)
    {
        groups.Add(group);
    }
    public List<Group> GetAll()
    {
        return groups;
    }
    public Group GetById(int id)
    {
        return groups.FirstOrDefault(g => g.Id == id);
    }
    public async Task SaveToFileAsync(string filename)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        using FileStream fs = new FileStream(filename, FileMode.Create);
        await JsonSerializer.SerializeAsync(fs, groups, options);
    }
    public async Task LoadFromFileAsync(string filename)
    {
        if (!File.Exists(filename))
        {
            Console.WriteLine("File not found.");
            return;
        }
        using FileStream fs = new FileStream(filename, FileMode.Open);
        groups = await JsonSerializer.DeserializeAsync<List<Group>>(fs) ?? new List<Group>();
    }
}