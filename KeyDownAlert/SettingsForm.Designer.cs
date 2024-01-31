namespace KeyDownAlert
{
    partial class SettingsForm
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
            this.DiameterTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NotPressedATextBox = new System.Windows.Forms.TextBox();
            this.NotPressedRTextBox = new System.Windows.Forms.TextBox();
            this.NotPressedGTextBox = new System.Windows.Forms.TextBox();
            this.NotPressedBTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.PressedBTextBox = new System.Windows.Forms.TextBox();
            this.PressedGTextBox = new System.Windows.Forms.TextBox();
            this.PressedRTextBox = new System.Windows.Forms.TextBox();
            this.PressedATextBox = new System.Windows.Forms.TextBox();
            this.DefaultsButton = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.SideMouse1ComboBox = new System.Windows.Forms.ComboBox();
            this.SideMouse2ComboBox = new System.Windows.Forms.ComboBox();
            this.MiddleMouseComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // DiameterTextBox
            // 
            this.DiameterTextBox.Location = new System.Drawing.Point(399, 139);
            this.DiameterTextBox.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.DiameterTextBox.Name = "DiameterTextBox";
            this.DiameterTextBox.Size = new System.Drawing.Size(112, 44);
            this.DiameterTextBox.TabIndex = 0;
            this.DiameterTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DiameterTextBox_KeyDown);
            this.DiameterTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DiameterTextBox_KeyUp);
            this.DiameterTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DiameterTextBox_MouseDown);
            this.DiameterTextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DiameterTextBox_MouseMove);
            this.DiameterTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DiameterTextBox_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(133, 151);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 37);
            this.label1.TabIndex = 1;
            this.label1.Text = "Circle Diameter";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(120, 259);
            this.label2.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(388, 37);
            this.label2.TabIndex = 3;
            this.label2.Text = "Color No Buttons Pressed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 458);
            this.label3.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(337, 37);
            this.label3.TabIndex = 4;
            this.label3.Text = "Color Buttons Pressed";
            // 
            // NotPressedATextBox
            // 
            this.NotPressedATextBox.Location = new System.Drawing.Point(76, 359);
            this.NotPressedATextBox.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.NotPressedATextBox.Name = "NotPressedATextBox";
            this.NotPressedATextBox.Size = new System.Drawing.Size(112, 44);
            this.NotPressedATextBox.TabIndex = 5;
            this.NotPressedATextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NotPressedTextBox_KeyDown);
            this.NotPressedATextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseDown);
            this.NotPressedATextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseMove);
            this.NotPressedATextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseUp);
            // 
            // NotPressedRTextBox
            // 
            this.NotPressedRTextBox.Location = new System.Drawing.Point(215, 359);
            this.NotPressedRTextBox.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.NotPressedRTextBox.Name = "NotPressedRTextBox";
            this.NotPressedRTextBox.Size = new System.Drawing.Size(112, 44);
            this.NotPressedRTextBox.TabIndex = 6;
            this.NotPressedRTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NotPressedTextBox_KeyDown);
            this.NotPressedRTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseDown);
            this.NotPressedRTextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseMove);
            this.NotPressedRTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseUp);
            // 
            // NotPressedGTextBox
            // 
            this.NotPressedGTextBox.Location = new System.Drawing.Point(355, 359);
            this.NotPressedGTextBox.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.NotPressedGTextBox.Name = "NotPressedGTextBox";
            this.NotPressedGTextBox.Size = new System.Drawing.Size(112, 44);
            this.NotPressedGTextBox.TabIndex = 7;
            this.NotPressedGTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NotPressedTextBox_KeyDown);
            this.NotPressedGTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseDown);
            this.NotPressedGTextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseMove);
            this.NotPressedGTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseUp);
            // 
            // NotPressedBTextBox
            // 
            this.NotPressedBTextBox.Location = new System.Drawing.Point(494, 359);
            this.NotPressedBTextBox.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.NotPressedBTextBox.Name = "NotPressedBTextBox";
            this.NotPressedBTextBox.Size = new System.Drawing.Size(112, 44);
            this.NotPressedBTextBox.TabIndex = 8;
            this.NotPressedBTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NotPressedTextBox_KeyDown);
            this.NotPressedBTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseDown);
            this.NotPressedBTextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseMove);
            this.NotPressedBTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(120, 313);
            this.label4.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 37);
            this.label4.TabIndex = 9;
            this.label4.Text = "A";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(253, 313);
            this.label5.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 37);
            this.label5.TabIndex = 10;
            this.label5.Text = "R";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(390, 313);
            this.label6.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 37);
            this.label6.TabIndex = 11;
            this.label6.Text = "G";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(529, 313);
            this.label7.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 37);
            this.label7.TabIndex = 12;
            this.label7.Text = "B";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(529, 521);
            this.label8.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 37);
            this.label8.TabIndex = 20;
            this.label8.Text = "B";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(390, 521);
            this.label9.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 37);
            this.label9.TabIndex = 19;
            this.label9.Text = "G";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(253, 521);
            this.label10.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 37);
            this.label10.TabIndex = 18;
            this.label10.Text = "R";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(120, 521);
            this.label11.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(39, 37);
            this.label11.TabIndex = 17;
            this.label11.Text = "A";
            // 
            // PressedBTextBox
            // 
            this.PressedBTextBox.Location = new System.Drawing.Point(494, 566);
            this.PressedBTextBox.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.PressedBTextBox.Name = "PressedBTextBox";
            this.PressedBTextBox.Size = new System.Drawing.Size(112, 44);
            this.PressedBTextBox.TabIndex = 16;
            this.PressedBTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PressedTextBox_KeyDown);
            this.PressedBTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseDown);
            this.PressedBTextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseMove);
            this.PressedBTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseUp);
            // 
            // PressedGTextBox
            // 
            this.PressedGTextBox.Location = new System.Drawing.Point(355, 566);
            this.PressedGTextBox.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.PressedGTextBox.Name = "PressedGTextBox";
            this.PressedGTextBox.Size = new System.Drawing.Size(112, 44);
            this.PressedGTextBox.TabIndex = 15;
            this.PressedGTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PressedTextBox_KeyDown);
            this.PressedGTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseDown);
            this.PressedGTextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseMove);
            this.PressedGTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseUp);
            // 
            // PressedRTextBox
            // 
            this.PressedRTextBox.Location = new System.Drawing.Point(215, 566);
            this.PressedRTextBox.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.PressedRTextBox.Name = "PressedRTextBox";
            this.PressedRTextBox.Size = new System.Drawing.Size(112, 44);
            this.PressedRTextBox.TabIndex = 14;
            this.PressedRTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PressedTextBox_KeyDown);
            this.PressedRTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseDown);
            this.PressedRTextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseMove);
            this.PressedRTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseUp);
            // 
            // PressedATextBox
            // 
            this.PressedATextBox.Location = new System.Drawing.Point(76, 566);
            this.PressedATextBox.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.PressedATextBox.Name = "PressedATextBox";
            this.PressedATextBox.Size = new System.Drawing.Size(112, 44);
            this.PressedATextBox.TabIndex = 13;
            this.PressedATextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PressedTextBox_KeyDown);
            this.PressedATextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseDown);
            this.PressedATextBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseMove);
            this.PressedATextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BothTextBoxes_MouseUp);
            // 
            // DefaultsButton
            // 
            this.DefaultsButton.Location = new System.Drawing.Point(215, 34);
            this.DefaultsButton.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.DefaultsButton.Name = "DefaultsButton";
            this.DefaultsButton.Size = new System.Drawing.Size(174, 54);
            this.DefaultsButton.TabIndex = 21;
            this.DefaultsButton.Text = "Defaults";
            this.DefaultsButton.UseVisualStyleBackColor = true;
            this.DefaultsButton.Click += new System.EventHandler(this.DefaultsButton_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(120, 672);
            this.label12.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(399, 37);
            this.label12.TabIndex = 22;
            this.label12.Text = "Tip: Click and Drag to +/- 1";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(835, 151);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(266, 37);
            this.label13.TabIndex = 23;
            this.label13.Text = "Side Mouse Btn 1";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(835, 318);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(268, 37);
            this.label14.TabIndex = 24;
            this.label14.Text = "Side Mouse Btn 2";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(832, 517);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(271, 37);
            this.label15.TabIndex = 25;
            this.label15.Text = "Middle Mouse Btn";
            // 
            // SideMouse1ComboBox
            // 
            this.SideMouse1ComboBox.FormattingEnabled = true;
            this.SideMouse1ComboBox.Location = new System.Drawing.Point(826, 202);
            this.SideMouse1ComboBox.Name = "SideMouse1ComboBox";
            this.SideMouse1ComboBox.Size = new System.Drawing.Size(288, 45);
            this.SideMouse1ComboBox.TabIndex = 26;
            this.SideMouse1ComboBox.SelectedIndexChanged += new System.EventHandler(this.AllComboBoxes_SelectedIndexChanged);
            // 
            // SideMouse2ComboBox
            // 
            this.SideMouse2ComboBox.FormattingEnabled = true;
            this.SideMouse2ComboBox.Location = new System.Drawing.Point(826, 369);
            this.SideMouse2ComboBox.Name = "SideMouse2ComboBox";
            this.SideMouse2ComboBox.Size = new System.Drawing.Size(288, 45);
            this.SideMouse2ComboBox.TabIndex = 27;
            this.SideMouse2ComboBox.SelectedIndexChanged += new System.EventHandler(this.AllComboBoxes_SelectedIndexChanged);
            // 
            // MiddleMouseComboBox
            // 
            this.MiddleMouseComboBox.FormattingEnabled = true;
            this.MiddleMouseComboBox.Location = new System.Drawing.Point(826, 580);
            this.MiddleMouseComboBox.Name = "MiddleMouseComboBox";
            this.MiddleMouseComboBox.Size = new System.Drawing.Size(288, 45);
            this.MiddleMouseComboBox.TabIndex = 28;
            this.MiddleMouseComboBox.SelectedIndexChanged += new System.EventHandler(this.AllComboBoxes_SelectedIndexChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(19F, 37F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1557, 720);
            this.Controls.Add(this.MiddleMouseComboBox);
            this.Controls.Add(this.SideMouse2ComboBox);
            this.Controls.Add(this.SideMouse1ComboBox);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.DefaultsButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.PressedBTextBox);
            this.Controls.Add(this.PressedGTextBox);
            this.Controls.Add(this.PressedRTextBox);
            this.Controls.Add(this.PressedATextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NotPressedBTextBox);
            this.Controls.Add(this.NotPressedGTextBox);
            this.Controls.Add(this.NotPressedRTextBox);
            this.Controls.Add(this.NotPressedATextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DiameterTextBox);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DiameterTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox NotPressedATextBox;
        private System.Windows.Forms.TextBox NotPressedRTextBox;
        private System.Windows.Forms.TextBox NotPressedGTextBox;
        private System.Windows.Forms.TextBox NotPressedBTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox PressedBTextBox;
        private System.Windows.Forms.TextBox PressedGTextBox;
        private System.Windows.Forms.TextBox PressedRTextBox;
        private System.Windows.Forms.TextBox PressedATextBox;
        private System.Windows.Forms.Button DefaultsButton;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox SideMouse1ComboBox;
        private System.Windows.Forms.ComboBox SideMouse2ComboBox;
        private System.Windows.Forms.ComboBox MiddleMouseComboBox;
    }
}