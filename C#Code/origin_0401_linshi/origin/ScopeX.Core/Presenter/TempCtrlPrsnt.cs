using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public class TempCtrlPrsnt : MulticastPrsnt<ITempView>, ITempPrsnt
    {
        public TempCtrlPrsnt(IDsoPrsnt idp, ITempView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.TempCtrl,
                ModelCreateOptions.Standalone => new TempCtrlModel(),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        private protected override TempCtrlModel Model { get; }

        public Boolean AutoCtrlFans
        {
            get => Model.AutoCtrlFans;
            set
            {
                Model.AutoCtrlFans = value;
                Hardware.HdCmdFactory.Push(HdCmd.SystemCtrl);
            }
        }

        public Boolean AutoCaliSystem
        {
            get => Model.AutoCaliSystem;
            set => Model.AutoCaliSystem = value;
        }

        public String[] FansName => Model.FansName;

        public Int32 CurFanNameId
        { 
            get => Model.CurFanNameId;
            set => Model.CurFanNameId = value;
        }

        public Int32 CurFanSpeed
        {
            get => Model.CurFanSpeed;
            set
            {
                Model.CurFanSpeed = value;
                Hardware.HdCmdFactory.Push(HdCmd.SystemCtrl);
            }
        }

        public Int32 SpeedMax => Model.SpeedMax;
        public Int32 SpeedMin => Model.SpeedMin;

        public Dictionary<String, Double> Temp => Model.Temp;
    }
}
