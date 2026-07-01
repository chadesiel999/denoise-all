/* 文件名称：ControlExtension.cs
 * 文件说明：自定义控件的扩展方法
 * 创建作者：wangcj
 * 创建日期：20210610
 */
using EventBus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScopeX.UserControls;
using ScopeX.Controls.Common.Structs;
using ScopeX.Controls.Common.Helper;
using System.Runtime.CompilerServices;
using ScopeX.U2.BaseControl;

namespace ScopeX.U2
{
    internal static class ControlExtension
    {

        public static void SetItemText(this ComboBoxEx cbe, Object value)
        {
            if (cbe.FindString(value.ToString()) > 0)
            {
                cbe.SelectedValue = value;
            }
            else
            {
                cbe.Text = value.ToString();
            }
        }

        public static void SetItemValue<TValue>(this ComboBoxEx cbe, TValue value)
        {
            foreach (var kvp in cbe.Items.Cast<KeyValuePair<String, TValue>>())
            {
                if (kvp.Value.Equals(value))
                {
                    cbe.SelectedItem = kvp;
                    return;
                }
            }
        }

        public static DialogResult ShowDialogByEvent(this Form form)
        {
            return form == null
                ? throw new ArgumentNullException(nameof(form))
                : EventBroker.Instance.GetEvent<FormShowDialogEventArgs, DialogResult>().Publish(form, new FormShowDialogEventArgs() { Current = form });
        }

