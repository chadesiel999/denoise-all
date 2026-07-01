using EventBus;
using System;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Controls.Common.Structs;
using Microsoft.VisualBasic;
using ScopeX.Core.Jitter;

namespace ScopeX.U2
{
    public class JitterApp :IJitterView, IDisposable
    {
        public JitterApp(JitterPrsnt prsnt)
        {
            Presenter = prsnt;
            Presenter?.TryAddView(this);
        }

        public static JitterApp Default { get; internal set; }

        public JitterInfo JitterInfo { get; set; }

        public void ShowForm()
        {
            if(JitterInfo!=null&& !JitterInfo.IsDisposed&& JitterInfo.IsHandleCreated)
            {
                JitterInfo.OnBodyClicked();
            }
        }

        //public JitterForm MakeForm()
        //{
        //    var jf = new JitterForm()
        //    {
        //        Presenter = Presenter,
        //        Anchor = AnchorStyles.Top | AnchorStyles.Right,
        //    };
        //    jf.Presenter.TryAddView(jf);

        //    return jf;
        //}

        public SerialAnalysisForm MakeForm()
        {
            var saf = new SerialAnalysisForm(Presenter as JitterPrsnt);
            saf.Presenter.TryAddView(saf);

            return saf;
        }

        //public void MakeForm()
        //{
        //    Presenter.Active = true;
        //}

        public MeasureResultForm MakeMeasureResultForm()
        {
            var mrf = new MeasureResultForm()
            {
                Presenter = Presenter as JitterPrsnt,
                Anchor = AnchorStyles.Top,
                Location = new(100, 100),
            };
            Presenter.TryAddView(mrf);
            return mrf;
        }

        private Control JitterParamInfoCtrl
        {
            get;
            set;
        }

        public void ShowJitterParamInfoForm()
        {
            var form = JitterParamInfoCtrl?.FindForm();

            if (form is null)
            {
                var mrf = new MeasureResultForm()
                {
                    Presenter = Presenter as JitterPrsnt,
                    Anchor = AnchorStyles.Top,
                    Location = new(100, 100),
                };

                mrf.Presenter.TryAddView(mrf);

                JitterParamInfoCtrl = mrf.GetDataView;

                EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = mrf, Type = FormType.InfoForm });
            }
        }

        public void CloseJitterParamInfoForm(Boolean needInvokeClose)
        {
            if (needInvokeClose)
            {
                JitterParamInfoCtrl?.FindForm()?.Close();
            }

            JitterParamInfoCtrl = null;
        }

        private Control EyePatternParamInfoCtrl
        {
            get;
            set;
        }

        public void ShowEyePatternParamInfoForm()
        {
            var form = EyePatternParamInfoCtrl?.FindForm();

            if (form is null)
            {
                var eppf = new EyePatternParameterForm()
                {
                    Presenter = Presenter as JitterPrsnt,
                    Anchor = AnchorStyles.Top,
                    Location = new(100, 100),
                };

                eppf.Presenter.TryAddView(eppf);

                EyePatternParamInfoCtrl = eppf.GetDataView;

                EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = eppf, Type = FormType.InfoForm });
            }
        }

        public void CloseEyePatternParamInfoForm(Boolean needInvokeClose)
        {
            if (needInvokeClose)
            {
                EyePatternParamInfoCtrl?.FindForm()?.Close();
            }

            EyePatternParamInfoCtrl = null;
        }
        public Boolean VisibleBathtub { get; set; }

        public Boolean VisibleQFactor { get; set; }

        public Boolean VisibleSpectrum { get; set; }

        public Boolean VisibleHistogram { get; set; }

        public Boolean VisibleTrack { get; set; }
        public IJitterPrsnt Presenter { get ; set; }

        public void UpdateView(Object presenter, String propertyName)
        {
            if ((Program.Oscilloscope.View as DsoForm).InvokeRequired)
            {
                (Program.Oscilloscope.View as DsoForm).BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }

        public void Update(object prsnt, string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            var jprsnt = Presenter as JitterPrsnt;
            switch (propertyName)
            {
                case nameof(jprsnt.Active):
                    (Program.Oscilloscope.View as DsoForm)?.UpdateView(prsnt, propertyName);
                    break;
                case nameof(jprsnt.EyeParamEnable):
                    if (jprsnt.Active)
                    {
                        if (jprsnt.EyeParamEnable)
                        {
                            ShowEyePatternParamInfoForm();
                        }
                        else
                        {
                            CloseEyePatternParamInfoForm(true);
                        }
                    }
                    break;
                case nameof(jprsnt.JitterParamEnable):
                    if (jprsnt.Active)
                    {
                        if (jprsnt.JitterParamEnable)
                        {
                            ShowJitterParamInfoForm();
                        }
                        else
                        {
                            CloseJitterParamInfoForm(true);
                        }
                    }
                    break;
            }
        }

        public void Dispose()
        {
            Presenter?.TryRemoveView(this);
        }
    }
}
