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

        public int Diameter { get; set; }
        public Color PressedColor { get; set; }
        public Color NotPressedColor { get; set; }
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


        private void CloseForm()
        {
            if (int.TryParse(DiameterTextBox.Text, out int result) && result > 0)
            {
                Diameter = result;
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter a positive integer.");
            }
        }
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            SetTextBoxes();
        }
        private void SetTextBoxes()
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
        }
        private int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void NotPressedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateNotPressed();
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
        private void PressedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            UpdatePressed();
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
        private void NotPressedTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = e.Location;
                (sender as TextBox).Cursor = Cursors.Hand;
            }
        }
        private void NotPressedTextBox_MouseMove(object sender, MouseEventArgs e)
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
                    }
                }

                dragStartPoint = e.Location;
            }
        }
        private void NotPressedTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                (sender as TextBox).Cursor = Cursors.IBeam;
                UpdateNotPressed();
                UpdatePressed();
            }
        }

        private void UpdateDiameter()
        {
            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            form1.SetDiameter(Diameter);
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

        private void DefaultsButton_Click(object sender, EventArgs e)
        {
            PressedColor = Color.Red;
            NotPressedColor = Color.Green;
            Diameter = 100;
            SetTextBoxes();
            UpdatePressed();
            UpdateNotPressed();
            UpdateDiameter();
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
    }
}
