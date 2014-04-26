using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Threading;
using DevComponents.DotNetBar.Metro;

namespace GetAllPicture
{
    public partial class MainForm : MetroForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private string[] getDownloadList()
        {
            string listStr = this.textBox1.Text;
            string[] list = Regex.Split(listStr, "\r\n");
            return list;
        }

        private void downloadPic(string[] dList, string folderPath)
        {
            for (int i = 0; i < dList.Length; i++)
            {
                string url = dList[i];
                if(url == "")
                {
                    continue;
                }
                string picName = url.Substring(url.LastIndexOf('/'));
                string savePath = folderPath + picName;
                
                try
                {
                    WebRequest request = WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    Stream reader = response.GetResponseStream();
                    FileStream writer = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                    byte[] buff = new byte[512];
                    int c = 0; //实际读取的字节数
                    while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                    {
                        writer.Write(buff, 0, c);
                    }
                    writer.Close();
                    writer.Dispose();

                    reader.Close();
                    reader.Dispose();
                    response.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(url + "\n" + ex.Message, "Download Picture Error");
                }
            }
            MessageBox.Show("Download Completed");
        }

        private void doDownload()
        {
            string[] dList = getDownloadList();
            string savePath = this.textBox2.Text;
            downloadPic(dList, savePath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(doDownload);
            t.Start();
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            string appPath = Application.StartupPath;
            string defaultOutputPath = appPath + "";
            dialog.SelectedPath = "";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.SelectedPath;
            }
            else
            {
                textBox1.Text = "";
            } 
        }
    }
}
