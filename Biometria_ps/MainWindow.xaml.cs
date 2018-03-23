using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using OxyPlot;

namespace Biometria_ps
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap bm;

        public MainWindow()
        {

            InitializeComponent();

            LoadImage();


            Image.MouseLeftButtonDown += image_MouseLeftButtonDown;

        }




        private void load_button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadImage();

        }

        private void LoadImage()
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif;*.tiff|All files|*.*";
            if (op.ShowDialog() == true)
            {
                bm = new Bitmap(op.FileName);
                displayImage(bm);
                displayImage(bm,1);


            }
            else
            {
                return;
            }

        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                
                return bitmapimage;
            }
        }


        private void zoom_button_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Size newSize = new System.Drawing.Size((int)(bm.Width * 1.25), (int)(bm.Height * 1.25));
            bm = new Bitmap(bm, newSize);

            displayImage(bm);
        }

        public void displayImage(Bitmap bmp)
        {
            Image.Source = BitmapToImageSource(bmp);
        }
        public void displayImage(Bitmap bmp, int a)
        {
            ImageOrigin.Source = BitmapToImageSource(bmp);
        }

        private void unzoom_button_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Size newSize = new System.Drawing.Size((int)(bm.Width * 0.8), (int)(bm.Height * 0.8));
            bm = new Bitmap(bm, newSize);

            displayImage(bm);

        }
        private void getRGB(Bitmap bm,int x,int y)
        {
            System.Drawing.Color col = bm.GetPixel(x,y);
            text.Text = "R:"+col.R.ToString()+"G:"+col.G.ToString()+"B:"+col.B.ToString();

        }


        private void MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(((IInputElement)e.Source));

            if ((p.X >= 0) && (p.X < bm.Width) && (p.Y >= 0) && (p.Y < bm.Height))
            {
                getRGB(bm, (int)(p.X * bm.Width / Image.ActualWidth), (int)(p.Y * bm.Height / Image.ActualHeight));

            }
        }
        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(((IInputElement)e.Source));
            if ((p.X >= 0) && (p.X < bm.Width) && (p.Y >= 0) && (p.Y < bm.Height))
            {
                System.Drawing.Color color = bm.GetPixel((int)(p.X * bm.Width / Image.ActualWidth), (int)(p.Y * bm.Height / Image.ActualHeight));
                int r = 0;
                int g = 0;
                int b = 0;
                r = int.Parse(tr.Text);
                g = int.Parse(tg.Text);
                b = int.Parse(tb.Text);
                System.Drawing.Color colorToSet = System.Drawing.Color.FromArgb(color.A,r,g,b);

                bm.SetPixel((int)(p.X * bm.Width / Image.ActualWidth), (int)(p.Y * bm.Height / Image.ActualHeight), colorToSet);
                displayImage(bm);

            }
        }


        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Out";
            dlg.Filter = "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|" +
                "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff";
            if (dlg.ShowDialog() == true)
            {
                bm.Save(dlg.FileName);
            }
            else
                return;

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider.Value!=1)
            {

                Image.RenderTransform = null;

                Image.RenderTransform = new ScaleTransform(slider.Value, slider.Value);
                sliderText.Text = slider.Value.ToString();
            }

        }

        private void histogram_button_Click(object sender, RoutedEventArgs e)
        {


            Hisotogram hist = new Hisotogram(bm);

            hist.Show();

        }

        private void lighten_button_Click(object sender, RoutedEventArgs e)
        {
            var powTable = new int[256];
            var color = new System.Drawing.Color();
            int r;
            int g;
            int b;
            for (int i = 0; i < 256; i++)
            {
                //powTable[i] = (int)Math.Pow(i, 1.2);
                powTable[i] = i + 10;
                if (powTable[i] > 255)
                {
                    powTable[i] = 255;

                }


            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    color = bm.GetPixel(i, j);
                    r = powTable[color.R];
                    g = powTable[color.G];
                    b = powTable[color.B];

                    bm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, r, g, b));
                }
            }
            displayImage(bm);

        }

        private void dim_button_Click(object sender, RoutedEventArgs e)
        {
            var logTable = new int[256];
            var color = new System.Drawing.Color();
            int r;
            int g;
            int b;
            for (int i = 0; i < 256; i++)
            {
                //logTable[i] = (int)Math.Log(i+1,1.1);
                logTable[i] = i - 10;
                if (logTable[i]<0)
                {
                    logTable[i] = 0;
                }
            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    color = bm.GetPixel(i, j);
                    r = logTable[color.R];
                    g = logTable[color.G];
                    b = logTable[color.B];

                    bm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, r, g, b));
                }
            }
            displayImage(bm);

        }

        private void strech_button_Click(object sender, RoutedEventArgs e)
        {
            var lut = new double[256];
            var color = new System.Drawing.Color();
            int r;
            int g;
            int b;
            for (int i = 0; i < 256; i++)
            {
                //lut[i] = (255 / (double.Parse(max.Text) - double.Parse(min.Text)) * (i-double.Parse(max.Text) ));
                //if (lut[i]>255)
                //{
                //    lut[i] = 255;
                //}
                //else if(lut[i]<0)
                //{
                //    lut[i] = 0;
                //}
                if (i < int.Parse(min.Text))
                {
                    lut[i] = 0;
                }
                else if (i > int.Parse(max.Text))
                {
                    lut[i] = 255;
                }
                else
                {
                    lut[i] = ((i - double.Parse(min.Text)) / (int.Parse(max.Text) - int.Parse(min.Text))) * 255;
                }

                
            }


            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    color = bm.GetPixel(i, j);
                    r = (int)lut[color.R];
                    g = (int)lut[color.G];
                    b = (int)lut[color.B];

                    bm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, r, g, b));
                }
            }
            displayImage(bm);
        }

        private void equalize_button_Click(object sender, RoutedEventArgs e)
        {
            var histR = new int[256];
            var histG = new int[256];
            var histB = new int[256];
            var sR = new double[256];
            var sG = new double[256];
            var sB = new double[256];
            double pR =0;
            double pG =0;
            double pB =0;
            int posR =-1;
            int posG=-1;
            int posB=-1;
            int r;
            int g;
            int b;
            int n = bm.Height * bm.Width;
            var lutR = new int[256];
            var lutG = new int[256];
            var lutB = new int[256];
            var tmp = new System.Drawing.Color();
            for (int i = 0; i < 256; i++)
            {
                histB[i] = 0;
                histG[i] = 0;
                histR[i] = 0;
            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    tmp = bm.GetPixel(i, j);
                    histR[tmp.R]++;
                    histG[tmp.G]++;
                    histB[tmp.B]++;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                sR[i] = 0;
                sG[i] = 0;
                sB[i] = 0;
                pR += (double)histR[i] / n;
                pG += (double)histG[i] / n;
                pB += (double)histB[i] / n;
                
                for (int j = 0; j < i; j++)
                {
                    sR[i] = pR;
                    sG[i] = pG;
                    sB[i] = pB;
                }
                if (sR[i] !=0 && posR==-1)
                    posR=i;
                if (sG[i] != 0 && posG == -1)
                    posG = i;
                if (sB[i] != 0 && posB == -1)
                    posB = i;
            }
           
            for (int i = 0; i < 256; i++)
            {
                lutR[i] = (int)(((sR[i] - sR[posR]) / (1 - sR[posR])) * 255);
                lutG[i] = (int)(((sG[i] - sG[posG]) / (1 - sG[posG])) * 255);
                lutB[i] = (int)(((sB[i] - sB[posB]) / (1 - sB[posB])) * 255);
            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    tmp = bm.GetPixel(i, j);

                    r = lutR[tmp.R];
                    g = lutG[tmp.G];
                    b = lutB[tmp.B];

                    bm.SetPixel(i, j, System.Drawing.Color.FromArgb(tmp.A, r, g, b));

                }
            }
            displayImage(bm);

        }

        private void gray_button_Click(object sender, RoutedEventArgs e)
        {
            var color = new System.Drawing.Color();
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    color = bm.GetPixel(i, j);
                    //bm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, color.R, color.R, color.R));
                    bm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, color.G, color.G, color.G));
                    //bm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, color.B, color.B, color.B));
                }
            }
            displayImage(bm);
        }
    }
}
