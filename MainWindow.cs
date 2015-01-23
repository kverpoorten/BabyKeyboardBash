using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;


namespace BabyKeyboardBash
{
    public partial class MainWindow : Form
    {
        //Store an instance of DrawShapes
        private DrawShapes ds;
        private SoundPlayer sp;
        private KeyboardHook.Parameters param;

        public MainWindow(KeyboardHook.Parameters param)
        {
            this.param = param;
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            //Create an instance of DrawShapes, passing the form
            //to draw on and the settings from the config file
            string setting = Program.appSettings["DisplayConfig"];

            StringBuilder helpText = new StringBuilder();
            helpText.Append("Click here to empty the screen and start over");
            
            switch (param)
            {
                case KeyboardHook.Parameters.AllowAltF4:
                    helpText.Append(" - Press ALT+F4 to escape");
                    break;
                case KeyboardHook.Parameters.AllowAltTab:
                    helpText.Append(" - Press ALT+Tab to escape");
                    break;
                case KeyboardHook.Parameters.AllowAltTabAndWindows:
                    helpText.Append(" - Press ALT+Tab or Windows Key to escape");
                    break;
                case KeyboardHook.Parameters.AllowWindowsKey:
                    helpText.Append(" - Press Windows Key to escape");
                    break;
            }
            this.lblClear.Text = helpText.ToString();

            ds = new DrawShapes(this, CenterTextPanel, setting);

            var appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            sp = new SoundPlayer(Path.Combine(appDir, "Sounds/Alphabet/"), Path.Combine(appDir, "Sounds/Random/"));

            //Set up the event handler for the KeyboardHook's
            //KeyIntercepted event
            Program.kh.KeyIntercepted += new KeyboardHook.KeyboardHookEventHandler(kh_KeyIntercepted);
            
            //make sure the centerpanel is located in teh middle of the screen
            CenterTextPanel.Left = (this.Width - CenterTextPanel.Width)/2;
            CenterTextPanel.Top = (this.Height - CenterTextPanel.Height)/2;
        }

        void kh_KeyIntercepted(KeyboardHook.KeyboardHookEventArgs e)
        {
            //Check if this key event is being passed to
            //other applications and disable TopMost in 
            //case they need to come to the front
            if (e.PassThrough)
            {
                this.TopMost = false;
            }

            //draw screen
            try
            {
                ds.Draw(GetKeyStringRepresentation(e.KeyCode));
            }
            catch { }   //make sure the app never crashes, because then the rest of the PC is reachable again...

            //play sound
            try
            {
                if (e.KeyUp)
                {
                    //only sound when keyup
                    sp.Play(e.KeyCode);
                }
            }
            catch { }   //make sure the app never crashes, because then the rest of the PC is reachable again...
        }

        private string GetKeyStringRepresentation(Keys key)
        {
            switch (key)
            {
                #region Numbers
                case Keys.D0:
                case Keys.NumPad0:
                    return "0";
                case Keys.D1:
                case Keys.NumPad1:
                    return "1";
                case Keys.D2:
                case Keys.NumPad2:
                    return "2";
                case Keys.D3:
                case Keys.NumPad3:
                    return "3";
                case Keys.D4:
                case Keys.NumPad4:
                    return "4";
                case Keys.D5:
                case Keys.NumPad5:
                    return "5";
                case Keys.D6:
                case Keys.NumPad6:
                    return "6";
                case Keys.D7:
                case Keys.NumPad7:
                    return "7";
                case Keys.D8:
                case Keys.NumPad8:
                    return "8";
                case Keys.D9:
                case Keys.NumPad9:
                    return "9";
                #endregion

                #region NumPad
                case Keys.Add:
                    return "+";
                case Keys.Multiply:
                    return "*";
                case Keys.Subtract:
                    return "-";
                case Keys.Divide:
                    return "/";
                case Keys.Decimal:
                    return ".";
                #endregion

                #region OEM Keys
                case Keys.Separator:
                    return "|";
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.ControlKey:
                    return "Ctrl";
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.ShiftKey:
                    return "Shift";
                case Keys.Capital:
                    return "Caps Lock";
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.Alt:
                    return "Alt";
                case Keys.LWin:
                case Keys.RWin:
                    return "Windows";
                case Keys.Oemtilde:
                    return "~";
                case Keys.Oemcomma:
                    return ",";
                case Keys.OemPeriod:
                    return ".";
                case Keys.OemQuestion:
                    return "?";
                case Keys.OemMinus:
                    return "-";
                case Keys.Oemplus:
                    return "+";
                case Keys.OemSemicolon:
                    return ";";
                case Keys.OemOpenBrackets:
                    return "[";
                case Keys.OemCloseBrackets:
                    return "]";
                case Keys.Back:
                    return "Backspace";
                case Keys.Oem7:
                    return "'";
                case Keys.Oem5:
                    return "\\";
                case Keys.Enter:
                    return "Enter";
                #endregion

                default:
                    return key.ToString();  //works for most keys
            }
        }

        private void lblClear_Click(object sender, EventArgs e)
        {
            //Clear the form
            ds.Initialise();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Restore the screen from DrawShape's backup bitmap if all 
            //or part of the screen needs repainting
            using (Graphics graphics = e.Graphics)
            {
                graphics.DrawImage(ds.BackupBitmap, 0, 0);
            }

        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            //Put the form back on top if it is activated
            //(this will hide the Windows Taskbar)
            this.TopMost = true;
        }


    }
}