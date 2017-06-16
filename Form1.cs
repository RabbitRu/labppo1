using labppo1.InnerStruct;
using labppo1.Fileworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace labppo1
{
    public partial class Form1 : Form
    {
        private DataTree tree;
        private ILoader xmlworker;

        public Form1()
        {
            InitializeComponent();
            tree = new DataTree();
            xmlworker = new XMLLoader();
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = openFileDialog1.FileName;

            tree.Clear();
            tree = xmlworker.LoadFromFile(filename);

        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = saveFileDialog1.FileName;
            xmlworker.WriteToFile(filename, tree);

        }
    }
}
