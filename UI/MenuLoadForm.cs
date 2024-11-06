using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Base.UI
{
    public partial class MenuLoadForm : Form
    {
        private static String picPath = Application.StartupPath + @"\logo";//加载页面图片所在路径

        public MenuLoadForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        //加载
        private void MenuLoadForm_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1500;
            timer1.Enabled = true;

            //修改logo
            if (!Directory.Exists(picPath)) Directory.CreateDirectory(picPath);
            else
            {
                List<string> fileNames = GetPictureFiles(picPath);

                foreach (string fileName in fileNames)
                {
                    if (Path.GetFileName(fileName).Split('.')[0] == "logo")
                    {
                        pictureBox1.Image = Image.FromFile(Application.StartupPath + @"\logo\" + fileName);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;//加载的图片充满picrurebox大小
                    }
                }
            }
        }

        //显示加载界面1.5s
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();

            foreach (Form myForm in Application.OpenForms) 
            {
                myForm.Show();
            }

            timer1.Enabled = false;
        }

        //获取多种指定图片格式文件
        private List<string> GetPictureFiles(string filePath)
        {
            DirectoryInfo di = new DirectoryInfo(filePath);
            FileInfo[] files = di.GetFiles("*.*");
            string fileName;
            List<string> list = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                fileName = files[i].Name.ToLower();
                if (fileName.EndsWith(".bmp")
                    || fileName.EndsWith(".png")
                    || fileName.EndsWith(".jpg")
                    || fileName.EndsWith("gif")
                    || fileName.EndsWith("svg")
                    || fileName.EndsWith("tif")
                    || fileName.EndsWith("webp"))
                {
                    list.Add(fileName);
                }
            }
            return list;
        }
    }
}
