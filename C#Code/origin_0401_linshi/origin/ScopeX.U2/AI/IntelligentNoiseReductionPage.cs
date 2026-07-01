using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class IntelligentNoiseReductionPage : UserControl, IArtificialIntelligenceView, IStylize
    {
        public IntelligentNoiseReductionPage()
        {
            InitializeComponent();
            InitCbxSource();
            InitCbxMethod();
            InitNebMaxAverageCnt();
        }

        private void InitCbxSource()
        {
            CbxSource.DataSource = ChannelIdExt.GetAnalogs().Select(o => new KeyValuePair<ChannelId, String>(o, o.ToString())).ToList();
            CbxSource.DisplayMember = "Value";
            CbxSource.ValueMember = "Key";

            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.NoiseReductionChnlId = (ChannelId)CbxSource.SelectedIndex;
                }
            };
        }

        private void InitCbxMethod()
        {
            CbxMethod.DataSource = Enum.GetValues<NoiseRedutionMethod>().Select(o => new KeyValuePair<String, NoiseRedutionMethod>(o.GetAlias(), o)).ToList();
            CbxMethod.DisplayMember = "Key";
            CbxMethod.ValueMember = "Value";

            CbxMethod.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.CurNoiseRedutionMethod = (NoiseRedutionMethod)CbxMethod.SelectedIndex;
                }
                UpdateModelButtonsVisibility();
            };
        }

        private void InitNebMaxAverageCnt()
        {
            NebMaxAverageCnt.AddClicked = (a, b) => Presenter.MaxAverageCount++;
            NebMaxAverageCnt.SubClicked = (a, b) => Presenter.MaxAverageCount--;
            NebMaxAverageCnt.StringFormatFunc = (value) => Presenter.MaxAverageCount.ToString();
            NebMaxAverageCnt.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.MaxAverageCount = (Int32)data;

                nkf.SetKeyBoardValue(LblMaxAverageCnt.Text, "", 2, onokclickeventaction,
                    Presenter.MaxAverageCount, Int32.MaxValue, Int32.MinValue);

                nkf.ShowDialogByPosition();
            };
        }

        private Boolean _ArgToCtrl = false;
        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }


        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public ArtificialIntelligencePrsnt Presenter
        {
            get => (ArtificialIntelligencePrsnt)(ParentForm as IArtificialIntelligenceView).Presenter;
            set => (ParentForm as IArtificialIntelligenceView).Presenter = value;
        }

        IArtificialIntelligencePrsnt IView<IArtificialIntelligencePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (ArtificialIntelligencePrsnt)value;
        }

        public void UpdateView(object prsnt, string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            _ArgToCtrl = true;

            switch (propertyName)
            {
                case nameof(Presenter.NoiseReductionChnlId):
                    UpdateView();
                    break;

                case nameof(Presenter.CurAINoiseReductionEnable):
                    ChkIntelligentNoiseReduction.Checked = Presenter.CurAINoiseReductionEnable;
                    break;

                case nameof(Presenter.CurNoiseRedutionMethod):
                    CbxMethod.SelectedIndex = (Int32)Presenter.CurNoiseRedutionMethod;
                    UpdateModelButtonsVisibility();
                    break;

                case nameof(Presenter.MaxAverageCount):
                    NebMaxAverageCnt.UpdateValueString();
                    break;
            }

            _ArgToCtrl = false;
        }

        private void UpdateView()
        {
            if (DesignMode)
                return;

            _ArgToCtrl = true;

            CbxSource.SelectedIndex = (Int32)Presenter.NoiseReductionChnlId;
            ChkIntelligentNoiseReduction.Checked = Presenter.CurAINoiseReductionEnable;
            CbxMethod.SelectedIndex = (Int32)Presenter.CurNoiseRedutionMethod;
            NebMaxAverageCnt.UpdateValueString();
            UpdateModelButtonsVisibility();

            _ArgToCtrl = false;
        }

        private void UpdateModelButtonsVisibility()
        {
            bool shouldShowSwitch = false;
            
            if (CbxMethod.SelectedItem != null)
            {
                var selectedMethod = ((KeyValuePair<String, NoiseRedutionMethod>)CbxMethod.SelectedItem).Value;
                
                // 只有当降噪策略为神经网络时才显示模型学习开关
                shouldShowSwitch = (selectedMethod == NoiseRedutionMethod.NeuralNetwork);
            }
            
            ChkModelLearning.Visible = shouldShowSwitch;
            
            // 只有在模型学习开关打开时才显示模型更新按钮
            BtnModelUpdate.Visible = shouldShowSwitch && ChkModelLearning.Checked;
        }

        private void CbxMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 此事件已在 InitCbxMethod 中处理
        }

        private void ChkModelLearning_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                // 更新按钮可见性
                UpdateModelButtonsVisibility();

                // TODO: 在此处添加模型学习开关状态变化的逻辑
                Presenter.ModelLearning = ChkModelLearning.Checked;
            }
        }

        private void BtnModelUpdate_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                // TODO: 在此处添加模型更新相关的逻辑
                Presenter.ActionModelBuild();
            }
        }

        private void ChkIntelligentNoiseReduction_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurAINoiseReductionEnable = ChkIntelligentNoiseReduction.Checked;
            }
        }

        private void BtnResetAvgCnt_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ResetAverageNoiseRedution();
            }
        }
    }
}
