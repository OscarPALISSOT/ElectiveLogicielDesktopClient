using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAdministration
{
    public partial class Form2 : Form
    {
        private Form1 mother;

        public Form2(Form1 mother)
        {
            this.mother = mother;
            InitializeComponent();
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {

        }
    }
}
