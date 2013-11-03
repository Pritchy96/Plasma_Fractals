using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plasma_Fractal
{
    class Manager
    {
        private MainState currentState;

        public MainState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        public Manager()
        {
            currentState = new MainState(Screen.width, Screen.height);
        }

        public void MouseMoved(MouseEventArgs e)
        {
            currentState.MouseMoved(e);
        }

        public void MouseClicked(MouseEventArgs e)
        {
            currentState.MouseClicked(e);
        }

        public void Update()
        {
            currentState.Update();
        }

        public void Redraw(PaintEventArgs e)
        {
            currentState.Redraw(e);
        }

    }
}

        
