using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite; //
using System.IO;            //

namespace WindowsFormsApp7
{
    public partial class Form1 : Form
    {
        private SQLiteConnection connection;
        private SQLiteDataAdapter adapter;
        private DataTable dataTable = new DataTable();

        public Form1()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadData();
        }

        private void InitializeDatabase()
        {
            // Define database file path
            string dbFile = "data.db";

            // Check if database file exists, create it if it doesn't
            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
            }

            // Initialize the SQLite connection
            connection = new SQLiteConnection($"Data Source={dbFile};Version=3;");
            connection.Open();

            // Create table if it doesn't exist
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Persons (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Age INTEGER NOT NULL,
                    Phone TEXT NOT NULL,
                    Address TEXT NOT NULL,
                    Date TEXT NOT NULL
                );";
            using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void LoadData()
        {
            // Load data from the database into the DataTable
            string selectQuery = "SELECT * FROM Persons";
            adapter = new SQLiteDataAdapter(selectQuery, connection);
            SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(adapter);

            dataTable.Clear();
            adapter.Fill(dataTable);

            // Bind the DataTable to the DataGridView
            dataGridView1.DataSource = dataTable;
        }
             private void btnInsert_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Data will put into the Database", "test");
            try
            {
                string insertQuery = @"
                    INSERT INTO Persons (Name, Age, Phone, Address, Date)
                    VALUES (@Name, @Age, @Phone, @Address, @Date)";
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", txtName.Text);
                    command.Parameters.AddWithValue("@Age", int.Parse(txtAge.Text));
                    command.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    command.Parameters.AddWithValue("@Address", txtAddress.Text);
                    command.Parameters.AddWithValue("@Date", dateTimePicker1.Value.ToString("yyyy-MM-dd"));

                    command.ExecuteNonQuery();
                }
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null)
                {
                    int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value);

                    string updateQuery = @"
                        UPDATE Persons
                        SET Name = @Name, Age = @Age, Phone = @Phone, Address = @Address, Date = @Date
                        WHERE Id = @Id";
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Name", txtName.Text);
                        command.Parameters.AddWithValue("@Age", int.Parse(txtAge.Text));
                        command.Parameters.AddWithValue("@Phone", txtPhone.Text);
                        command.Parameters.AddWithValue("@Address", txtAddress.Text);
                        command.Parameters.AddWithValue("@Date", dateTimePicker1.Value.ToString("yyyy-MM-dd"));

                        command.ExecuteNonQuery();
                    }
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Please select a row to update.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null)
                {
                    int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value);

                    string deleteQuery = "DELETE FROM Persons WHERE Id = @Id";
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ClearInputs()
        {
            txtName.Clear();
            txtAge.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            dateTimePicker1.Value = DateTime.Now;
            txtName.Focus();
        }
    }
}
