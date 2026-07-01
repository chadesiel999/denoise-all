using FontStashSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Veldrid.Common;
using Veldrid.Sdl2;
using Veldrid.Windows.Winform;

namespace VeldridTest
{
    public partial class Form1 : Form
    {
        IVeldridContent control;
        bool sizechanged = false;
        public Form1()
        {
            InitializeComponent();
            control =new Veldrid.Common.VeldridContent();
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Task.Run(() =>
            {
                int count = 1024 * 1024;
                Veldrid.Common.VeldridCompute.Test.AddTestCompute add = new Veldrid.Common.VeldridCompute.Test.AddTestCompute(control, (uint)count);
                add.Value = 2;
                Random random = new Random();
                int[] data = Enumerable.Range(0, count).Select(x =>(int)(MathF.Sin(x / (Single)count * 20) * 3500 + random.NextSingle() * 500 - 250)).ToArray();
                int[] result = new int[data.Length];
                int index = 0;
                while (true)
                {
                    data = Enumerable.Range(0, count).Select(x =>(int)( MathF.Sin(x / (Single)count * 20) * 3500 + random.NextSingle() * 500 - 250)).ToArray();
                    add.Value = (int)MathF.Round(random.NextSingle()*100,0);
                    add.DoCompute(data);
                    add.GetResult(ref result,add.MaxDataCount);
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (result[i] - add.Value != data[i])
                        {
                            Console.WriteLine("Error");
                            break;
                        }
                    }
                    Console.WriteLine($"{data[1]}:{result[1]}   {add.Value}   {result[1] - data[1]}");
                    index++;
                    Thread.Sleep(1);
                    if(index>=1000) break;
                }
                add?.Dispose();
            });
        }
    }
}
