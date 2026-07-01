using EventBus;
using System;
using System.Windows.Forms;
using ScopeX.Controls.Common.Structs;
using ScopeX.Core;

namespace ScopeX.U2
{
    public class VectorAnalysisApp
    {
        private VsaErrParamInfoForm? _vsaErrParamInfoForm;

        public VectorAnalysisApp(VectorAnalysisPrsnt prsnt)
        {
            Presenter = prsnt;
        }

        public static VectorAnalysisApp Default { get; internal set; }

        public Form InfoForm { get; private set; }

        public VectorAnalysisPrsnt Presenter { get; }

        public void HideInfoForm()
        {
            InfoForm?.Close();
            InfoForm = null;
        }

        private void VsaErrParamInfoForm_FormClosed(Object sender, FormClosedEventArgs e)
        {
            if (ReferenceEquals(sender, _vsaErrParamInfoForm))
                _vsaErrParamInfoForm = null;
        }

        public void CloseVsaErrParamInfoForm()
        {
            if (_vsaErrParamInfoForm == null || _vsaErrParamInfoForm.IsDisposed)
            {
                _vsaErrParamInfoForm = null;
                return;
            }

            var f = _vsaErrParamInfoForm;
            _vsaErrParamInfoForm = null;
            f.FormClosed -= VsaErrParamInfoForm_FormClosed;
            f.Close();
        }

        public void SyncVsaErrParamInfoFormWithPresenterEnabled()
        {
            if (Presenter.Enabled)
            {
                CloseVsaErrParamInfoForm();
                _vsaErrParamInfoForm = new VsaErrParamInfoForm(Presenter.GenerateDigtalPrsnt)
                {
                    Anchor = AnchorStyles.Top,
                    Location = new(100, 100),
                };
                _vsaErrParamInfoForm.FormClosed += VsaErrParamInfoForm_FormClosed;
                EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = _vsaErrParamInfoForm, Type = FormType.InfoForm });
            }
            else
            {
                CloseVsaErrParamInfoForm();
            }
        }

        public VectorAnalysisForm MakeForm()
        {
            var saf = new VectorAnalysisForm()
            {
                Presenter = Presenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };
            saf.Presenter.TryAddView(saf);

            return saf;
        }

        public void ShowInfoForm()
        {
            if (InfoForm is not null)
            {
                EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = InfoForm, Type = FormType.InfoForm });
            }
        }
    }
}
