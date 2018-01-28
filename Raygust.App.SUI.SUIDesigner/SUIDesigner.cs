using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Raygust.App.SUI
{
    internal enum ControlSide
    {
        None,
        Top,
        Left,
        Right,
        Bottom
    }

    public partial class SUIDesigner : Form
    {
        public SUIDesigner()
        {
            InitializeComponent();
        }

        Control selectedControl = null;
        int selectedControlX;
        int selectedControlY;

        bool drag = false;
        bool resize = false;

        Point launchmenu;

        ControlSide horizontal = ControlSide.None;
        ControlSide vertical = ControlSide.None;

        private void Register_Control(Control control)
        {
            control.MouseDown += Control_MouseDown;
            control.MouseMove += Control_MouseMove;
            control.MouseUp += Control_MouseUp;
            control.PreviewKeyDown += Control_PreviewKeyDown;
            control.Paint += Control_Paint;
            control.MouseClick += Control_MouseClick;
        }

        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                launchmenu = Cursor.Position;
                this.ctxMenu.Show(Cursor.Position);
            }
        }

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Label)
            {
                Pen myPen = new Pen(Color.Black);
                myPen.DashPattern = new float[] { 2, 2 };
                Rectangle rect = new Rectangle(0, 0, e.ClipRectangle.Width - 2, e.ClipRectangle.Height - 2);
                e.Graphics.DrawRectangle(myPen, rect);
            }
        }

        private void Control_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (selectedControl != null)
                {
                    this.Controls.Remove(selectedControl);
                    selectedControl = null;
                }
            }
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
            resize = false;
            this.Cursor = Cursors.Default;
            this.Refresh();
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedControlX = e.X;
                selectedControlY = e.Y;
                selectedControl = (Control)sender;
                vertical = ControlSide.None;
                horizontal = ControlSide.None;

                Control myCtl;
                if (selectedControl.Parent == this)
                {
                    myCtl = this;
                }
                else
                {
                    myCtl = selectedControl.Parent;
                }

                if (myCtl.PointToClient(Cursor.Position).Y - selectedControl.Top <= 4)
                {
                    resize = true;
                    this.Cursor = Cursors.SizeNS;
                    vertical = ControlSide.Top;
                }
                else if (selectedControl.Top + selectedControl.Height - myCtl.PointToClient(Cursor.Position).Y <= 4)
                {
                    resize = true;
                    this.Cursor = Cursors.SizeNS;
                    vertical = ControlSide.Bottom;
                }

                if (myCtl.PointToClient(Cursor.Position).X - selectedControl.Left <= 4)
                {
                    resize = true;
                    this.Cursor = Cursors.SizeWE;
                    horizontal = ControlSide.Left;
                }
                else if (selectedControl.Left + selectedControl.Width - myCtl.PointToClient(Cursor.Position).X <= 4)
                {
                    resize = true;
                    this.Cursor = Cursors.SizeWE;
                    horizontal = ControlSide.Right;
                }

                if (!resize)
                {
                    drag = true;
                    this.Cursor = Cursors.Hand;
                }
                else if (horizontal == ControlSide.Left && vertical == ControlSide.Top)
                {
                    this.Cursor = Cursors.SizeNWSE;
                }
                else if (horizontal == ControlSide.Right && vertical == ControlSide.Top)
                {
                    this.Cursor = Cursors.SizeNESW;
                }
                else if (horizontal == ControlSide.Left && vertical == ControlSide.Bottom)
                {
                    this.Cursor = Cursors.SizeNESW;
                }
                else if (horizontal == ControlSide.Right && vertical == ControlSide.Bottom)
                {
                    this.Cursor = Cursors.SizeNWSE;
                }
            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            Control ctl = (Control)selectedControl;
            if (Control.MouseButtons != MouseButtons.Left)
            {
                drag = false;
                resize = false;
                this.Cursor = Cursors.Default;
            }
            if (drag && sender == selectedControl)
            {
                ctl.Left = (e.X + ctl.Left) - selectedControlX;
                ctl.Top = (e.Y + ctl.Top) - selectedControlY;
            }
            else if (drag && sender == selectedControl.Parent)
            {
                ctl.Left = e.X - selectedControlX;
                ctl.Top = e.Y - selectedControlY;
            }
            else if (resize && sender == selectedControl)
            {
                switch (vertical)
                {
                    case ControlSide.Bottom:
                        ctl.Height = e.Y;
                        break;
                    case ControlSide.Top:
                        int height = (selectedControlY - e.Y) + ctl.Height;
                        ctl.Top = (e.Y + ctl.Top) - selectedControlY;
                        ctl.Height = height;
                        break;
                }

                switch (horizontal)
                {
                    case ControlSide.Right:
                        ctl.Width = e.X;
                        break;
                    case ControlSide.Left:
                        int width = (selectedControlX - e.X) + ctl.Width;
                        ctl.Left = (e.X + ctl.Left) - selectedControlX;
                        ctl.Width = width;
                        break;
                }
            }

            else if (resize && sender != selectedControl)
            {
                switch (vertical)
                {
                    case ControlSide.Bottom:
                        ctl.Height = ctl.PointToClient(Cursor.Position).Y;
                        break;
                    case ControlSide.Top:
                        int height = (selectedControlY - ctl.PointToClient(Cursor.Position).Y) + ctl.Height;
                        ctl.Top = (ctl.PointToClient(Cursor.Position).Y + ctl.Top) - selectedControlY;
                        ctl.Height = height;
                        break;
                }

                switch (horizontal)
                {
                    case ControlSide.Right:
                        ctl.Width = ctl.PointToClient(Cursor.Position).X;
                        break;
                    case ControlSide.Left:
                        int width = (selectedControlX - ctl.PointToClient(Cursor.Position).X) + ctl.Width;
                        ctl.Left = (ctl.PointToClient(Cursor.Position).X + ctl.Left) - selectedControlX;
                        ctl.Width = width;
                        break;
                }
            }
        }

        private void SUIDesigner_Load(object sender, EventArgs e)
        {
            ToolStripItem item = null;

            item = newToolStripMenuItem.DropDownItems.Add("Label");
            item.Tag = "System.Windows.Forms.Label";
            item.Click += labelToolStripMenuItem_Click;
            item = newToolStripMenuItem.DropDownItems.Add("Textbox");
            item.Tag = "System.Windows.Forms.TextBox";
            item.Click += labelToolStripMenuItem_Click;
            item = newToolStripMenuItem.DropDownItems.Add("Button");
            item.Tag = "System.Windows.Forms.Button";
            item.Click += labelToolStripMenuItem_Click;
            item = newToolStripMenuItem.DropDownItems.Add("ComboBox");
            item.Tag = "System.Windows.Forms.ComboBox";
            item.Click += labelToolStripMenuItem_Click;
            item = newToolStripMenuItem.DropDownItems.Add("ListBox");
            item.Tag = "System.Windows.Forms.ListBox";
            item.Click += labelToolStripMenuItem_Click;
            item = newToolStripMenuItem.DropDownItems.Add("PictureBox");
            item.Tag = "System.Windows.Forms.PictureBox";
            item.Click += labelToolStripMenuItem_Click;
            item = newToolStripMenuItem.DropDownItems.Add("WebBrowser");
            item.Tag = "System.Windows.Forms.WebBrowser";
            item.Click += labelToolStripMenuItem_Click;
        }

        private void labelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (ToolStripItem)sender;

            Assembly asmg = Assembly.LoadWithPartialName("System.Windows.Forms");
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies().First(asm => asm.FullName.StartsWith("System.Windows.Forms,", StringComparison.CurrentCultureIgnoreCase));
            Control newControl = null;

            switch ((string)item.Tag)
            {
                case "System.Windows.Forms.WebBrowser":
                case "System.Windows.Forms.PictureBox":
                    newControl = (Control)Activator.CreateInstance(asmg.GetType("System.Windows.Forms.Label"));
                    newControl.BackColor = Color.White;
                    break;
                case "System.Windows.Forms.TextBox":
                    newControl = (Control)Activator.CreateInstance(asmg.GetType("System.Windows.Forms.TextBox"));
                    TextBox UITextbox = (TextBox)newControl;
                    UITextbox.ReadOnly = true;
                    UITextbox.BackColor = Color.White;
                    break;
                default:
                    newControl = (Control)Activator.CreateInstance(asmg.GetType((string)item.Tag));
                    break;
            }

            newControl.Top = this.PointToClient(launchmenu).Y;
            newControl.Left = this.PointToClient(launchmenu).X;
            newControl.Text = item.Text;

            Register_Control(newControl);
            this.Controls.Add(newControl);
        }

        private void SUIDesigner_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                launchmenu = Cursor.Position;
                this.ctxMenu.Show(Cursor.Position);
            }
        }
    }
}
