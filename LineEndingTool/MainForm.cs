using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LineEndingTool
{
    public partial class MainForm : Form
    {
        delegate void SetTextCallback(string _text);

        private StringBuilder messages = new StringBuilder();

        public MainForm()
        {
            InitializeComponent();
        }

        public void ShowMessage(string _text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.listMessage.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ShowMessage);
                this.Invoke(d, new object[] { _text });
            }
            else
            {
                messages.Append(_text);
                this.listMessage.Items.Add(_text);
                this.listMessage.Update();
                //this.rtxtMessage.Text = messages.ToString();
                //this.rtxtMessage.Update();
            }
        }

        private int total = 0;
        private void btnFind_Click(object sender, EventArgs e)
        {
            total = 0;
            ShowMessage($"Start Find: {this.txtFolder.Text}\\{this.txtFileFormat.Text}\r\n");
            this.procceed(this.txtFolder.Text, this.txtFileFormat.Text, false);
            ShowMessage($"End {total}");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void procceed(string path, string fileFormats, bool replaced)
        {            
            //StringBuilder str = new StringBuilder();
            DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles(fileFormats); //Getting Text files
            
            foreach (FileInfo file in Files)
            {
                long count1 = this.Count(file.FullName, "\r");
                long count2 = this.Count(file.FullName, "\n");
                if (count1 != count2)
                {
                    total++;
                    string str = $"{file.FullName} ({count1})({count2})\r\n";

                    if (replaced)
                    {
                        this.Update(file.FullName);
                    }
                    ShowMessage(str);
                }
            }

            DirectoryInfo[] dics = d.GetDirectories();
            foreach (DirectoryInfo sd in dics)
            {
                procceed(sd.FullName, fileFormats, replaced);
            }
        }

        private long Count(string filePath, string pattern)
        {
            string str = File.ReadAllText(filePath);

            long count = str.Length - str.Replace(pattern, "").Length;

            return count;
        }

        private void Update(string filePath)
        {
            string pattern1 = "\r";
            string pattern2 = "\n";
            string pattern3 = "\r\n";

            string str = File.ReadAllText(filePath);

            str = str.Replace(pattern3, pattern1);
            str = str.Replace(pattern2, pattern1);
            str = str.Replace(pattern1, pattern3);

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                {
                    writer.Write(str);
                }
                writer.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            total = 0;
            ShowMessage($"Start Update: {this.txtFolder.Text}\\{this.txtFileFormat.Text}\r\n");
            this.procceed(this.txtFolder.Text, this.txtFileFormat.Text, true);
            ShowMessage($"End {total}");
        }
    }
}
