using Veldrid.Common;
namespace VeldridTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FontStashSharp.FontManger.Instance.AddFontDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Fonts));
            Application.Run(new Form1());
        }
    }
}