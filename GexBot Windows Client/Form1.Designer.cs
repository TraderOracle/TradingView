namespace GrabGex
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new Button();
            this.richTextBox1 = new RichTextBox();
            this.cbSymbol = new ComboBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.cbType = new ComboBox();
            this.label3 = new Label();
            this.txtConversion = new TextBox();
            this.label4 = new Label();
            this.cbVolOI = new ComboBox();
            this.chkState = new CheckBox();
            this.label5 = new Label();
            this.txtKey = new TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new Point(111, 299);
            this.button1.Margin = new Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new Size(107, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "Grab Gex";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += this.button1_Click;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new Point(233, 19);
            this.richTextBox1.Margin = new Padding(2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new Size(456, 648);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // cbSymbol
            // 
            this.cbSymbol.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbSymbol.FormattingEnabled = true;
            this.cbSymbol.Items.AddRange(new object[] { "SPX", "ES_SPX", "NDX", "NQ_NDX", "QQQ", "TQQQ", "AAPL", "TSLA", "MSFT", "AMZN", "NVDA", "META", "VIX", "GOOG", "IWM", "TLT", "GLD", "USO" });
            this.cbSymbol.Location = new Point(83, 16);
            this.cbSymbol.Margin = new Padding(2);
            this.cbSymbol.Name = "cbSymbol";
            this.cbSymbol.Size = new Size(135, 28);
            this.cbSymbol.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(17, 19);
            this.label1.Margin = new Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(62, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Symbol:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(27, 57);
            this.label2.Margin = new Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(43, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Type:";
            // 
            // cbType
            // 
            this.cbType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] { "zero", "full", "one" });
            this.cbType.Location = new Point(83, 54);
            this.cbType.Margin = new Padding(2);
            this.cbType.Name = "cbType";
            this.cbType.Size = new Size(135, 28);
            this.cbType.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new Point(42, 194);
            this.label3.Margin = new Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(85, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Conversion:";
            // 
            // txtConversion
            // 
            this.txtConversion.Location = new Point(143, 191);
            this.txtConversion.Margin = new Padding(2);
            this.txtConversion.Name = "txtConversion";
            this.txtConversion.Size = new Size(75, 27);
            this.txtConversion.TabIndex = 7;
            this.txtConversion.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new Point(17, 96);
            this.label4.Margin = new Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new Size(54, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Vol/OI:";
            // 
            // cbVolOI
            // 
            this.cbVolOI.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbVolOI.FormattingEnabled = true;
            this.cbVolOI.Items.AddRange(new object[] { "Volume", "Open Interest" });
            this.cbVolOI.Location = new Point(83, 93);
            this.cbVolOI.Margin = new Padding(2);
            this.cbVolOI.Name = "cbVolOI";
            this.cbVolOI.Size = new Size(135, 28);
            this.cbVolOI.TabIndex = 9;
            this.cbVolOI.SelectedIndexChanged += this.cbVolOI_SelectedIndexChanged;
            // 
            // chkState
            // 
            this.chkState.AutoSize = true;
            this.chkState.Location = new Point(143, 240);
            this.chkState.Name = "chkState";
            this.chkState.Size = new Size(65, 24);
            this.chkState.TabIndex = 10;
            this.chkState.Text = "State";
            this.chkState.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new Point(11, 141);
            this.label5.Margin = new Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new Size(62, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "API Key:";
            // 
            // txtKey
            // 
            this.txtKey.Location = new Point(84, 138);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new Size(134, 27);
            this.txtKey.TabIndex = 12;
            this.txtKey.Text = "A4HghGHZ62YB";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(704, 683);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chkState);
            this.Controls.Add(this.cbVolOI);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtConversion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbSymbol);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Margin = new Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Grab Gex";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Button button1;
        private RichTextBox richTextBox1;
        private ComboBox cbSymbol;
        private Label label1;
        private Label label2;
        private ComboBox cbType;
        private Label label3;
        private TextBox txtConversion;
        private Label label4;
        private ComboBox cbVolOI;
        private CheckBox chkState;
        private Label label5;
        private TextBox txtKey;
    }
}
