using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plasma_Fractal
{
    public partial class Gen_Menu : Form
    {
        Main_State mainState;

        public Gen_Menu(Main_State mainState)
        {
            InitializeComponent();
            this.mainState = mainState;
        }

        private void txt_Width_TextChanged(object sender, EventArgs e)
        {
            if (chk_Linked.Checked)
                txt_Height.Text = txt_Width.Text;
        }

        private void txt_Height_TextChanged(object sender, EventArgs e)
        {
            if (chk_Linked.Checked)
                txt_Width.Text = txt_Height.Text;
        }

        private void txtRivers_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_Generate_Click(object sender, EventArgs e)
        {
            try
            {
                mainState.MakeIsland(int.Parse(txt_Width.Text), int.Parse(txt_Height.Text), chk_Coloured.Checked, chk_Shaded.Checked, chk_Noise.Checked, int.Parse(txtRivers.Text), int.Parse(txtBaseRough.Text), int.Parse(txtShaderRough.Text));
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a Width and Height!");
            }
        }
    }
}
