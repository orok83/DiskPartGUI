using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskPartForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<Disk> Drivers { get; set; } = Disks.getDisks();
        private void refreshButton_Click(object sender, EventArgs e)
        {
            var formatedDriversList = (from d in Drivers
                                       select new { Name = $"{d.Index} - ({d.status}) - {d.Size}", Value = d.Index }).ToList();
            disksComboBox.DataSource = formatedDriversList;
            disksComboBox.DisplayMember = "Name";
            disksComboBox.ValueMember = "Value";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            refreshButton_Click(sender, e);
        }

        private void repairButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedDrive = Drivers.FirstOrDefault(d => d.Index == disksComboBox.SelectedValue.ToString());
                var formatType = fat32RadioButton.Checked ? "fat32" : "ntfs";
                var result = MessageBox.Show($"Are you sure do you want to format {selectedDrive.Index} - ({selectedDrive.status}) - {selectedDrive.Size} with {formatType} type?", "Confirm Disk Clean", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    resultTextBox.Text=Disks.cleanDisk(selectedDrive.Index, formatType);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
