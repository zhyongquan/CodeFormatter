using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace CodeFormatter
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void txtSource_DoubleClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtSource.Text = folderDialog.SelectedPath;
            }
        }

        private void txtDestination_DoubleClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtDestination.Text = folderDialog.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.listHeaders.Items.Count == 0)
            {
                return;
            }
            if (this.txtDestination.Text == "" || this.txtSource.Text == "")
            {
                return;
            }
            if(this.txtDestination.Text==this.txtSource.Text)
            {
                return;
            }
            string source = this.txtSource.Text, destination = this.txtDestination.Text;
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = true;
            //

            DirectoryInfo dir = new DirectoryInfo(source);
            List<string> fileList = new List<string>();
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                fileList.AddRange(GetAll(d));
            }
            //
            int count = fileList.Count;
            this.txtLog.Text="Start";
            for (int i = 0; i < count; i++)
            {
                this.txtLog.AppendText("\r\n" + fileList[i]);
                Formatter.Do(source, destination, fileList[i]);
                this.progressBar1.Value = (int)((i + 1) / count * 100);
            }
            foreach (object obj in this.listHeaders.Items)
            {
                string file = obj.ToString();
                File.Copy(file, destination + "\\" + Path.GetFileName(file), true);
            }
            this.txtLog.AppendText("\r\n"+"End");

            Process.Start("explorer.exe", destination);
        }
        private List<string> GetAll(DirectoryInfo dir)//搜索文件夹中的文件       
        {
            List<string> FileList =new List<string>();
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                FileList.Add(fi.FullName);
            }
            return FileList;
        }

        private void listHeaders_DoubleClick(object sender, EventArgs e)
        {
            this.listHeaders.Items.Clear();
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Filter = "Header File(*.h)|*.h";
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string str in fileDialog.FileNames)
                {
                    this.listHeaders.Items.Add(str);
                }
            }
        }
    }
}
