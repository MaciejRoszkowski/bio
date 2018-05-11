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
                lutR[i] = Math.Abs((int)(((sR[i] - sR[posR]) / (1 - sR[posR])) * 255));
                lutG[i] = Math.Abs((int)(((sG[i] - sG[posG]) / (1 - sG[posG])) * 255));
                lutB[i] = Math.Abs((int)(((sB[i] - sB[posB]) / (1 - sB[posB])) * 255));
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
        private void GrayScale()
        {
            var tmpbm = new Bitmap(bm);
            var color = new System.Drawing.Color();
            for (int i = 0; i < tmpbm.Width; i++)
            {
                for (int j = 0; j < tmpbm.Height; j++)
                {
                    color = tmpbm.GetPixel(i, j);
                    //bm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, color.R, color.R, color.R));
                    bm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, color.G, color.G, color.G));
                    //bm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, color.B, color.B, color.B));
                }
            }
            bm = new Bitmap(bm);
        }

        private void binarization_button_Click(object sender, RoutedEventArgs e)
        {
            //var color = new System.Drawing.Color();
            //GrayScale();
            if (int.Parse(bin_value.Text)>255 || int.Parse(bin_value.Text)<0)
            {
                return;
            }
            binarization(int.Parse(bin_value.Text));
        }
        private void binarization(int value)
        {
            var tmpbm = new Bitmap(bm);
            var color = new System.Drawing.Color();
            //GrayScale();

            for (int i = 0; i < tmpbm.Width; i++)
            {
                for (int j = 0; j < tmpbm.Height; j++)
                {
                    color = tmpbm.GetPixel(i, j);
                    if (color.G < value)
                    {
                        tmpbm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, 0, 0, 0));
                    }
                    else
                    {
                        tmpbm.SetPixel(i, j, System.Drawing.Color.FromArgb(color.A, 255, 255, 255));

                    }
                }
            }
            bm = new Bitmap(tmpbm);
            displayImage(bm);


        }
        private void otsu_button_Click(object sender, RoutedEventArgs e)
        {
            //GrayScale();
            var hist = new int[256];
            var tmp = new System.Drawing.Color();
            for (int i = 0; i < 256; i++)
            {
                hist[i] = 0;

            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    tmp = bm.GetPixel(i, j);
                    hist[tmp.G]++;
                }
            }

            int total = bm.Width * bm.Height;
            double wB = 0;
            double wF = 0;
            int sum = 0;
            double sumB = 0;
            double varMax = 0;
            int t = 0;
            double uB;
            double uF;

            for (int i = 0; i < 256; i++)
            {
                sum += i * hist[i];
            }
            for (int i = 0; i < 256; i++)
            {
                wB += hist[i];
                if (wB == 0) continue;

                wF = total - wB;
                if (wF == 0) break;

                sumB += (i * hist[i]);

                uB = sumB / wB; 
                uF = (sum - sumB) / wF;

                double varBetween =wB *wF * Math.Pow((uF - uB),2);

                if (varBetween > varMax)
                {
                    varMax = varBetween;
                    t = i;
                }
            }
            binarization(t);
        }

        private void niblack_button_Click(object sender, RoutedEventArgs e)
        {
            //GrayScale();
            var tmpBitmap = new Bitmap(bm);
            var colors = new int[bm.Width, bm.Height];
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    colors[i, j] = bm.GetPixel(i, j).G;
                }
            }
            double avg=0;
            int count = 0;
            var tmp = new System.Drawing.Color();
            double sD=0;
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    avg = 0;
                    count = 0;
                    sD = 0;
                    for (int k = i- (int.Parse(window_value.Text) / 2); k <= i+(int.Parse(window_value.Text)/2); k++)
                    {
                        for (int l = j-(int.Parse(window_value.Text) / 2); l <= j + (int.Parse(window_value.Text) / 2); l++)
                        {
                            if (k>=0 && k<bm.Width && l>=0 && l<bm.Height)
                            {

                                avg += colors[k, l];
                                sD += Math.Pow(colors[k, l], 2);
                                count++;
                            }

                        }
                    }
                    sD /= count;
                    avg/= count;
                    sD -= Math.Pow(avg,2);

                    sD = Math.Sqrt(sD);
                    tmp = bm.GetPixel(i, j);

                    if (tmp.G >= (avg + double.Parse(k_value.Text) * sD))
                    {

                        tmpBitmap.SetPixel(i, j, System.Drawing.Color.White);


                    }
                    else
                    {
                        tmpBitmap.SetPixel(i, j, System.Drawing.Color.Black);

                    }

                }
            }
            bm =new Bitmap( tmpBitmap);
            displayImage(bm);
        }

        private void conv_button_Click(object sender, RoutedEventArgs e)
        {
            var tmpbm = new Bitmap(bm);

            int sumR = 0;
            int sumG = 0;
            int sumB = 0;
            var w = new int[3, 3];
            w[0, 0] = int.Parse(ul.Text);
            w[1, 0] = int.Parse(um.Text);
            w[2, 0] = int.Parse(ur.Text);
            w[0, 1] = int.Parse(ml.Text);
            w[1, 1] = int.Parse(mm.Text);
            w[2, 1] = int.Parse(mr.Text);
            w[0, 2] = int.Parse(ll.Text);
            w[1, 2] = int.Parse(lm.Text);
            w[2, 2] = int.Parse(lr.Text);
            int sum = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    sum += w[i, j];
                }
            }

            var colorsR = new int[bm.Width, bm.Height];
            var colorsG = new int[bm.Width, bm.Height];
            var colorsB = new int[bm.Width, bm.Height];
            var colorsA = new int[bm.Width, bm.Height];

            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    colorsR[i, j] = bm.GetPixel(i, j).R;
                    colorsG[i, j] = bm.GetPixel(i, j).G;
                    colorsB[i, j] = bm.GetPixel(i, j).B;
                    colorsA[i, j] = bm.GetPixel(i, j).A;
                }
            }
            
            for (int x = 1; x < bm.Width-1; x++)
            {
                for (int y = 1; y < bm.Height-1; y++)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            sumR += colorsR[x + i, y + j] * w[i + 1, j + 1];
                            sumG += colorsG[x + i, y + j] * w[i + 1, j + 1];
                            sumB += colorsB[x + i, y + j] * w[i + 1, j + 1];

                        }
                    }
                    if (sum != 0)
                    {
                        sumR /= sum;
                        sumG /= sum;
                        sumB /= sum;
                    }
                    sumR = sumR > 255 ? 255 : sumR < 0 ? 0 : sumR;
                    sumG = sumG > 255 ? 255 : sumG < 0 ? 0 : sumG;
                    sumB = sumB > 255 ? 255 : sumB < 0 ? 0 : sumB;


                    tmpbm.SetPixel(x, y, System.Drawing.Color.FromArgb(colorsA[x,y], sumR, sumG, sumB));

                    sumR = 0;
                    sumG = 0;
                    sumB = 0;
                }
            }
            bm = new Bitmap(tmpbm);
            displayImage(bm);
        }

        private void median3_Click(object sender, RoutedEventArgs e)
        {
            Median(3);
        }
        private void median5_Click(object sender, RoutedEventArgs e)
        {
            Median(5);
        }
        private void Median(int size)
        {
            var tmpbm = new Bitmap(bm);
            int a = 0;

            var valuesR = new List<int>();
            var valuesG = new List<int>();
            var valuesB = new List<int>();

            var tmpCol = new System.Drawing.Color();
            for (int i = size/2; i < bm.Width-size/2; i++)
            {
                for (int j = size/2; j < bm.Height-size/2; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        for (int l = 0; l < size; l++)
                        {
                            tmpCol = bm.GetPixel(i + k - size/2, j + l - size/2);
                            valuesR.Add(tmpCol.R);
                            valuesG.Add(tmpCol.G);
                            valuesB.Add(tmpCol.B);
                            if (k == size / 2)
                                a = tmpCol.A;
                            
                        }
                    }
                    valuesB.Sort();
                    valuesR.Sort();
                    valuesG.Sort();

                    tmpbm.SetPixel(i, j, System.Drawing.Color.FromArgb(a, valuesR[size * size / 2], valuesG[size * size / 2], valuesB[size * size / 2]));

                    valuesB.Clear();
                    valuesR.Clear();
                    valuesG.Clear();
                }
            }
            bm = new Bitmap(tmpbm);
            displayImage(bm);
        }

        private void kuwahara_Click(object sender, RoutedEventArgs e)
        {
            var rs = new double[4];
            var gs = new double[4];
            var bs = new double[4];

            var rw = new double[4];
            var gw = new double[4];
            var bw = new double[4];

            int rmin, gmin, bmin;
            var tmpbm = new Bitmap(bm);

            var colorsR = new int[bm.Width, bm.Height];
            var colorsG = new int[bm.Width, bm.Height];
            var colorsB = new int[bm.Width, bm.Height];
            var tmpCol = new System.Drawing.Color();

            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    tmpCol = bm.GetPixel(i, j);
                    colorsR[i, j] = tmpCol.R;
                    colorsG[i, j] = tmpCol.G;
                    colorsB[i, j] = tmpCol.B;
                }
            }

            for (int i = 2; i < bm.Width - 2; i++)
            {
                for (int j = 2; j < bm.Height - 2; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        rs[k] = 0;
                        gs[k] = 0;
                        bs[k] = 0;
                    }

                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            rs[0] += colorsR[i - 2 + k, j - 2 + l] / 9.0;
                            rs[1] += colorsR[i + k, j - 2 + l] / 9.0;
                            rs[2] += colorsR[i - 2 + k, j + l] / 9.0;
                            rs[3] += colorsR[i + k, j + l] / 9.0;

                            gs[0] += colorsG[i - 2 + k, j - 2 + l] / 9.0;
                            gs[1] += colorsG[i + k, j - 2 + l] / 9.0;
                            gs[2] += colorsG[i - 2 + k, j + l] / 9.0;
                            gs[3] += colorsG[i + k, j + l] / 9.0;

                            bs[0] += colorsB[i - 2 + k, j - 2 + l] / 9.0;
                            bs[1] += colorsB[i + k, j - 2 + l] / 9.0;
                            bs[2] += colorsB[i - 2 + k, j + l] / 9.0;
                            bs[3] += colorsB[i + k, j + l] / 9.0;
                        }
                    }

                    for (int k = 0; k < 4; k++)
                    {
                        rw[k] = 0;
                        gw[k] = 0;
                        bw[k] = 0;
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            rw[0] += Math.Pow(colorsR[i - 2 + k, j - 2 + l]-rs[0], 2);
                            rw[1] += Math.Pow(colorsR[i + k, j - 2 + l]-rs[1], 2);
                            rw[2] += Math.Pow(colorsR[i - 2 + k, j + l]-rs[2], 2);
                            rw[3] += Math.Pow(colorsR[i + k, j + l]-rs[3], 2);

                            gw[0] += Math.Pow(colorsR[i - 2 + k, j - 2 + l] - gs[0], 2);
                            gw[1] += Math.Pow(colorsR[i + k, j - 2 + l] - gs[1], 2);
                            gw[2] += Math.Pow(colorsR[i - 2 + k, j + l] - gs[2], 2);
                            gw[3] += Math.Pow(colorsR[i + k, j + l] - gs[3], 2);

                            bw[0] += Math.Pow(colorsR[i - 2 + k, j - 2 + l] - bs[0], 2);
                            bw[1] += Math.Pow(colorsR[i + k, j - 2 + l] - bs[1], 2);
                            bw[2] += Math.Pow(colorsR[i - 2 + k, j + l] - bs[2], 2);
                            bw[3] += Math.Pow(colorsR[i + k, j + l] - bs[3], 2);


                        }
                    }
                    rmin = 0;
                    gmin = 0;
                    bmin = 0;
                    for (int k = 1; k < 4; k++)
                    {
                        rmin = rw[k] < rw[rmin] ? k : rmin;
                        gmin = gw[k] < gw[rmin] ? k : gmin;
                        bmin = bw[k] < bw[rmin] ? k : bmin;
                    }

                    tmpbm.SetPixel(i, j, System.Drawing.Color.FromArgb((int)rs[rmin], (int)gs[gmin], (int)bs[bmin]));

                }
            }
            bm = new Bitmap(tmpbm);
            displayImage(bm);
        }
    }
}
