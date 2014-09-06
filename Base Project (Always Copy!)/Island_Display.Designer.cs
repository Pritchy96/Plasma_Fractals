namespace Plasma_Fractal
{
    partial class Island_Display
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DrawScreen = new Plasma_Fractal.DBPanel();
            this.SuspendLayout();
            // 
            // DrawScreen
            // 
            this.DrawScreen.AutoScroll = true;
            this.DrawScreen.BackColor = System.Drawing.Color.White;
            this.DrawScreen.Location = new System.Drawing.Point(0, 0);
            this.DrawScreen.Name = "DrawScreen";
            this.DrawScreen.Size = new System.Drawing.Size(613, 456);
            this.DrawScreen.TabIndex = 0;
            this.DrawScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.Repaint);
            this.DrawScreen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClick);
            this.DrawScreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoved);
            // 
            // Island_Display
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(596, 442);
            this.Controls.Add(this.DrawScreen);
            this.Name = "Island_Display";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Island Generator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnExit);
            this.Move += new System.EventHandler(this.Island_Display_Move);
            this.Resize += new System.EventHandler(this.Island_Display_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        public DBPanel DrawScreen;



    }
}

