using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ShadowGTMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        String baseFolder;
        List<String> Imgfiles = new List<string>();
        int currentImg = 0;
        Point currntLocation;
        bool isPlay = false;
        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.Desktop;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                baseFolder = fbd.SelectedPath;
                textBox1.Text = fbd.SelectedPath;
                button5_Click(sender, e);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            DirectoryInfo folder = new DirectoryInfo(baseFolder);
            FileInfo[] files = folder.GetFiles();
            Imgfiles.Clear();
            // Display all the files.
            foreach (FileInfo img in files)
            {
                if (img.Extension.ToLower() == ".png" ||
                   img.Extension.ToLower() == ".jpg" ||
                   img.Extension.ToLower() == ".gif" ||
                   img.Extension.ToLower() == ".jpeg" ||
                   img.Extension.ToLower() == ".bmp" ||
                   img.Extension.ToLower() == ".tif")
                {
                    Imgfiles.Add(img.FullName);
                }
            }
            if (Imgfiles.Count == 0)
            {
                MessageBox.Show("No image found..!");
                return;
            }
            currentImg = 0;
            display();
        }
        private void display()
        {
            if(currentImg>=Imgfiles.Count || currentImg < 0)
            {
                return;
            }
            string DataFileName = System.IO.Path.ChangeExtension(Imgfiles[currentImg], "txt");
            pictureBox1.Image = Image.FromFile(Imgfiles[currentImg]);

            try
            {
                string[] lines = System.IO.File.ReadAllLines(DataFileName);
                string[] data = lines[1].Split(' ');
                currntLocation = new Point(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));
                pictureBox1.Refresh();
                drowLines();

                data = lines[2].Split(' ');
                Point NewLocation = new Point(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));

                int ans = chechNewPoint(NewLocation);

                drowNum(ans, NewLocation);

            }
            catch (FileNotFoundException e)
            {
                if (isPlay)
                {
                    timer1.Stop();
                    MessageBox.Show("No File");
                    isPlay = false;
                }
            }

            //pictureBox1.Refresh();

            //MessageBox.Show("Dot Net Perls is awesome.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentImg++;

            if (currentImg >= Imgfiles.Count)
            {
                
                timer1.Stop();
                isPlay = false;
                MessageBox.Show("Go to beginig..!");
                currentImg = 0;
            }
            display();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentImg--;

            if (currentImg < 0)
            {
                MessageBox.Show("Go to end..!");
                currentImg = Imgfiles.Count - 1;
            }
            display();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {


            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                currntLocation = e.Location;

            }
            pictureBox1.Refresh();
            drowLines();

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int ans = chechNewPoint(e.Location);
                drowNum(ans, e.Location);

                string DataFileName = System.IO.Path.ChangeExtension(Imgfiles[currentImg], "txt");
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(DataFileName, false))
                {
                    file.WriteLine(ans);
                    file.WriteLine(currntLocation.X + " " + currntLocation.Y);
                    file.WriteLine(e.Location.X + " " + e.Location.Y);
                }
                System.Threading.Thread.Sleep(100);
                button1_Click(null, null);
            }

        }
        private void drowNum(int num, Point location)
        {
            using (Graphics g = pictureBox1.CreateGraphics())
            {
                g.FillRectangle(Brushes.Black, location.X, location.Y, 20, 20);
                g.DrawString(" " + num, new Font("Arial", 14), Brushes.Yellow, location);

            }
        }
        private void drowLines()
        {
            using (Graphics g = pictureBox1.CreateGraphics())
            {
                // Draw next line and...
                g.DrawLine(Pens.Black, currntLocation, currntLocation);
                g.DrawLine(Pens.Black, currntLocation, new Point(currntLocation.X, 0));

                int min = Math.Min(pictureBox1.Width - currntLocation.X, currntLocation.Y);
                g.DrawLine(Pens.Black, currntLocation, new Point(currntLocation.X + min, currntLocation.Y - min));

                g.DrawLine(Pens.Black, currntLocation, new Point(pictureBox1.Width, currntLocation.Y));

                min = Math.Min(pictureBox1.Width - currntLocation.X, pictureBox1.Height - currntLocation.Y);
                g.DrawLine(Pens.Black, currntLocation, new Point(currntLocation.X + min, currntLocation.Y + min));

                g.DrawLine(Pens.Black, currntLocation, new Point(currntLocation.X, pictureBox1.Height));

                min = Math.Min(currntLocation.X, pictureBox1.Height - currntLocation.Y);
                g.DrawLine(Pens.Black, currntLocation, new Point(currntLocation.X - min, currntLocation.Y + min));

                g.DrawLine(Pens.Black, currntLocation, new Point(0, currntLocation.Y));

                min = Math.Min(currntLocation.X, currntLocation.Y);
                g.DrawLine(Pens.Black, currntLocation, new Point(currntLocation.X - min, currntLocation.Y - min));

            }
        }

        private int chechNewPoint(Point NewLocation)
        {
            float m = ((float)NewLocation.Y - currntLocation.Y) / ((float)NewLocation.X - currntLocation.X);

            double ang = Math.Atan(m);
            //MessageBox.Show(" "+ang);    

            if (currntLocation.Y > NewLocation.Y)
            {
                if (ang > 1)
                {
                    return 8;
                }
                if (ang > 0)
                {
                    return 7;
                }

                if (ang < -1)
                {
                    return 1;
                }
                return 2;
            }
            else
            {
                if (ang > 1)
                {
                    return 4;
                }
                if (ang > 0)
                {
                    return 3;
                }

                if (ang < -1)
                {
                    return 5;
                }
                return 6;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            isPlay = true;
            timer1.Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
    }
}
