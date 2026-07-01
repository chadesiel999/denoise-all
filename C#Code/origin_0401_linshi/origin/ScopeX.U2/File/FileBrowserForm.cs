using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    public partial class FileBrowserForm : ScopeX.UserControls.FileBrowserForm
    {
        public static FileBrowserForm Instance { get; } = new FileBrowserForm();

        private FileBrowserForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            CanClose = false;
            IsShowPin = false;
            IsShowPin = FixedToolIconInfos[2].IsShow = false;

            SetFileTypeExtensionDelegate(new Func<Enum, string>(x => x.GetAlias()));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetPath(ChoosedFolderPath);
            HeadBackColor = Color.FromArgb(50, 55, 65);
            DirectoryPage.BackColor = Color.FromArgb(41, 43, 50);
            FilePage.BackColor = Color.FromArgb(41, 43, 50);
        }

        public override void SetFileFilter<T>(IEnumerable<T> fts)
        {
            if (!fts.Any())
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"The arugmant '{nameof(fts)}' is empty or null in the function '{nameof(SetFileFilter)}'", EventBus.LogLevel.Error));
#if DEBUG
                throw new ArgumentException(null, nameof(fts));
#else
                return;
#endif
            }

            ComboBoxEx cbxfiletype = base.GetFileTypeCb();

            cbxfiletype.DataSource = fts.Select(x => new KeyValuePair<String, T>(x.ToString() + $"(.{x.GetAlias()})", x)).ToList();
            cbxfiletype.DisplayMember = "Key";
            cbxfiletype.ValueMember = "Value";
        }
    }
}
