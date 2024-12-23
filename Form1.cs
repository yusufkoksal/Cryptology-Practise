using System.Text.Json;
using Aes256;
namespace EsiProject2
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        List<Person> persons = new List<Person>();
        public static readonly string FilePath = "C:\\Users\\ysf20\\source\\repos\\EsiProject2\\EsiProject2\\person.json";
        private bool hasUnsavedChanges = false;


        private void LoadData()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var data = File.ReadAllText(FilePath);
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        persons = JsonSerializer.Deserialize<List<Person>>(data);
                        // Mevcut en yüksek ID'ye göre nextId'yi güncelle
                        Person.UpdateNextId(persons);

                        dataGridView1.Rows.Clear();

                        foreach (var item in persons)
                        {
                            int rowIndex = dataGridView1.Rows.Add();
                            dataGridView1.Rows[rowIndex].Cells[0].Value = item.Id;
                            dataGridView1.Rows[rowIndex].Cells[1].Value = item.FirstName.Decrypt();
                            dataGridView1.Rows[rowIndex].Cells[2].Value = item.LastName.Decrypt();
                            dataGridView1.Rows[rowIndex].Cells[3].Value = item.BirthDay.Decrypt();

                            item.FirstName = item.FirstName.Decrypt();
                            item.LastName = item.LastName.Decrypt();
                            item.BirthDay = item.BirthDay.Decrypt();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void AddUser(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Please fill in all fields!");
                return;
            }

            Person newPerson = new Person
            {
                Id = Person.GetNextId(), // Yeni ID al
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

            // Deðiþiklikleri hemen kaydet
            SaveChanges();

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();

            MessageBox.Show("User added successfully!");
        }
        private void ExportData(object sender, EventArgs e) // Dýþa Aktar
        {
            try
            {
                if (persons.Count == 0)
                {
                    MessageBox.Show("No data to export!");
                    return;
                }

                // Yeni bir liste oluþtur ve verileri þifrele
                List<Person> encryptedPersons = persons.Select(p => new Person
                {
                    Id = p.Id,
                    // Eðer veri zaten þifreliyse tekrar þifreleme
                    FirstName = p.FirstName.Contains("==") ? p.FirstName : p.FirstName.Encrypt1(),
                    LastName = p.LastName.Contains("==") ? p.LastName : p.LastName.Encrypt1(),
                    BirthDay = p.BirthDay.Contains("==") ? p.BirthDay : p.BirthDay.Encrypt1()
                }).ToList();

                string json = JsonSerializer.Serialize(encryptedPersons, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(FilePath, json);
                MessageBox.Show("Data exported successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}");
            }
        }

        private void updateUser(object sender, EventArgs e) // update Button
        {
            UpdateUser();
        }

        private void UpdateUser()
        {
            try
            {
                // Seçili satýr kontrolü
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a user to update!");
                    return;
                }

                // Textbox'larýn boþ olup olmadýðýný kontrol et
                if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Please fill in all fields!");
                    return;
                }

                // Seçili satýrýn index'ini al
                int selectedIndex = dataGridView1.SelectedRows[0].Index;

                // Seçili kullanýcýnýn ID'sini al
                int selectedId = Convert.ToInt32(dataGridView1.Rows[selectedIndex].Cells[0].Value);

                // Persons listesinde ilgili kullanýcýyý bul
                var personToUpdate = persons.FirstOrDefault(p => p.Id == selectedId);
                if (personToUpdate != null)
                {
                    // Kullanýcý bilgilerini güncelle
                    personToUpdate.FirstName = textBox1.Text;
                    personToUpdate.LastName = textBox2.Text;
                    personToUpdate.BirthDay = textBox3.Text;

                    // DataGridView'i güncelle
                    dataGridView1.Rows[selectedIndex].Cells[1].Value = textBox1.Text;
                    dataGridView1.Rows[selectedIndex].Cells[2].Value = textBox2.Text;
                    dataGridView1.Rows[selectedIndex].Cells[3].Value = textBox3.Text;

                    // TextBox'larý temizle
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();

                    MessageBox.Show("User updated successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}");
            }
        }

        // DataGridView'de satýr seçildiðinde TextBox'larý doldur
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    var selectedRow = dataGridView1.SelectedRows[0];
                    textBox1.Text = selectedRow.Cells[1].Value?.ToString();
                    textBox2.Text = selectedRow.Cells[2].Value?.ToString();
                    textBox3.Text = selectedRow.Cells[3].Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting user: {ex.Message}");
            }
        }


        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0) // Baþlýk satýrýna týklamayý önle
                {
                    // Seçilen satýrý al
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    // TextBox'larý doldur
                    textBox1.Text = row.Cells[1].Value?.ToString(); // FirstName
                    textBox2.Text = row.Cells[2].Value?.ToString(); // LastName
                    textBox3.Text = row.Cells[3].Value?.ToString(); // BirthDay

                    // Seçilen satýrý vurgula
                    dataGridView1.CurrentRow.Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting row: {ex.Message}");
            }
        }



        private void deleteButton(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Please select a row to delete!");
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this record?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int selectedIndex = dataGridView1.CurrentRow.Index;
                    int personId = Convert.ToInt32(dataGridView1.Rows[selectedIndex].Cells[0].Value);

                    // Kiþiyi persons listesinden bul ve sil
                    var personToDelete = persons.FirstOrDefault(p => p.Id == personId);
                    if (personToDelete != null)
                    {
                        persons.Remove(personToDelete);
                        dataGridView1.Rows.RemoveAt(selectedIndex);


                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();

                        // Persons listesi boþ deðilse, nextId'yi güncelle
                        if (persons.Any())
                        {
                            Person.UpdateNextId(persons);
                        }


                        SaveChanges();

                        MessageBox.Show("Record deleted successfully!");
                    }
                    else
                    {
                        MessageBox.Show("User not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting record: {ex.Message}");
            }
        }

        private void SaveChanges()
        {
            try
            {
                if (persons.Count == 0)
                {
                    if (File.Exists(FilePath))
                    {
                        File.WriteAllText(FilePath, "[]"); // Boþ bir JSON array yaz
                    }
                    return;
                }

                List<Person> encryptedPersons = persons.Select(p => new Person
                {
                    Id = p.Id,
                    FirstName = p.FirstName.Contains("==") ? p.FirstName : p.FirstName.Encrypt1(),
                    LastName = p.LastName.Contains("==") ? p.LastName : p.LastName.Encrypt1(),
                    BirthDay = p.BirthDay.Contains("==") ? p.BirthDay : p.BirthDay.Encrypt1()
                }).ToList();

                string json = JsonSerializer.Serialize(encryptedPersons, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving changes: {ex.Message}");
            }
        }
    }
}

