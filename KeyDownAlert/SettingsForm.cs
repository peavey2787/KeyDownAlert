using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyDownAlert
{
    public partial class SettingsForm : Form
    {
        private bool isDragging = false;
        private Point dragStartPoint;
        private int dragStep = 1;
        private bool appHasControl = false;

        public int Diameter { get; set; }
        public Color PressedColor { get; set; }
        public Color NotPressedColor { get; set; }
        public Buttons Buttons { get; set; }
        public string GameExeName { get; set; }
        public SettingsForm()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            // Draw small circles at the top left and right corners
            int smallCircleDiameter = 20;
            int spacing = 10;
            int leftX = spacing;
            int rightX = Width - smallCircleDiameter - spacing * 3;
            int topY = spacing;

            using (SolidBrush notPressedBrush = new SolidBrush(NotPressedColor))
            {
                g.FillEllipse(notPressedBrush, new Rectangle(leftX, topY, smallCircleDiameter, smallCircleDiameter));
            }

            using (SolidBrush pressedBrush = new SolidBrush(PressedColor))
            {
                g.FillEllipse(pressedBrush, new Rectangle(rightX, topY, smallCircleDiameter, smallCircleDiameter));
            }
        }


        private void SettingsForm_Load(object sender, EventArgs e)
        {
            SetGUI();
        }



        #region GUI Updates
        // GUI updates
        private void SetGUI()
        {
            DiameterTextBox.Text = Diameter.ToString();
            NotPressedATextBox.Text = NotPressedColor.A.ToString();
            NotPressedRTextBox.Text = NotPressedColor.R.ToString();
            NotPressedGTextBox.Text = NotPressedColor.G.ToString();
            NotPressedBTextBox.Text = NotPressedColor.B.ToString();

            PressedATextBox.Text = PressedColor.A.ToString();
            PressedRTextBox.Text = PressedColor.R.ToString();
            PressedGTextBox.Text = PressedColor.G.ToString();
            PressedBTextBox.Text = PressedColor.B.ToString();

            GameExeNameTextBox.Text = GameExeName;

            /*
                int sideMouseButton1 = 131072;
                int sideMouseButton2 = 65536;
                int sideMouseButtonUp = 524;
                int sideMouseButtonDown = 523;

                string keyboard = "keyboard";
                string lMouseButton = "LMB";
                string rMouseButton = "RMB";
                string mMouseButton = "MMB";
             */
            // Set default buttons for comboboxes
            Buttons defaultButtons = new Buttons();
            Button defaultButton = new Button();
            defaultButton.Id = "131072";
            defaultButton.Name = "SideMouse1";
            defaultButton.Action = "AutoCraft";
            defaultButtons.ButtonList.Add(defaultButton);

            defaultButton = new Button();
            defaultButton.Id = "65536";
            defaultButton.Name = "SideMouse2";
            defaultButton.Action = "AutoHarvest";
            defaultButtons.ButtonList.Add(defaultButton);

            defaultButton = new Button();
            defaultButton.Id = "MMB";
            defaultButton.Name = "MiddleMouse";
            defaultButton.Action = "AutoRun";
            defaultButtons.ButtonList.Add(defaultButton);

            // Set selected options
            appHasControl = true;
            foreach (Button button in Buttons.ButtonList)
            {
                Control[] matchingControls = Controls.Find(button.Name + "ComboBox", true);

                if (matchingControls.Length > 0 && matchingControls[0] is ComboBox matchingComboBox)
                {
                    // Add default buttons to options
                    Button selectedButton = new Button();
                    foreach (Button buttonOption in defaultButtons.ButtonList)
                    {
                        matchingComboBox.Items.Add(buttonOption);

                        if(buttonOption.Action == button.Action)
                        {
                            selectedButton = buttonOption;
                        }
                    }
                     
                    matchingComboBox.SelectedItem = selectedButton;
                }
            }
            appHasControl = false;
        }

        private void UpdateDiameter()
        {
            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            form1.SetDiameter(Diameter);
        }
        private void UpdatePressed()
        {
            int a, r, g, b;

            if (int.TryParse(PressedATextBox.Text, out a) &&
                int.TryParse(PressedRTextBox.Text, out r) &&
                int.TryParse(PressedGTextBox.Text, out g) &&
                int.TryParse(PressedBTextBox.Text, out b))
            {
                // Ensure the values are within the valid range
                a = Clamp(a, 0, 255);
                r = Clamp(r, 0, 255);
                g = Clamp(g, 0, 255);
                b = Clamp(b, 0, 255);

                PressedColor = Color.FromArgb(a, r, g, b);

                Invalidate();
            }

            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            form1.SetPressedColor(PressedColor);
        }
        private void UpdateNotPressed()
        {
            int a, r, g, b;

            if (int.TryParse(NotPressedATextBox.Text, out a) &&
                int.TryParse(NotPressedRTextBox.Text, out r) &&
                int.TryParse(NotPressedGTextBox.Text, out g) &&
                int.TryParse(NotPressedBTextBox.Text, out b))
            {
                // Ensure the values are within the valid range
                a = Clamp(a, 0, 255);
                r = Clamp(r, 0, 255);
                g = Clamp(g, 0, 255);
                b = Clamp(b, 0, 255);

                NotPressedColor = Color.FromArgb(a, r, g, b);

                Invalidate();
            }
            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            form1.SetNotPressedColor(NotPressedColor);
            form1.SetDiameter(Diameter);
        }
        #endregion


        #region Mouse Events
        // Mouse events
        private void BothTextBoxes_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = e.Location;
                (sender as TextBox).Cursor = Cursors.Hand;
            }
        }
        private void BothTextBoxes_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - dragStartPoint.X;
                int sign = Math.Sign(deltaX);

                if (sign != 0)
                {
                    int currentValue;
                    if (int.TryParse((sender as TextBox).Text, out currentValue))
                    {
                        // Use the Clamp function to keep the value within the range of 0 and 255
                        currentValue = Clamp(currentValue + sign * dragStep, 0, 255);

                        (sender as TextBox).Text = currentValue.ToString();

                        UpdateNotPressed();
                        UpdatePressed();
                        UpdateDiameter();
                    }
                }

                dragStartPoint = e.Location;
            }
        }
        private void BothTextBoxes_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                (sender as TextBox).Cursor = Cursors.IBeam;
                UpdateNotPressed();
                UpdatePressed();
            }
        }


        private void DiameterTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = e.Location;
                (sender as TextBox).Cursor = Cursors.Hand;
            }
        }
        private void DiameterTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - dragStartPoint.X;
                int sign = Math.Sign(deltaX);

                if (sign != 0)
                {
                    int currentValue;
                    if (int.TryParse((sender as TextBox).Text, out currentValue))
                    {
                        // Use the Clamp function to keep the value within the range of 0 and 255
                        currentValue = Clamp(currentValue + sign * dragStep, 0, 5555);

                        (sender as TextBox).Text = currentValue.ToString();
                        Diameter = currentValue;
                        UpdateDiameter();
                    }
                }
                dragStartPoint = e.Location;
            }
        }
        private void DiameterTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                (sender as TextBox).Cursor = Cursors.IBeam;
            }
        }
        #endregion


        #region Key Down/Up
        // Key down
        private void PressedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            UpdatePressed();
        }
        private void NotPressedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateNotPressed();
        }


        private void DiameterTextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void DiameterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (int.TryParse(DiameterTextBox.Text, out int diameter) && diameter > 0)
            {
                Diameter = diameter;
            }
        }
        #endregion


        // Buttons
        private void DefaultsButton_Click(object sender, EventArgs e)
        {
            PressedColor = Color.Red;
            NotPressedColor = Color.Green;
            Diameter = 100;
            SetGUI();
            UpdatePressed();
            UpdateNotPressed();
            UpdateDiameter();
        }


        #region Comboboxes
        // Comboboxes
        private void AllComboBoxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!appHasControl && sender is ComboBox comboBox)
            {    
                Button matchingButton = Buttons.ButtonList.Find(b => b.Name.Equals(comboBox.Name.Replace("ComboBox", "")));
                matchingButton.Action = comboBox.Text;
            }
        }
        #endregion


        // Helpers
        private int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void GameExeNameTextBox_TextChanged(object sender, EventArgs e)
        {
            string newVal = GameExeNameTextBox.Text;
            if(newVal.Contains(".exe"))            
                newVal = newVal.Replace(".exe", "");
            if (newVal.Contains("exe"))
                newVal = newVal.Replace("exe", "");
            GameExeName = GameExeNameTextBox.Text;
        }
    }
}
