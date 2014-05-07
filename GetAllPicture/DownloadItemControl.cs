using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GetAllPicture
{
    public partial class DownloadItemControl : UserControl
    {
        private Image imgComplete = Image.FromFile("img\\OK.png");
        private Image imgError = Image.FromFile("img\\error.png");
        private Image imgDownloading = Image.FromFile("img\\progress.gif");
 
        public DownloadItemControl()
        {
            InitializeComponent();
        }

        public DownloadItemControl(string itemName)
        {
            InitializeComponent();
            this.label1.Text = itemName;
        }

        public void setDownloadComplete()
        {
            this.pictureBox1.Image = imgComplete;
        }

        public void setDownloading()
        {
            this.pictureBox1.Image = imgDownloading;
        }

        public void setDownloadFail()
        {
            this.pictureBox1.Image = imgError;
        }
    }
}
