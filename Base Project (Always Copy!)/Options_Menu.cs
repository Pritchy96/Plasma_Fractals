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
    public partial class Options_Menu : Form
    {
        Main_State mainState;

        public Options_Menu(Main_State mainState)
        {
            InitializeComponent();
            this.mainState = mainState;
        }

        private void btn_Clouds_CheckedChanged(object sender, EventArgs e)
        {
            mainState.Clouds_CheckedChanged(sender, e);
        }

        private void btn_Shade_CheckedChanged(object sender, EventArgs e)
        {
            mainState.Shade_CheckedChanged(sender, e);
        }

        private void btn_Colour_CheckedChanged(object sender, EventArgs e)
        {
            mainState.Colour_CheckedChanged(sender, e);
        }
    }
}
