using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace UserAdministration
{
    public partial class Form1 : Form
    {
        public Controller ORM { set; get; }
        public bool connected = false;
        private DataSet ds;
        public void setORM(Controller var)
        {
            ORM = var;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<TextBox> textboxes = groupBox1.Controls.OfType<TextBox>().ToList();
                textboxes.RemoveAll(select => string.IsNullOrEmpty(select.Text));
                List<string> columns = textboxes.Select(select => select.Name.Substring(0, select.Name.Length - 3)).ToList();
                List<string> values = textboxes.Select(select => select.Text).ToList();
                refreshDB(columns, values);
            }
            catch (SqlException err)
            {
                errorFormDisplay(err.Message);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<Box> roles = roleBox.CheckedItems.Cast<Box>().ToList();
                List<TextBox> textBoxes = groupBox1.Controls.OfType<TextBox>().ToList();
                if (textBoxes.All(textbox => !string.IsNullOrWhiteSpace(textbox.Text)))
                    ORM.Insert(textBoxes.Select(select => select.Text).ToList(), textBoxes.Select(select => select.Name.Substring(0, select.Name.Length - 3)).ToList(),roles);
                else
                    errorFormDisplay("Please complete all fields");
            }
            catch (SqlException err)
            {
                errorFormDisplay(err.Message);
            }
            finally
            {
                refreshDB();
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<Box> roles = roleBox.CheckedItems.Cast<Box>().ToList();
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
                List<TextBox> textBoxes = groupBox1.Controls.OfType<TextBox>().ToList();
                if (textBoxes.All(textbox => !string.IsNullOrWhiteSpace(textbox.Text)))
                    ORM.Update(id, textBoxes.Select(select => select.Text).ToList(), textBoxes.Select(select => select.Name.Substring(0, select.Name.Length - 3)).ToList(), roles);
                else
                    errorFormDisplay("Please complete all fields");
            }
            catch (ArgumentOutOfRangeException)
            {

            }
            catch (SqlException err)
            {
                errorFormDisplay(err.Message);
            }
            finally
            {
                refreshDB();
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                ORM.Delete(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value));
            }
            catch (SqlException err)
            {
                RoleManagement error = new RoleManagement(err.Message);
            }
            finally
            {
                refreshDB();
                manageTextBox();
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                Form2 connectionForm = new Form2(this);
                connectionForm.ShowDialog();
                if (connected)
                {
                    buttonControl(true);
                }
            }
            else
            {
                buttonControl(false);
                ORM = null;
                connected = false;
            }
        }

        private void dataGridView1_EnabledChanged(object sender, EventArgs e)
        {
            if (dataGridView1.Enabled)
                refreshDB();
            else
                dataGridView1.DataSource = null;
        }

        private void buttonControl(bool a)
        {
            dataGridView1.Enabled = a;
            searchButton.Enabled = a;
            addButton.Enabled = a;
            groupBox1.Enabled = a;
            connectButton.Text = a ? "Disconnect" : "Connect";
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                deleteButton.Enabled = true;
                editButton.Enabled = true;
                tokenDropButton.Enabled = true;
                DataGridViewCellCollection row = dataGridView1.SelectedRows[0].Cells;
                manageTextBox(row[1].Value.ToString().Trim(), row[2].Value.ToString().Trim(), row[3].Value.ToString().Trim(), row[10].Value.ToString().Trim());
                try
                {
                    using (SqlDataReader reader = ORM.SelectRole(Convert.ToInt32(row[0].Value)))
                    {
                        List<int> checkedIndex = new List<int>();
                        foreach (Box a in roleBox.CheckedItems)
                            checkedIndex.Add(roleBox.Items.IndexOf(a));
                        foreach (int index in checkedIndex)
                            roleBox.SetItemChecked(index, false);
                        while (reader.Read())
                        {
                            Box item = roleBox.Items.Cast<Box>().First(s => s.ID == reader.GetInt32(1));
                            int index = roleBox.Items.IndexOf(item);
                            roleBox.SetItemChecked(index, true);
                        }
                    }
                }
                catch (SqlException err)
                {
                    errorFormDisplay(err.Message);
                }
            }
        }

        private void manageTextBox(string firstName = "", string lastName = "", string password = "", string mail = "")
        {
            FirstNameBox.Text = firstName;
            LastNameBox.Text = lastName;
            PasswordBox.Text = password;
            EmailBox.Text = mail;
        }

        private void refreshDB(params List<string>[] lists)
        {
            SqlDataAdapter dataAdapter = lists.Length == 0 ? new SqlDataAdapter(ORM.Select()) : new SqlDataAdapter(ORM.Select(lists[0], lists[1]));
            ds = new DataSet();
            dataAdapter.Fill(ds, "Users");
            dataGridView1.DataSource = ds.Tables["Users"].DefaultView;
            dataGridView1.ClearSelection();
            deleteButton.Enabled = false;
            editButton.Enabled = false;
            tokenDropButton.Enabled = false;
            manageTextBox();
        }

        private void roleButton_Click(object sender, EventArgs e)
        {
            Form3 roleManagement = new Form3(ORM);
            roleManagement.ShowDialog();
            manageCheckboxRole();
        }

        private void checkedListBox1_EnabledChanged(object sender, EventArgs e)
        {
            if (roleBox.Enabled)
                manageCheckboxRole();
            else
                roleBox.Items.Clear();
        }

        private void tokenDropButton_Click(object sender, EventArgs e)
        {
            try
            {
                ORM.DropToken(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value));
            }
            catch (ArgumentOutOfRangeException) { }
            catch (SqlException err)
            {
                errorFormDisplay(err.Message);
            }
        }

        private void errorFormDisplay(string msg)
        {
            RoleManagement error = new RoleManagement(msg);
            error.ShowDialog();
        }

        private void manageCheckboxRole()
        {
            roleBox.Items.Clear();
            using (SqlDataReader reader = ORM.Select("Roles").ExecuteReader())
            {
                while (reader.Read())
                {
                    Box cb = new Box(reader.GetInt32(0), reader.GetString(1));
                    roleBox.Items.Add(cb);
                }
            }
        }
    }
}
