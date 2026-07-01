using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.U2.LanguageSupoort;
namespace ScopeX.U2
{
    public partial class SplashForm : Form
    {
        private Image _Img;

        public SplashForm()
        {
            InitializeComponent();
            LblMessage.Visible = false;
            //TopMost = true;

            //Load Embedded Resource
            var ass = Assembly.GetExecutingAssembly();
            var mrs = ass.GetManifestResourceNames()?.ToList().FirstOrDefault(c => c.EndsWith(@"ScopeX.U2.Resources.Splash.gif"));
            if (!string.IsNullOrEmpty(mrs))
            {
                using var stream = ass.GetManifestResourceStream(@"ScopeX.U2.Resources.Splash.gif");
                _Img = Image.FromStream(stream);
            }
            else
            {
                string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ScopXConfig");
                //string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "splash.png");
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "splash_uestc.png");
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\StartMenu", "logo.png");
                string aboutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "AboutInfo.xml");
                //string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ScopXConfig", "splash.png");
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ScopXConfig", "splash_uestc.png");
                //string targetImgFilePath = Path.Combine(basePath, "splash.png");
                string targetImgFilePath = Path.Combine(basePath, "splash_uestc.png");
                string targetLogoFilePath = Path.Combine(basePath, "logo.png");
                string targetAboutFilePath = Path.Combine(basePath, "AboutInfo.xml");
                try
                {
                    if (!System.IO.File.Exists(targetImgFilePath) || !System.IO.File.Exists(targetLogoFilePath) || !System.IO.File.Exists(targetAboutFilePath))
                    {
                        Directory.CreateDirectory(basePath);
                        System.IO.File.SetAttributes(basePath, System.IO.File.GetAttributes(basePath) | FileAttributes.Hidden);
                        System.IO.File.Copy(imagePath, targetImgFilePath, true);
                        System.IO.File.Copy(logoPath, targetLogoFilePath, true);
                        System.IO.File.Copy(aboutPath, targetAboutFilePath, true);
                        Bitmap bitmap = new Bitmap(targetImgFilePath);
                        _Img = bitmap;
                    }
                    else
                    {
                         Bitmap bmp2 = new Bitmap(filePath);
                         _Img = bmp2;
                    }
                }
                catch
                {
                    //Console.WriteLine($"An error occurred: {ex.Message}");
                }
                
                //if (!System.IO.File.Exists(imagePath))
                //{
                //    _Img = Properties.Resources.Splash;
                //}
                //else
                //{
                //    Bitmap bitmap = new Bitmap(imagePath);

                //    _Img = bitmap;//Properties.Resources.Splash;
                //}
            }

            //if (Constants.ENABLE_DEBUG)
            //    ClientSize = new Size(100, 100);
            //else
            //    ClientSize = _Img.Size;
            ClientSize = _Img.Size;

            GifLb.Image = _Img;
            GifLb.Paint += GifLb_Paint;
        }
        
        private void GifLb_Paint(object sender, PaintEventArgs e)
        {
            var textsize = TextRenderer.MeasureText(LblMessage.Text, LblMessage.Font);
            TextRenderer.DrawText(
                e.Graphics,
                LblMessage.Text,
                LblMessage.Font,
                new Point(/*(GifLb.Image.Width - textsize.Width) / 2*/10, GifLb.Image.Height - textsize.Height - 10), LblMessage.ForeColor);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _Img?.Dispose();
            _Img = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(Program.InitApp), this);
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        public void ShowMessge(String msg)
        {
            LblMessage.Text = msg;
            GifLb.Invalidate();
        }
    }
}
