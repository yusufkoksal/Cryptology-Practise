using System.Text.Json;
using Aes256;
namespace EsiProject2
{
    public partial class Form1 : Form
    {
        List<Person> persons = new List<Person>();
        public static readonly string FilePath = "C:\\Users\\ysf20\\source\\repos\\EsiProject2\\EsiProject2\\person.json";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            {
                var data = File.ReadAllText(FilePath);
                if (data != null || data != "")
                {
                    persons = JsonSerializer.Deserialize<List<Person>>(data);
                    foreach (var item in persons)
                    {

                        int rowIndex = dataGridView1.Rows.Add();
                        dataGridView1.Rows[rowIndex].Cells[0].Value = item.Id;
                        dataGridView1.Rows[rowIndex].Cells[1].Value = item.FirstName.Decrypt();
                        dataGridView1.Rows[rowIndex].Cells[2].Value = item.LastName.Decrypt();
                        dataGridView1.Rows[rowIndex].Cells[3].Value = item.BirthDay.Decrypt();
                    }
                }

            }
        }

        private void button1_Click(object sender, EventArgs e) // Add User
        {



            Person newPerson = new Person
            {
                FirstName = textBox1.Text,
                LastName = textBox2.Text,
                BirthDay = textBox3.Text,
            };

            int n = dataGridView1.Rows.Add();
            dataGridView1.Rows[n].Cells[0].Value = newPerson.Id;
            dataGridView1.Rows[n].Cells[1].Value = newPerson.FirstName;
            dataGridView1.Rows[n].Cells[2].Value = newPerson.LastName;
            dataGridView1.Rows[n].Cells[3].Value = newPerson.BirthDay;
            persons.Add(newPerson);

            MessageBox.Show($"User added successfully !");
        }

        private void button2_Click(object sender, EventArgs e) // Dýþa Aktar
        {
            List<Person> existingPersons = new List<Person>();

            // JSON dosyasýndan mevcut kullanýcýlarý yükle
            if (File.Exists(FilePath))
            {
                string existingJson = File.ReadAllText(FilePath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    existingPersons = JsonSerializer.Deserialize<List<Person>>(existingJson);
                }
            }

            // Sadece mevcut ID'leri bir listeye toplayýn
            var existingIds = existingPersons.Select(p => p.Id).ToHashSet();

            foreach (Person person in persons)
            {
                // Eðer ayný ID varsa kullanýcýyý atla
                //if (existingIds.Contains(person.Id))
                //{
                //    continue;
                //}

                // Yeni kullanýcýyý þifrele
                person.FirstName = person.FirstName.Encrypt1();
                person.LastName = person.LastName.Encrypt1();
                person.BirthDay = person.BirthDay.Encrypt1();

                // Yeni kullanýcýyý listeye ekle
                existingPersons.Add(person);
            }

            // Listeyi JSON formatýnda kaydet
            string json = JsonSerializer.Serialize(existingPersons, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(FilePath, json);

            MessageBox.Show("Encrypted users saved to JSON file successfully!");
        }

    }
}


