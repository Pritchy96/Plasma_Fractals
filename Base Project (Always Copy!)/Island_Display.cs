using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plasma_Fractal
{
    public partial class Island_Display : Form
    {
        //Screen size.
        public static int width = 600;
        public static int height = 600;

        //Thread Variables.
        Boolean Running = false;
        Thread thread = null;

        Main_State state;

        #region Function Explanation
        //Constructor, sets Screen size and then begins Thread.
        #endregion
        public Island_Display()
        {
            InitializeComponent();
           // DrawScreen.Width = width;
          //  DrawScreen.Height = height;
            //this.TopMost = true;
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            state = new Main_State(width, height, this);
            BeginThread();
        }

        #region Function Explanation
        //Exit Event, kills Thread on Window close.
        #endregion
        private void OnExit(object sender, FormClosingEventArgs e)
        {
            killThread();
        }

        private void MouseClick(object sender, MouseEventArgs e)
        {
            state.MouseClicked(e);
        }

        private void MouseMoved(object sender, MouseEventArgs e)
        {
            state.MouseMoved(e);
        }

        private void Redraw(object sender, PaintEventArgs e)
        {
            state.Redraw(e);
        }

        #region Function Explanation
        //Creates and starts a Thread.
        #endregion
        public void BeginThread()
        {
            thread = new Thread(new ThreadStart(Update));
            thread.Start();
            Running = true;
        }

        #region Function Explanation
        //Kills the thread.
        #endregion
        public void killThread()
        {
            //Simply kills off the Thread.
            Running = false;
            thread.Abort();
            thread.Join();
        }

        #region Function Explanation
        //The main Update loop. Basically just updates Manager which handles
        //all Game updates.
        #endregion
        public void Update()
        {
            while (Running)
            {
                state.Update();
                
                //Cause screen to redraw.
                DrawScreen.Invalidate();

                //Basic Thread Slowing.
                Thread.Sleep(12);
            }
        }

        #region Function Explanation
        //Repaints Manager.
        #endregion
        private void Repaint(object sender, PaintEventArgs e)
        {
            state.Redraw(e);
        }

        private void Screen_Load(object sender, EventArgs e)
        {

        }

        private void btn_Colour_CheckedChanged(object sender, EventArgs e)
        {
            state.Colour_CheckedChanged(sender, e);
        }

        private void btnShade_CheckedChanged(object sender, EventArgs e)
        {
            state.Shade_CheckedChanged(sender, e);
        }

        private void btn_Clouds_CheckedChanged(object sender, EventArgs e)
        {
            state.Clouds_CheckedChanged(sender, e);
        }


    }
}
