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
        public void setORM(Controller var) {
            ORM = var;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            try {
                List<TextBox> textboxes = groupBox1.Controls.OfType<TextBox>().ToList();
                textboxes.RemoveAll(select => string.IsNullOrEmpty(select.Text));
                List<string> columns = textboxes.Select(select => select.Name.Substring(0, select.Name.Length - 3)).ToList();
                List<string> values = textboxes.Select(select => select.Text).ToList();
                refreshDB(columns, values);
            }
            catch (SqlException) {

            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<TextBox> textBoxes = groupBox1.Controls.OfType<TextBox>().ToList();
                if (textBoxes.All(textbox => !string.IsNullOrWhiteSpace(textbox.Text)))
                    ORM.Insert(textBoxes.Select(select => select.Text).ToList(), textBoxes.Select(select => select.Name.Substring(0, select.Name.Length - 3)).ToList());
            }
            catch (SqlException)
            {

            }
            finally
            {
                refreshDB();
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            try {
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
                List<TextBox> textBoxes = groupBox1.Controls.OfType<TextBox>().ToList();
                if (textBoxes.All(textbox => !string.IsNullOrWhiteSpace(textbox.Text)))
                    ORM.Update(id, textBoxes.Select(select => select.Text).ToList(), textBoxes.Select(select => select.Name.Substring(0, select.Name.Length - 3)).ToList());
            }
            catch (ArgumentOutOfRangeException) {

            }
            catch (SqlException)
            {

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
            catch (SqlException)
            {

            }
            finally
            {
                refreshDB();
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (!connected) {
                Form2 connectionForm = new Form2(this);
                connectionForm.ShowDialog();
                if (connected)
                {
                    buttonControl(true);
                }
            }
            else {
                buttonControl(false);
                ORM = null;
                connected = false;
            }
        }

        private void dataGridView1_EnabledChanged(object sender, EventArgs e)
        {
            refreshDB();
        }

        private void buttonControl(bool a) {
            dataGridView1.Enabled = a;
            searchButton.Enabled = a;
            addButton.Enabled = a;
            groupBox1.Enabled = a;
            connectButton.Text = a ? "Disconnect" : "Connect";
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            deleteButton.Enabled = true;
            editButton.Enabled = true;
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
        }
    }
}
