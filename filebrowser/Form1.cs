using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace filebrowser
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;
        public Form1()
        {
            InitializeComponent();

            SetWindowPos(Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width / 2,21);
            this.Location = new Point((Screen.PrimaryScreen.Bounds.Width/2)-(this.Size.Width/2), Screen.PrimaryScreen.WorkingArea.Bottom- 21);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.SendToBack();
            TaskService taskService = new TaskService();
            Microsoft.Win32.TaskScheduler.Task task = taskService.FindTask("bottomfilebrowser");
            if (task != null)
            {
                runOnStartupToolStripMenuItem.Checked = true;
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            this.SendToBack();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            this.SendToBack();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            this.SendToBack();
        }
        bool md = false;
        private void resizer_MouseUp(object sender, MouseEventArgs e)
        {
            md = false;
        }

        private void resizer_MouseDown(object sender, MouseEventArgs e)
        {
            md = true;
        }

        private void resizer_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            if (md == true)
            {
                this.Top = Math.Min(Screen.PrimaryScreen.WorkingArea.Bottom - 21, Math.Max(Screen.PrimaryScreen.WorkingArea.Top,Cursor.Position.Y));
                this.Height = Screen.PrimaryScreen.Bounds.Height-this.Top;
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void runOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TaskService taskService = new TaskService();
                Microsoft.Win32.TaskScheduler.Task task = taskService.FindTask("bottomfilebrowser");
                if (task != null)
                {
                    runOnStartupToolStripMenuItem.Checked = true;
                }
                if (runOnStartupToolStripMenuItem.Checked == false)
                {
                    if (task == null)
                    {
                        runOnStartupToolStripMenuItem.Checked = true;

                        TaskDefinition taskDefinition = taskService.NewTask();

                        taskDefinition.RegistrationInfo.Description = "start up";
                        taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
                        taskDefinition.Triggers.Add(new LogonTrigger());

                        taskDefinition.Actions.Add(new ExecAction(Application.ExecutablePath));

                        taskService.RootFolder.RegisterTaskDefinition("bottomfilebrowser", taskDefinition);

                    }
                    runOnStartupToolStripMenuItem.Checked = true;
                }
                else
                {
                    if (task != null)
                    {
                        taskService.RootFolder.DeleteTask("bottomfilebrowser");
                    }
                    taskService.Dispose();
                    runOnStartupToolStripMenuItem.Checked = false;
                }
            }
            catch { runOnStartupToolStripMenuItem.Checked = false; MessageBox.Show("Restart as admin","",MessageBoxButtons.OK,MessageBoxIcon.Error); }
        }
    }
}
