using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;

namespace GetAllPicture
{
    public partial class MainForm : DevComponents.DotNetBar.Metro.MetroAppForm
    {
        /// <summary>
        /// Ĭ������·��
        /// </summary>
        private string DEFAULT_DOWNLOAD_PATH = Application.StartupPath + "\\download";
        /// <summary>
        /// ÿ��item֮��ļ��
        /// </summary>
        private int ITEM_HEIGHT = 35;

        public Hashtable itemList = new Hashtable(); //�����б�controlList

        /// <summary>
        /// ִ�������¼�ί��
        /// </summary>
        public delegate void GetPicEventHandler();
        public GetPicEventHandler getPicEventHandler; 

        public MainForm()
        {
            InitializeComponent();
        }

        //ִ�����ذ�ť����¼�
        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (!inputCheck())
            {
                return;
            }
            else
            {
                //���������б�
                string[] dList = getDownloadList();
                string folderPath = this.filePathTextbox.Text;
                setDownloadTab(dList, folderPath);
                //��ִ̨������
                DownloadThread dt = new DownloadThread(dList, folderPath, getPicEventHandler, itemList);
                Thread thread = new Thread(dt.ThreadProc);
                thread.IsBackground = true;
                thread.Start();
                //�л�tabҳ��status���
                this.metroShell1.SelectedTab = this.metroTabItem2;
            }
        }

        /// <summary>
        /// ��statusҳ�����ɶ�Ӧ���������б�
        /// </summary>
        /// <param name="dList"></param>
        /// <param name="folderPath"></param>
        private void setDownloadTab(string[] dList, string folderPath)
        {
            for(int i = 0; i < dList.Length; i++)
            {
                DownloadItemControl item = new DownloadItemControl(dList[i].ToString());
                item.Tag = dList[i].ToString();
                this.panel1.Controls.Add(item);
                item.Location = new Point(0, i * ITEM_HEIGHT);
                item.Show();
                itemList.Add(item.Tag, item);
            }
        }

        /// <summary>
        /// ��ȡ�����б�
        /// </summary>
        /// <returns>���������б���ַ�������</returns>
        private string[] getDownloadList()
        {
            string listStr = this.richTextBoxEx1.Text;
            string[] list = Regex.Split(listStr, "\n");
            return list;
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        private bool inputCheck()
        {
            bool isLeagle = true;
            return isLeagle;
        }

        //ѡ��·����ť����¼�
        private void button2_Click(object sender, EventArgs e)
        {
            string downloadPath = DEFAULT_DOWNLOAD_PATH;
            //�½�Ĭ������Ŀ¼
            if(!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = downloadPath;
            if (folder.ShowDialog() == DialogResult.OK)
            {
                this.filePathTextbox.Text = folder.SelectedPath;
            }
        }

        /// <summary>
        /// ����ͼƬ�߳���
        /// </summary>
        public class DownloadThread
        {
            private string[] downloadList;
            private string savePath;
            private MainForm.GetPicEventHandler getPicEventHandler;
            private Hashtable itemList;

            public DownloadThread(string[] dList, string folderPath, GetPicEventHandler getPicEventHandler, Hashtable itemList)
            {
                this.downloadList = dList;
                this.savePath = folderPath;
                this.getPicEventHandler = getPicEventHandler;
                this.itemList = itemList;
            }

            public void ThreadProc()
            {
                downloadPic(this.downloadList, this.savePath);
            }

            /// <summary>
            /// ִ������ͼƬ
            /// </summary>
            private void downloadPic(string[] dList, string folderPath)
            {
                for (int i = 0; i < dList.Length; i++)
                {
                    string url = dList[i];
                    if (url == "")
                    {
                        continue;
                    }
                    string picName = url.Substring(url.LastIndexOf('/'));
                    string savePath = folderPath + picName;
                    
                    DownloadItemControl item = (DownloadItemControl)itemList[url];
                    item.setDownloading();

                    try
                    {
                        WebRequest request = WebRequest.Create(url);
                        WebResponse response = request.GetResponse();
                        Stream reader = response.GetResponseStream();
                        FileStream writer = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                        byte[] buff = new byte[512];
                        int c = 0; //ʵ�ʶ�ȡ���ֽ���
                        while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                        {
                            writer.Write(buff, 0, c);
                        }
                        writer.Close();
                        writer.Dispose();

                        reader.Close();
                        reader.Dispose();
                        response.Close();
                        item.setDownloadComplete();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(url + "\n" + ex.Message, "Download Picture Error");
                        item.setDownloadFail();
                    }
                }
                MessageBox.Show("Download Completed");
            }
        }
    }
}