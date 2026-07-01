namespace ScopeX.U2
{
    partial class HexNumberKeyboardFrom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Keyboard = new UserControls.HexNumberKeyboard();
            SuspendLayout();
            // 
            // Keyboard
            // 
            Keyboard.BackColor = System.Drawing.Color.FromArgb(41, 43, 50);
            Keyboard.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            Keyboard.Location = new System.Drawing.Point(2, 44);
            Keyboard.Margin = new System.Windows.Forms.Padding(0);
            Keyboard.MaxValue = long.MaxValue;
            Keyboard.MinValue = long.MinValue;
            Keyboard.Name = "Keyboard";
            Keyboard.Size = new System.Drawing.Size(444, 485);
            Keyboard.StyleFlags = UserControls.Style.StyleFlag.None;
            Keyboard.StylizeFlag = true;
            Keyboard.TabIndex = 0;
            Keyboard.Value = 0L;
            Keyboard.ValueType = ScopeX.Controls.Common.Structs.HexValueType.Bin;
            // 
            // HexNumberKeyboardFrom
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(448, 530);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 43, 50);
            Controls.Add(Keyboard);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            IconInterval = 20;
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            Name = "HexNumberKeyboardFrom";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "HexNumberKeyboard";
            Title = "HexNumberKeyboard";
            TitleColor = System.Drawing.Color.FromArgb(234, 234, 234);
            TitleFont = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 20;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(Keyboard, 0);
            ResumeLayout(false);
        }

        #endregion


        private ScopeX.UserControls.HexNumberKeyboard Keyboard;
    }
}