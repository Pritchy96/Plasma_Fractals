namespace Plasma_Fractal
{
    partial class Options_Menu
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
            this.btn_Clouds = new System.Windows.Forms.CheckBox();
            this.btn_Shade = new System.Windows.Forms.CheckBox();
            this.btn_Colour = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btn_Clouds
            // 
            this.btn_Clouds.AutoSize = true;
            this.btn_Clouds.Location = new System.Drawing.Point(12, 55);
            this.btn_Clouds.Name = "btn_Clouds";
            this.btn_Clouds.Size = new System.Drawing.Size(58, 17);
            this.btn_Clouds.TabIndex = 8;
            this.btn_Clouds.Text = "Clouds";
            this.btn_Clouds.UseVisualStyleBackColor = true;
            this.btn_Clouds.CheckedChanged += new System.EventHandler(this.btn_Clouds_CheckedChanged);
            // 
            // btn_Shade
            // 
            this.btn_Shade.AutoSize = true;
            this.btn_Shade.Location = new System.Drawing.Point(12, 32);
            this.btn_Shade.Name = "btn_Shade";
            this.btn_Shade.Size = new System.Drawing.Size(57, 17);
            this.btn_Shade.TabIndex = 7;
            this.btn_Shade.Text = "Shade";
            this.btn_Shade.UseVisualStyleBackColor = true;
            this.btn_Shade.CheckedChanged += new System.EventHandler(this.btn_Shade_CheckedChanged);
            // 
            // btn_Colour
            // 
            this.btn_Colour.AutoSize = true;
            this.btn_Colour.Location = new System.Drawing.Point(12, 9);
            this.btn_Colour.Name = "btn_Colour";
            this.btn_Colour.Size = new System.Drawing.Size(50, 17);
            this.btn_Colour.TabIndex = 6;
            this.btn_Colour.Text = "Color";
            this.btn_Colour.UseVisualStyleBackColor = true;
            this.btn_Colour.CheckedChanged += new System.EventHandler(this.btn_Colour_CheckedChanged);
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(101, 237);
            this.Controls.Add(this.btn_Clouds);
            this.Controls.Add(this.btn_Shade);
            this.Controls.Add(this.btn_Colour);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.Text = "Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox btn_Clouds;
        private System.Windows.Forms.CheckBox btn_Shade;
        private System.Windows.Forms.CheckBox btn_Colour;
    }
}