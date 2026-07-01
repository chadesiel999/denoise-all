using System;
using System.Threading.Channels;
using System.Windows.Forms;
using ScopeX.U2.BaseControl;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    partial class DecodeEventTableForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DecodeEventTableForm));
            ScopeX.UserControls.RadioButtonItem rB1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem rB2 = new ScopeX.UserControls.RadioButtonItem();
            Content = new System.Windows.Forms.Panel();
            RbChannel = new UIRadioButtonGroup();
            Table = new Panel();
            EventTablePage = new EventTablePage();
            // 
            // tableLayoutPanel1
            // 
            Content.Controls.Add(RbChannel);
            Content.Controls.Add(Table);
            Content.Controls.Add(PageControl);
            //tableLayoutPanel1.Controls.Add(TbcEvent, 0, 0);
            Content.Dock = System.Windows.Forms.DockStyle.Fill;
            Content.Location = new System.Drawing.Point(0, 45);
            Content.Name = "Content";
            //
            // RbChannel
            //
            this.RbChannel.BackColor = System.Drawing.Color.Black;
            this.RbChannel.BorderColor = System.Drawing.Color.Black;
            this.RbChannel.BorderThickness = 0;
            this.RbChannel.ButtonBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.RbChannel.ButtonFont = null;
            rB1.Icon = null;
            rB1.Padding = new System.Windows.Forms.Padding(0);
            rB1.Tag = null;
            rB1.Text = "B1";
            rB2.Icon = null;
            rB2.Padding = new System.Windows.Forms.Padding(0);
            rB2.Tag = null;
            rB2.Text = "B2";
            if (!PlatformUIManager.Default.Platform.Attribute.MutiBus)
            {
                this.RbChannel.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] {
            rB1};
            }
            else
            {
                this.RbChannel.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] {
            rB1,
            rB2};
            }
            this.RbChannel.ButtonOffset = 10;
            this.RbChannel.ButtonTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.RbChannel.ChoosedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.RbChannel.ChoosedButtonIndex = -1;
            this.RbChannel.ChoosedButtonTextColor = System.Drawing.Color.Black;
            this.RbChannel.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.RbChannel.ContentPadding = new System.Windows.Forms.Padding(0);
            this.RbChannel.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.RbChannel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RbChannel.Height = 30;
            this.RbChannel.MaximumSize = new System.Drawing.Size(10000, 30);
            this.RbChannel.MinimumSize = new System.Drawing.Size(200, 30);
            this.RbChannel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.RbChannel.Name = "RbChannel";
            this.RbChannel.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.RbChannel.StylizeFlag = true;
            this.RbChannel.TabIndex = 1;
            this.RbChannel.Size = new System.Drawing.Size(550, 30);
            this.RbChannel.Location = new System.Drawing.Point(0, 0);
            this.RbChannel.IndexChanged += new System.EventHandler(this.RbChannel_IndexChanged);
            //
            //Table
            //
            Table.Controls.Add(EventTablePage);
            Table.AutoScroll = true;
            Table.Size = new System.Drawing.Size(550, 445);
            Table.Location = new System.Drawing.Point(0, 95);
            Table.SizeChanged += Table_SizeChanged;
            // 
            // EventTablePage
            // 
            EventTablePage.Scrollable = false;
            EventTablePage.Font = AppStyleConfig.DefaultContextFont;
            EventTablePage.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            EventTablePage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            EventTablePage.ForeColor = System.Drawing.Color.FromArgb(234, 234, 234);
            EventTablePage.FullRowSelect = true;
            EventTablePage.MultiSelect = false;
            EventTablePage.GridLines = true;
            EventTablePage.Location = new System.Drawing.Point(0, 0);
            EventTablePage.Margin = new System.Windows.Forms.Padding(0);
            EventTablePage.Name = "EventTablePage";
            EventTablePage.OwnerDraw = true;
            EventTablePage.Size = new System.Drawing.Size(474, 581);
            EventTablePage.TabIndex = 0;
            EventTablePage.UseCompatibleStateImageBehavior = false;
            EventTablePage.View = System.Windows.Forms.View.Details;
            EventTablePage.SizeChanged += EventTablePage_SizeChanged;
            EventTablePage.SelectedIndexChanged += EventTablePage_SelectedIndexChanged;
            //
            //PageControl
            //
            PageControl.Location = new System.Drawing.Point(0, 520);
            PageControl.Size = new System.Drawing.Size(550, 30);

            // 
            // DecodeEventTableForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(42, 42, 42);
            CanClose = false;
            ClientSize = new System.Drawing.Size(550, 600);
            ContentBackColor = System.Drawing.Color.FromArgb(42, 42, 42);
            Controls.Add(Content);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(42, 42, 42);
            IsShowPin = false;
            Name = "DecodeEventTableForm";
            ShowInTaskbar = false;
            TitleLableHeight = 38;
            Controls.SetChildIndex(Content, 0);

            Content.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion


        private System.Windows.Forms.Panel Content;
        private UIRadioButtonGroup RbChannel;
        private System.Windows.Forms.Panel Table;
        private EventTablePage EventTablePage;
        private PageControl PageControl;
    }
}