        /// <summary>
        /// 把一个enum类型传递给UIRadioButtonGroup，用于初始化按键值及类型；
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="btnGroup"></param>
        /// <param name="choosedItem"></param>
        public static void SetType<T>(this UIRadioButtonGroup btnGroup, T choosedItem)
            where T : Enum
        {
            List<KeyValuePair<string, T>> enumlist = Enum.GetValues(typeof(T)).Cast<T>().Select(item =>
            {
                var itemDescription = item.GetDescription();
                return new KeyValuePair<string, T>(itemDescription, item);
            }).ToList();

            for (int i = 0; i < enumlist.Count; i++)
            {
                if (btnGroup.ScopeXIconButtons.Length > i)
                {
                    btnGroup.ButtonItems[i].Text = enumlist[i].Key;
                    btnGroup.ScopeXIconButtons[i].Text = enumlist[i].Key;
                    btnGroup.ButtonItems[i].Tag = enumlist[i].Value;
                    if (enumlist[i].Key == choosedItem.GetDescription())
                    {
                        btnGroup.ChoosedButtonIndex = i;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 通过类型值，设置UIRadioButtonGroup的值；
        /// </summary>
        /// <param name="btnGroup"></param>
        /// <param name="obj"></param>
        public static void SetSelectValue(this UIRadioButtonGroup btnGroup, object obj)
        {
            int index = (btnGroup?.ButtonItems.ToList().FindIndex((btn) => btn.Tag.Equals(obj))).Value;
            btnGroup.ChoosedButtonIndex = index >= 0 ? index : throw new Exception("DisplayType:Setting exception, " + obj);
        }


        public static void SetType<T>(this ComboBoxEx comboBox, T choosedItem) where T : Enum
        {
            comboBox.DataSource = Enum.GetValues(typeof(T)).Cast<T>().Select(x => new KeyValuePair<string, T>(x.GetDescription(), x)).ToList();
            comboBox.DisplayMember = "Key";
            comboBox.ValueMember = "Value";
            comboBox.SelectedValue = choosedItem;
        }
        /// <summary>
        /// 通过枚举器绑定ComboBox控件选项值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="comboBox">控件</param>
        /// <param name="sources">枚举器</param>
        public static void SetType<T>(this ComboBoxEx comboBox, IEnumerable<T> sources) where T : Enum
        {
            if (sources == null || sources.Count() == 0)
            {
                throw new ArgumentNullException(nameof(sources));
            }
            comboBox.DataSource = sources.Select(x => new KeyValuePair<string, T>(x.GetDescription(), x)).ToList();
            comboBox.DisplayMember = "Key";
            comboBox.ValueMember = "Value";
            comboBox.SelectedValue = sources.First();
        }

        public static NumberKeybordForm UniformInitKeyBoard(this NumberKeybordForm kForm, ContainerControl owner, Control con = null)
        {
            kForm.StartPosition = FormStartPosition.Manual;
            if (con != null && con is ScopeXNumericEditBox ebox)
                ebox.IsFocusClicked = true;
            else if (con != null && con is ScopeXIconButton btn)
                btn.IsInputFocus = true;

            kForm.Load += (sender, e) =>
            {
                kForm.TopMost = true;
            };
            kForm.Shown += (sender, e) =>
            {
                kForm.TopMost = false;
            };

            kForm.FormClosed += (sender, e) =>
            {
                kForm = null;

                if (owner.ParentForm is FloatForm form)
                { 
                    form.Activate();
                    form.CanClose = true;
                    if (con != null && con is ScopeXNumericEditBox ebox)
                        ebox.IsFocusClicked = false;
                    else if (con != null && con is ScopeXIconButton btn)
                        btn.IsInputFocus = false;

                }
                //添加
                //对于filterdesigner 和码型参数设置界面等是form而不是page的情况，他们的owner为空
                if (owner.ParentForm == null && owner is FloatForm dform)
                {
                    dform.Activate();
                    dform.CanClose = true;
                    if (con != null && con is ScopeXNumericEditBox ebox)
                        ebox.IsFocusClicked = false;
                    else if (con != null && con is ScopeXIconButton btn)
                        btn.IsInputFocus = false;
                }
                //添加
            };
            kForm.NumberKeyboard.CancelEvent += (sender, args) =>
            {
                kForm.Close();
            };

            if (owner.ParentForm is FloatForm form)
            {
                form.CanClose = false;
            }
            var parent = GetOwnerOrParentAtt(owner);
            if (parent != null && kForm is FlashBorderForm)
            {
                parent.HasFlashChild = true;
            }

            return kForm;
        }

        public static NumberKeybordAwgForm UniformInitKeyBoard(this NumberKeybordAwgForm kForm, ContainerControl owner, Control con = null)
        {
            kForm.StartPosition = FormStartPosition.Manual;
            if (con != null && con is ScopeXNumericEditBox ebox)
                ebox.IsFocusClicked = true;
            else if (con != null && con is ScopeXIconButton btn)
                btn.IsInputFocus = true;
            kForm.Load += (sender, e) =>
            {
                kForm.TopMost = true;
            };
            kForm.Shown += (sender, e) =>
            {
                kForm.TopMost = false;
            };

            kForm.FormClosed += (sender, e) =>
            {
                kForm = null;

                if (owner.ParentForm is FloatForm form)
                {
                    form.Activate();
                    form.CanClose = true;
                    if (con != null && con is ScopeXNumericEditBox ebox)
                        ebox.IsFocusClicked = false;
                    else if (con != null && con is ScopeXIconButton btn)
                        btn.IsInputFocus = false;

                }
                //添加
                //对于filterdesigner 和码型参数设置界面等是form而不是page的情况，他们的owner为空
                if (owner.ParentForm == null && owner is FloatForm dform)
                {
                    dform.CanClose = true;
                    if (con != null && con is ScopeXNumericEditBox ebox)
                        ebox.IsFocusClicked = false;
                    else if (con != null && con is ScopeXIconButton btn)
                        btn.IsInputFocus = false;
                }
                //添加
            };
            kForm.NumberKeyboard.CancelEvent += (sender, args) =>
            {
                kForm.Close();
            };

            if (owner.ParentForm is FloatForm form)
            {
                form.CanClose = false;
            }
            var parent = GetOwnerOrParentAtt(owner);
            if (parent != null && kForm is FlashBorderForm)
            {
                parent.HasFlashChild = true;
            }

            return kForm;
        }

        private static FlashBorderForm GetOwnerOrParentAtt(Control owner)
        {
            if (owner is FlashBorderForm parent)
            {
                return parent;
            }
            else if (owner.Parent != null && owner.Parent is Control pctl)
            {
                return GetOwnerOrParentAtt(pctl);
            }

            return null;
        }

        public static NumberKeybordForm SetKeyBoardValue(this NumberKeybordForm kForm, String title, String uintString, Int32 decimalNumber
            , Action<Double> onOkClickEventAction, Double defaultValue, Double maxValue = Int32.MaxValue, Double minValue = Int32.MinValue,
            Boolean useSI = true, Boolean IsInteger = false)
        {
            kForm.Title = title;
            kForm.NumberKeyboard.Unit = uintString;
            kForm.NumberKeyboard.DecimalNumber = decimalNumber;
            kForm.NumberKeyboard.MaxValue = maxValue;
            kForm.NumberKeyboard.MinValue = minValue;
            kForm.NumberKeyboard.DefaultValue = defaultValue;
            kForm.NumberKeyboard.UseSI = useSI;
            kForm.NumberKeyboard.IsInteger = IsInteger;
            kForm.NumberKeyboard.OkClickEvent += (sender, args) =>
            {
                onOkClickEventAction(args.Data);
                kForm.Close();
            };

            return kForm;
        }

        public static NumberKeybordAwgForm SetKeyBoardValue(this NumberKeybordAwgForm kForm, String title, String uintString, Int32 decimalNumber
    , Action<Double> onOkClickEventAction, Double defaultValue, Double maxValue = Int32.MaxValue, Double minValue = Int32.MinValue,
            Double maxVrms = Int32.MaxValue, Double minVrms = Int32.MinValue, Double maxdBm = Int32.MaxValue, Double mindBm = Int32.MinValue,
    Boolean useSI = true, Boolean IsInteger = false)
        {
            kForm.Title = title;
            kForm.NumberKeyboard.DecimalNumber = decimalNumber;
            kForm.NumberKeyboard.MaxVpp = maxValue;
            kForm.NumberKeyboard.MinVpp = minValue;
            kForm.NumberKeyboard.MaxVrms = maxVrms;
            kForm.NumberKeyboard.MinVrms = minVrms;
            kForm.NumberKeyboard.MaxdBm = maxdBm;
            kForm.NumberKeyboard.MindBm = mindBm;
            kForm.NumberKeyboard.Unit = uintString;
            switch (uintString)
            {
                case "Vrms":
                    kForm.NumberKeyboard.MaxValue = maxVrms;
                    kForm.NumberKeyboard.MinValue = minVrms;
                    break;
                case "dBm":
                    kForm.NumberKeyboard.MaxValue = maxdBm;
                    kForm.NumberKeyboard.MinValue = mindBm;
                    break;
                case "Vpp":
                default:
                    kForm.NumberKeyboard.MaxValue = maxValue;
                    kForm.NumberKeyboard.MinValue = minValue;
                    break;
            }
            kForm.NumberKeyboard.DefaultValue = defaultValue;
            kForm.NumberKeyboard.UseSI = useSI;
            kForm.NumberKeyboard.IsInteger = IsInteger;
            kForm.NumberKeyboard.OkClickEvent += (sender, args) =>
            {
                onOkClickEventAction(args.Data);
                kForm.Close();
            };

            return kForm;
        }

        public static HexNumberKeyboardFrom UniformInitKeyBoard(this HexNumberKeyboardFrom kForm, ContainerControl owner)
        {
            kForm.StartPosition = FormStartPosition.Manual;
            kForm.Load += (sender, e) =>
            {
                kForm.TopMost = true;
            };
            kForm.Shown += (sender, e) =>
            {
                kForm.TopMost = false;
            };
            kForm.FormClosed += (sender, e) =>
            {
                kForm = null;
                if (owner.ParentForm is FloatForm form)
                {
                    form.CanClose = true;
                }
            };
            kForm.NumberKeyboard.CancelClick += (sender, args) =>
            {
                kForm.Close();
            };

            if (owner.ParentForm is FloatForm form)
            {
                form.CanClose = false;
            }

            return kForm;
        }

        public static HexNumberKeyboardFrom SetKeyBoardValue(this HexNumberKeyboardFrom kForm, String title
            , Action<Int64> onOkClickEventAction, Int64 defaultValue, Int64 maxValue = Int64.MaxValue, Int64 minValue = Int64.MinValue, HexValueType valueType = HexValueType.Hex)
        {
            kForm.Title = title;
            kForm.NumberKeyboard.MaxValue = maxValue;
            kForm.NumberKeyboard.MinValue = minValue;
            kForm.NumberKeyboard.Value = defaultValue;
            kForm.NumberKeyboard.ValueType = valueType;
            kForm.NumberKeyboard.OkClick += (sender, args) =>
            {
                onOkClickEventAction(args.Data);
                kForm.Close();
            };

            return kForm;
        }

        /// <summary>
        /// 调整窗体的大小，但是会有位置的变化，变化规律于窗体的朝向相关；
        /// </summary>
        /// <param name="form"></param>
        /// <param name="newSize"></param>
        public static void AdjustSize(this Form form, Size newSize)
        {
            Form appform = Program.Oscilloscope.View as DsoForm;
            Rectangle borderrect = new Rectangle(appform.Location, appform.Size);
            Size deltasize = newSize - form.Size;
            Point deltapos = new Point(0, 0);

            switch (form.Anchor)
            {
                case AnchorStyles.Top:
                    deltapos = new Point(-deltasize.Width / 2, 0);
                    break;
                case AnchorStyles.Bottom:
                    deltapos = new Point(deltasize.Width / 2, -deltasize.Height);
                    break;
                case AnchorStyles.Left:
                    deltapos = new Point(0, -deltasize.Height / 2);
                    break;
                case AnchorStyles.Right:
                    deltapos = new Point(-deltasize.Width, -deltasize.Height / 2);
                    break;
            }

            form.Size = newSize;

            //窗口位置要是超出左右上下边界时，调整窗口的位置
            Point AdjustPos(Point pos)
            {
                if (pos.X < borderrect.X)
                {
                    pos.X = borderrect.X;
                }
                else if (pos.X + form.Width > borderrect.X + borderrect.Width)
                {
                    pos.X = borderrect.X + borderrect.Width - form.Width;
                }

                if (pos.Y < borderrect.Y)
                {
                    pos.Y = borderrect.Y;
                }
                else if (pos.Y + form.Height > borderrect.Y + borderrect.Height)
                {
                    pos.Y = borderrect.Y + borderrect.Height - form.Height;
                }

                return pos;
            }

            if (form is FloatForm fform)
            {
                fform.StartPositionInOwner = AdjustPos(new Point(fform.StartPositionInOwner.X + deltapos.X, fform.StartPositionInOwner.Y + deltapos.Y));
                fform.StartPositionEx = AdjustPos(new Point(fform.StartPositionEx.X + deltapos.X, fform.StartPositionEx.Y + deltapos.Y));
            }
            form.Location = AdjustPos(new Point(form.Location.X + deltapos.X, form.Location.Y + deltapos.Y));
        }
    }
}
