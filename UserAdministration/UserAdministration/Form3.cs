using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAdministration
{
    public partial class Form3 : Form
    {
        Controller ORM;
        public Form3(Controller ORM)
        {
            InitializeComponent();
            CancelButton = closeButton;
            this.ORM = ORM;
            refreshDB();
        }

        private void refreshDB(params List<string>[] lists)
        {
            try
            {
                SqlDataAdapter da = lists.Length == 0 ? new SqlDataAdapter(ORM.Select("Roles")) : new SqlDataAdapter(ORM.Select(lists[0], lists[1], "Roles"));
                DataSet ds = new DataSet();
                da.Fill(ds, "Roles");
                roleGridView.DataSource = ds.Tables["Roles"].DefaultView;
                roleGridView.ClearSelection();
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
                if (!string.IsNullOrEmpty(roleTextbox.Text))
                {
                    ORM.InsertRole(roleTextbox.Text);
                }
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
            try { ORM.Delete(Convert.ToInt32(roleGridView.SelectedRows[0].Cells[0].Value), false); } 
            catch (SqlException err) { } 
            finally { refreshDB(); }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            List<string> a = new List<string>(); 
            List<string> b = new List<string>();
            a.Add(roleTextbox.Text);
            b.Add("Roles");
            refreshDB(b,a);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (roleGridView.SelectedRows.Count != 0 && !string.IsNullOrEmpty(roleTextbox.Text))
                {
                    ORM.UpdateRole(System.Convert.ToInt32(roleGridView.SelectedRows[0].Cells[0].Value), roleTextbox.Text);
                }
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

        private void closeButton_Click(object sender, EventArgs e)
        {
            ORM = null;
            this.Close();
        }

        private void roleGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            editButton.Enabled = true;
            deleteButton.Enabled = true;
            roleTextbox.Text = roleGridView.SelectedRows[0].Cells[1].Value.ToString();
        }

        private void errorFormDisplay(string msg)
        {
            RoleManagement error = new RoleManagement(msg);
            error.ShowDialog();
        }
    }
}
