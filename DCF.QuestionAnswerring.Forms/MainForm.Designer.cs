namespace DCF.QuestionAnswering
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._chooseFormLbl = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this._btnOpen = new System.Windows.Forms.Button();
            this._btnCreateNew = new System.Windows.Forms.Button();
            this._btnExit = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._lblItem = new System.Windows.Forms.Label();
            this._lboxQuestions = new System.Windows.Forms.ListBox();
            this._tbDescription = new System.Windows.Forms.TextBox();
            this._itemDescLbl = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _chooseFormLbl
            // 
            this._chooseFormLbl.AutoSize = true;
            this._chooseFormLbl.Dock = System.Windows.Forms.DockStyle.Top;
            this._chooseFormLbl.Font = new System.Drawing.Font("Comic Sans MS", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._chooseFormLbl.ForeColor = System.Drawing.Color.Khaki;
            this._chooseFormLbl.Location = new System.Drawing.Point(0, 0);
            this._chooseFormLbl.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this._chooseFormLbl.Name = "_chooseFormLbl";
            this._chooseFormLbl.Padding = new System.Windows.Forms.Padding(5, 5, 0, 10);
            this._chooseFormLbl.Size = new System.Drawing.Size(543, 60);
            this._chooseFormLbl.TabIndex = 100;
            this._chooseFormLbl.Text = "Please choose or create a question";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel1.Controls.Add(this._btnOpen);
            this.panel1.Controls.Add(this._btnCreateNew);
            this.panel1.Controls.Add(this._btnExit);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 677);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1184, 87);
            this.panel1.TabIndex = 2;
            // 
            // _btnOpen
            // 
            this._btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btnOpen.BackColor = System.Drawing.Color.Olive;
            this._btnOpen.FlatAppearance.BorderColor = System.Drawing.Color.Olive;
            this._btnOpen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this._btnOpen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkKhaki;
            this._btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnOpen.Font = new System.Drawing.Font("Comic Sans MS", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnOpen.ForeColor = System.Drawing.Color.White;
            this._btnOpen.Location = new System.Drawing.Point(5, 12);
            this._btnOpen.Name = "_btnOpen";
            this._btnOpen.Size = new System.Drawing.Size(127, 55);
            this._btnOpen.TabIndex = 1;
            this._btnOpen.Text = "Open";
            this._btnOpen.UseVisualStyleBackColor = false;
            // 
            // _btnCreateNew
            // 
            this._btnCreateNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btnCreateNew.BackColor = System.Drawing.Color.Olive;
            this._btnCreateNew.FlatAppearance.BorderColor = System.Drawing.Color.Olive;
            this._btnCreateNew.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this._btnCreateNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkKhaki;
            this._btnCreateNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCreateNew.Font = new System.Drawing.Font("Comic Sans MS", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnCreateNew.ForeColor = System.Drawing.Color.White;
            this._btnCreateNew.Location = new System.Drawing.Point(156, 12);
            this._btnCreateNew.Name = "_btnCreateNew";
            this._btnCreateNew.Size = new System.Drawing.Size(161, 55);
            this._btnCreateNew.TabIndex = 10;
            this._btnCreateNew.Text = "Create new question";
            this._btnCreateNew.UseVisualStyleBackColor = false;
            this._btnCreateNew.Visible = false;
            // 
            // _btnExit
            // 
            this._btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnExit.BackColor = System.Drawing.Color.Olive;
            this._btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnExit.FlatAppearance.BorderColor = System.Drawing.Color.Olive;
            this._btnExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this._btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkKhaki;
            this._btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnExit.Font = new System.Drawing.Font("Comic Sans MS", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnExit.ForeColor = System.Drawing.Color.White;
            this._btnExit.Location = new System.Drawing.Point(1054, 12);
            this._btnExit.Name = "_btnExit";
            this._btnExit.Size = new System.Drawing.Size(123, 55);
            this._btnExit.TabIndex = 2;
            this._btnExit.Text = "Exit";
            this._btnExit.UseVisualStyleBackColor = false;
            this._btnExit.Click += new System.EventHandler(this._btnExit_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1184, 617);
            this.panel2.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(50);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._lblItem);
            this.splitContainer1.Panel1.Controls.Add(this._lboxQuestions);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._tbDescription);
            this.splitContainer1.Panel2.Controls.Add(this._itemDescLbl);
            this.splitContainer1.Size = new System.Drawing.Size(1184, 617);
            this.splitContainer1.SplitterDistance = 394;
            this.splitContainer1.TabIndex = 4;
            // 
            // _lblItem
            // 
            this._lblItem.AutoSize = true;
            this._lblItem.Dock = System.Windows.Forms.DockStyle.Top;
            this._lblItem.Font = new System.Drawing.Font("Comic Sans MS", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblItem.ForeColor = System.Drawing.Color.Gold;
            this._lblItem.Location = new System.Drawing.Point(0, 0);
            this._lblItem.Name = "_lblItem";
            this._lblItem.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this._lblItem.Size = new System.Drawing.Size(176, 45);
            this._lblItem.TabIndex = 100;
            this._lblItem.Text = "Question:";
            // 
            // _lboxQuestions
            // 
            this._lboxQuestions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lboxQuestions.BackColor = System.Drawing.Color.Black;
            this._lboxQuestions.Font = new System.Drawing.Font("Comic Sans MS", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lboxQuestions.ForeColor = System.Drawing.Color.White;
            this._lboxQuestions.FormattingEnabled = true;
            this._lboxQuestions.ItemHeight = 40;
            this._lboxQuestions.Location = new System.Drawing.Point(0, 48);
            this._lboxQuestions.Name = "_lboxQuestions";
            this._lboxQuestions.Size = new System.Drawing.Size(387, 524);
            this._lboxQuestions.TabIndex = 0;
            // 
            // _tbDescription
            // 
            this._tbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbDescription.BackColor = System.Drawing.Color.Black;
            this._tbDescription.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._tbDescription.Font = new System.Drawing.Font("Comic Sans MS", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tbDescription.ForeColor = System.Drawing.Color.White;
            this._tbDescription.Location = new System.Drawing.Point(5, 48);
            this._tbDescription.Multiline = true;
            this._tbDescription.Name = "_tbDescription";
            this._tbDescription.ReadOnly = true;
            this._tbDescription.Size = new System.Drawing.Size(775, 567);
            this._tbDescription.TabIndex = 100;
            this._tbDescription.TabStop = false;
            // 
            // _itemDescLbl
            // 
            this._itemDescLbl.AutoSize = true;
            this._itemDescLbl.Dock = System.Windows.Forms.DockStyle.Top;
            this._itemDescLbl.Font = new System.Drawing.Font("Comic Sans MS", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._itemDescLbl.ForeColor = System.Drawing.Color.Gold;
            this._itemDescLbl.Location = new System.Drawing.Point(0, 0);
            this._itemDescLbl.Name = "_itemDescLbl";
            this._itemDescLbl.Padding = new System.Windows.Forms.Padding(5, 0, 0, 10);
            this._itemDescLbl.Size = new System.Drawing.Size(211, 55);
            this._itemDescLbl.TabIndex = 100;
            this._itemDescLbl.Text = "Description:";
            // 
            // MainForm
            // 
            this.AcceptButton = this._btnOpen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this._btnExit;
            this.ClientSize = new System.Drawing.Size(1184, 764);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._chooseFormLbl);
            this.MinimumSize = new System.Drawing.Size(500, 200);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Question Answering";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _chooseFormLbl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label _itemDescLbl;
        private System.Windows.Forms.Button _btnExit;
        private System.Windows.Forms.Label _lblItem;
        protected System.Windows.Forms.ListBox _lboxQuestions;
        protected System.Windows.Forms.TextBox _tbDescription;
        protected System.Windows.Forms.Button _btnCreateNew;
        protected System.Windows.Forms.Button _btnOpen;
    }
}

