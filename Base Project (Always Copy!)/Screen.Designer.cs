namespace Plasma_Fractal
{
    partial class Screen
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
            this.btn_Colour = new System.Windows.Forms.CheckBox();
            this.btn_Shade = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DrawScreen = new Plasma_Fractal.DBPanel();
            this.btnShade = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Colour
            // 
            this.btn_Colour.AutoSize = true;
            this.btn_Colour.Location = new System.Drawing.Point(710, 12);
            this.btn_Colour.Name = "btn_Colour";
            this.btn_Colour.Size = new System.Drawing.Size(50, 17);
            this.btn_Colour.TabIndex = 2;
            this.btn_Colour.Text = "Color";
            this.btn_Colour.UseVisualStyleBackColor = true;
            this.btn_Colour.CheckedChanged += new System.EventHandler(this.btn_Colour_CheckedChanged);
            // 
            // btn_Shade
            // 
            this.btn_Shade.AutoSize = true;
            this.btn_Shade.Location = new System.Drawing.Point(710, 35);
            this.btn_Shade.Name = "btn_Shade";
            this.btn_Shade.Size = new System.Drawing.Size(57, 17);
            this.btn_Shade.TabIndex = 3;
            this.btn_Shade.Text = "Shade";
            this.btn_Shade.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.DrawScreen);
            this.panel1.Location = new System.Drawing.Point(-3, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(707, 562);
            this.panel1.TabIndex = 4;
            // 
            // DrawScreen
            // 
            this.DrawScreen.AutoScroll = true;
            this.DrawScreen.BackColor = System.Drawing.Color.White;
            this.DrawScreen.Location = new System.Drawing.Point(3, 3);
            this.DrawScreen.Name = "DrawScreen";
            this.DrawScreen.Size = new System.Drawing.Size(476, 302);
            this.DrawScreen.TabIndex = 0;
            this.DrawScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.Redraw);
            this.DrawScreen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClick);
            this.DrawScreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoved);
            // 
            // btnShade
            // 
            this.btnShade.AutoSize = true;
            this.btnShade.Location = new System.Drawing.Point(710, 58);
            this.btnShade.Name = "btnShade";
            this.btnShade.Size = new System.Drawing.Size(58, 17);
            this.btnShade.TabIndex = 5;
            this.btnShade.Text = "Clouds";
            this.btnShade.UseVisualStyleBackColor = true;
            this.btnShade.CheckedChanged += new System.EventHandler(this.btnShade_CheckedChanged);
            // 
            // Screen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.btnShade);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btn_Shade);
            this.Controls.Add(this.btn_Colour);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Screen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Plasma Fractal Map Generator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnExit);
            this.Load += new System.EventHandler(this.Screen_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DBPanel DrawScreen;
        private System.Windows.Forms.CheckBox btn_Colour;
        private System.Windows.Forms.CheckBox btn_Shade;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox btnShade;


    }
}

