using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
	public class SettingPrsnt : MulticastPrsnt<ISettingView>, ISettingPrsnt
	{
		private protected override SettingModel Model
		{
			get;
		}
		public SettingPrsnt(IDsoPrsnt idp, ISettingView? view) : base(idp)
		{
			Model = DsoModel.Default.Setting;
			Model.PropertyChanged += OnPropertyChanged;

			if (view is not null)
			{
				view.Presenter = this;

				TryAddView(view);
			}
		}

		public AuxInputType AuxInputSignal
		{
			get => Model.AuxInputSignal;
			set
			{
				Model.AuxInputSignal = value;
				Hardware.HdCmdFactory.Push(HdCmd.AWGConfig);
			}
		}
		public EdgeSlope AuxInPolarity
		{
			get => Model.AuxInPolarity;
			set
			{
				Model.AuxInPolarity = value;
				Hardware.HdCmdFactory.Push(HdCmd.AWGConfig);
			}
		}
		public AuxOutputType AuxOutputSignal
		{
			get => Model.AuxOutputSignal;
			set => Model.AuxOutputSignal = value;
		}
		public EdgeSlope AuxOutPolarity
		{
			get => Model.AuxOutPolarity;
			set => Model.AuxOutPolarity = value;
		}
	}
}
