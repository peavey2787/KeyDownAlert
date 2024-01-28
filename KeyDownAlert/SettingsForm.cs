using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyDownAlert
{
    public partial class SettingsForm : Form
    {
        public int Diameter { get; private set; }
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(DiameterTextBox.Text, out int result) && result > 0)
            {
                Diameter = result;
                DialogResult = DialogResult.OK;
                CloseForm();
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter a positive integer.");
            }
        }
        private void CloseForm()
        {
            AppsSettings.Save<string>("Diameter", DiameterTextBox.Text);
            Close();
        }
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            DiameterTextBox.Text = AppsSettings.Load<string>("Diameter");
        }
    }
}
