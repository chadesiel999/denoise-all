
namespace ScopeX.U2
{
    partial class NumberKeybordAwgForm
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
            Keyboard = new UserControls.NumberKeyboardAwg();
            SuspendLayout();
            Keyboard.SuspendLayout();
            // 
            // Keyboard
            // 
            Keyboard.AbsoluteMinValue = null;
            Keyboard.BackColor = System.Drawing.Color.FromArgb(41, 43, 50);
            Keyboard.DecimalNumber = 2;
            //Keyboard.DefaultValue = 0D;
            Keyboard.EnbleKeyBoardListen = true;
            Keyboard.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Keyboard.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            Keyboard.Location = new System.Drawing.Point(2, 45);
            Keyboard.Margin = new System.Windows.Forms.Padding(4);
            Keyboard.Name = "Keyboard";
            Keyboard.Size = new System.Drawing.Size(500, 468);
            Keyboard.StyleFlags = UserControls.Style.StyleFlag.None;
            Keyboard.StylizeFlag = true;
            Keyboard.TabIndex = 1;
            Keyboard.TabStop = true;
            //Keyboard.Unit = null;
            Keyboard.UseSI = true;
            // 
            // NumberKeybordForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(504, 525);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 43, 50);
            ControlBox = false;
            Controls.Add(Keyboard);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            IconInterval = 20;
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NumberKeybordForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "NumberKeybordForm";
            Title = "NumberKeybordForm";
            TitleColor = System.Drawing.Color.FromArgb(234, 234, 234);
            TitleFont = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 20;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(Keyboard, 0);
            Keyboard.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.NumberKeyboardAwg Keyboard;
    }
}