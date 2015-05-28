using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Detail : Form
    {
        public Detail(int nSamples, double maxSize)
        {
            InitializeComponent();
            label1.Text = "Number of Samples: " + nSamples.ToString();
            label2.Text = "Maximum value in Samples: " + maxSize.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
