using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Printing;
using System.ComponentModel;
using System.Drawing;

namespace ScopeX.Core
{
    public class PrintPrsnt : MulticastPrsnt<IPrintView>, IPrintPrsnt
    {
        private protected override PrintModel Model
        {
            get;
        }

        public PrintPrsnt(IDsoPrsnt idp, IPrintView? view, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.Print,
                ModelCreateOptions.Standalone => new(),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        //public PrintFunction Function
        //{
        //    get => Model.Function;
        //    set => Model.Function = value;
        //}

        public PrintOrient Orient
        {
            get => Model.Orient;
            set => Model.Orient = value;

        }
        public PrintColor PrintColor
        {
            get => Model.PrintColor;
            set => Model.PrintColor = value;
        }

        public PrintSaveArea PrintArea
        {
            get => Model.PrintArea;
            set => Model.PrintArea = value;
        }

        //Print to some one file
        //public String FileName
        //{
        //    get => Model.PrntFileName;
        //    set => Model.PrntFileName = value;
        //}

        //public String FilePath
        //{
        //    get => Model.PrntFilePath;
        //    set => Model.PrntFilePath = value;
        //}
    }
}
