// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/24</date>

namespace ScopeX.U2
{
    using EventBus;
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.Controls.Common.Structs;
    using ScopeX.U2.PassFail;

    /// <summary>
    /// Defines the <see cref="PassFailApp" />.
    /// </summary>
    public class PassFailApp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PassFailApp"/> class.
        /// </summary>
        /// <param name="prsnt">The prsnt<see cref="PassFailPrsnt"/>.</param>
        public PassFailApp(PassFailPrsnt prsnt)
        {
            Presenter = prsnt;
        }

        /// <summary>
        /// Gets or sets the Default.
        /// </summary>
        public static PassFailApp Default { get; internal set; }

        /// <summary>
        /// Gets the InfoForm.
        /// </summary>
        public Form InfoForm { get; private set; }

        /// <summary>
        /// Gets the Presenter.
        /// </summary>
        public PassFailPrsnt Presenter { get; }

        /// <summary>
        /// Gets or sets the VisibleMask.
        /// </summary>
        //public Boolean VisibleMask { get; set; } = true;


        /// <summary>
        /// The MakeForm.
        /// </summary>
        /// <returns>The <see cref="SearchForm"/>.</returns>
        public PassFailForm MakeForm()
        {
            var pff = new PassFailForm()
            {
                Presenter = Presenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };
            pff.Presenter.TryAddView(pff);

            return pff;
        }
        public Control InfoControl
        {
            get;
            set;
        }

        public void ShowInfoForm()
        {
            if (InfoControl is null)
            {
                var pfif = new PassFailInfoForm(Presenter)
                {
                    Anchor = AnchorStyles.Top,
                    Location = new(100, 100),
                };

                InfoControl = pfif.GetDataView;

                EventBus.EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = pfif, Type = FormType.InfoForm });
            }
        }
        /// <summary>
        /// 电压值转虚拟坐标值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="scaleBymV"></param>
        /// <param name="posIdxPerDiv"></param>
        /// <returns></returns>
        public Double GetPosition(Double value, Double scaleBymV, Double posIdxPerDiv)
        {
            return value / scaleBymV * posIdxPerDiv;
        }
    }
}
