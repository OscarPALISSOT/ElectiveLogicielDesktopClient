using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace UserAdministration
{
    public partial class Form2 : Form
    {
        private Form1 mother;

        public Form2(Form1 mother)
        {
            this.mother = mother;
            InitializeComponent();
            AcceptButton = confirmButton;
            CancelButton = cancelButton;
        }

        private void confirmButton_Click(object sender, EventArgs e) {
            if (usernameBox.Text.Length == 0 || passwordBox.Text.Length == 0) {
                
            }
            else {
                try {
                    Controller ORM = new Controller(usernameBox.Text.Trim(), passwordBox.Text.Trim());
                    mother.setORM(ORM);
                    mother.connected = true;
                    this.Close();
                }
                catch (SqlException) {

                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
