namespace Plasma_Fractal
{
    partial class Gen_Menu
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
            this.btn_Generate = new System.Windows.Forms.Button();
            this.txt_Width = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Height = new System.Windows.Forms.TextBox();
            this.chk_Coloured = new System.Windows.Forms.CheckBox();
            this.chk_Shaded = new System.Windows.Forms.CheckBox();
            this.chk_Noise = new System.Windows.Forms.CheckBox();
            this.chk_Linked = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBaseRough = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtShaderRough = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtRivers = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_Generate
            // 
            this.btn_Generate.Location = new System.Drawing.Point(26, 274);
            this.btn_Generate.Name = "btn_Generate";
            this.btn_Generate.Size = new System.Drawing.Size(75, 23);
            this.btn_Generate.TabIndex = 0;
            this.btn_Generate.Text = "Generate";
            this.btn_Generate.UseVisualStyleBackColor = true;
            this.btn_Generate.Click += new System.EventHandler(this.btn_Generate_Click);
            // 
            // txt_Width
            // 
            this.txt_Width.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_Width.Location = new System.Drawing.Point(55, 9);
            this.txt_Width.Name = "txt_Width";
            this.txt_Width.Size = new System.Drawing.Size(46, 13);
            this.txt_Width.TabIndex = 1;
            this.txt_Width.Text = "600";
            this.txt_Width.TextChanged += new System.EventHandler(this.txt_Width_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Width:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Height:";
            // 
            // txt_Height
            // 
            this.txt_Height.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_Height.Location = new System.Drawing.Point(55, 32);
            this.txt_Height.Name = "txt_Height";
            this.txt_Height.Size = new System.Drawing.Size(46, 13);
            this.txt_Height.TabIndex = 3;
            this.txt_Height.Text = "600";
            this.txt_Height.TextChanged += new System.EventHandler(this.txt_Height_TextChanged);
            // 
            // chk_Coloured
            // 
            this.chk_Coloured.AutoSize = true;
            this.chk_Coloured.Checked = true;
            this.chk_Coloured.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Coloured.Location = new System.Drawing.Point(26, 88);
            this.chk_Coloured.Name = "chk_Coloured";
            this.chk_Coloured.Size = new System.Drawing.Size(68, 17);
            this.chk_Coloured.TabIndex = 5;
            this.chk_Coloured.Text = "Coloured";
            this.chk_Coloured.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chk_Coloured.UseVisualStyleBackColor = true;
            // 
            // chk_Shaded
            // 
            this.chk_Shaded.AutoSize = true;
            this.chk_Shaded.Checked = true;
            this.chk_Shaded.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Shaded.Location = new System.Drawing.Point(26, 111);
            this.chk_Shaded.Name = "chk_Shaded";
            this.chk_Shaded.Size = new System.Drawing.Size(63, 17);
            this.chk_Shaded.TabIndex = 6;
            this.chk_Shaded.Text = "Shaded";
            this.chk_Shaded.UseVisualStyleBackColor = true;
            // 
            // chk_Noise
            // 
            this.chk_Noise.AutoSize = true;
            this.chk_Noise.Checked = true;
            this.chk_Noise.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Noise.Location = new System.Drawing.Point(26, 134);
            this.chk_Noise.Name = "chk_Noise";
            this.chk_Noise.Size = new System.Drawing.Size(53, 17);
            this.chk_Noise.TabIndex = 7;
            this.chk_Noise.Text = "Noise";
            this.chk_Noise.UseVisualStyleBackColor = true;
            // 
            // chk_Linked
            // 
            this.chk_Linked.AutoSize = true;
            this.chk_Linked.Checked = true;
            this.chk_Linked.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Linked.Location = new System.Drawing.Point(26, 51);
            this.chk_Linked.Name = "chk_Linked";
            this.chk_Linked.Size = new System.Drawing.Size(58, 17);
            this.chk_Linked.TabIndex = 8;
            this.chk_Linked.Text = "Linked";
            this.chk_Linked.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chk_Linked.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Base Roughness:";
            // 
            // txtBaseRough
            // 
            this.txtBaseRough.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBaseRough.Location = new System.Drawing.Point(38, 223);
            this.txtBaseRough.Name = "txtBaseRough";
            this.txtBaseRough.Size = new System.Drawing.Size(46, 13);
            this.txtBaseRough.TabIndex = 9;
            this.txtBaseRough.Text = "24";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 239);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Shader Roughness:";
            // 
            // txtShaderRough
            // 
            this.txtShaderRough.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtShaderRough.Location = new System.Drawing.Point(38, 255);
            this.txtShaderRough.Name = "txtShaderRough";
            this.txtShaderRough.Size = new System.Drawing.Size(46, 13);
            this.txtShaderRough.TabIndex = 11;
            this.txtShaderRough.Text = "18";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "How many Rivers:";
            // 
            // txtRivers
            // 
            this.txtRivers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRivers.Location = new System.Drawing.Point(36, 183);
            this.txtRivers.Name = "txtRivers";
            this.txtRivers.Size = new System.Drawing.Size(46, 13);
            this.txtRivers.TabIndex = 13;
            this.txtRivers.Text = "4";
            this.txtRivers.TextChanged += new System.EventHandler(this.txtRivers_TextChanged);
            // 
            // Gen_Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(122, 300);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtRivers);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtShaderRough);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBaseRough);
            this.Controls.Add(this.chk_Linked);
            this.Controls.Add(this.chk_Noise);
            this.Controls.Add(this.chk_Shaded);
            this.Controls.Add(this.chk_Coloured);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_Height);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_Width);
            this.Controls.Add(this.btn_Generate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Gen_Menu";
            this.Text = "Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Generate;
        private System.Windows.Forms.TextBox txt_Width;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Height;
        private System.Windows.Forms.CheckBox chk_Coloured;
        private System.Windows.Forms.CheckBox chk_Shaded;
        private System.Windows.Forms.CheckBox chk_Noise;
        private System.Windows.Forms.CheckBox chk_Linked;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBaseRough;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtShaderRough;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtRivers;

    }
}