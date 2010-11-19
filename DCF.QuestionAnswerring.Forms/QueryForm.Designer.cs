namespace DCF.QuestionAnswering
{
    partial class QueryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._splitContQuery = new System.Windows.Forms.SplitContainer();
            this._flowLayoutPanelQueryText = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this._panelQueryButtons = new System.Windows.Forms.Panel();
            this._btnDone = new System.Windows.Forms.Button();
            this._btnQuery = new System.Windows.Forms.Button();
            this._dataGridViewQueryResults = new System.Windows.Forms.DataGridView();
            this._splitContQuery.Panel1.SuspendLayout();
            this._splitContQuery.Panel2.SuspendLayout();
            this._splitContQuery.SuspendLayout();
            this._flowLayoutPanelQueryText.SuspendLayout();
            this._panelQueryButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridViewQueryResults)).BeginInit();
            this.SuspendLayout();
            // 
            // _splitContQuery
            // 
            this._splitContQuery.BackColor = System.Drawing.Color.DimGray;
            this._splitContQuery.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._splitContQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContQuery.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this._splitContQuery.Location = new System.Drawing.Point(0, 0);
            this._splitContQuery.Name = "_splitContQuery";
            this._splitContQuery.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _splitContQuery.Panel1
            // 
            this._splitContQuery.Panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_splitContQuery.Panel1.BackgroundImage")));
            this._splitContQuery.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._splitContQuery.Panel1.Controls.Add(this._flowLayoutPanelQueryText);
            this._splitContQuery.Panel1.Controls.Add(this._panelQueryButtons);
            // 
            // _splitContQuery.Panel2
            // 
            this._splitContQuery.Panel2.BackColor = System.Drawing.Color.Black;
            this._splitContQuery.Panel2.Controls.Add(this._dataGridViewQueryResults);
            this._splitContQuery.Size = new System.Drawing.Size(690, 335);
            this._splitContQuery.SplitterDistance = 110;
            this._splitContQuery.TabIndex = 0;
            // 
            // _flowLayoutPanelQueryText
            // 
            this._flowLayoutPanelQueryText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._flowLayoutPanelQueryText.BackColor = System.Drawing.Color.Transparent;
            this._flowLayoutPanelQueryText.Controls.Add(this.label1);
            this._flowLayoutPanelQueryText.Controls.Add(this.comboBox1);
            this._flowLayoutPanelQueryText.Location = new System.Drawing.Point(3, 3);
            this._flowLayoutPanelQueryText.Name = "_flowLayoutPanelQueryText";
            this._flowLayoutPanelQueryText.Size = new System.Drawing.Size(571, 98);
            this._flowLayoutPanelQueryText.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "The sample long text";
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.Black;
            this.comboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Bold);
            this.comboBox1.ForeColor = System.Drawing.Color.White;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Example 1",
            "Example 2"});
            this.comboBox1.Location = new System.Drawing.Point(162, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 27);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.Text = "Text";
            // 
            // _panelQueryButtons
            // 
            this._panelQueryButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._panelQueryButtons.BackColor = System.Drawing.Color.Transparent;
            this._panelQueryButtons.Controls.Add(this._btnDone);
            this._panelQueryButtons.Controls.Add(this._btnQuery);
            this._panelQueryButtons.Location = new System.Drawing.Point(580, 3);
            this._panelQueryButtons.Name = "_panelQueryButtons";
            this._panelQueryButtons.Size = new System.Drawing.Size(105, 103);
            this._panelQueryButtons.TabIndex = 0;
            // 
            // _btnDone
            // 
            this._btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnDone.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnDone.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnDone.FlatAppearance.BorderColor = System.Drawing.Color.Olive;
            this._btnDone.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkKhaki;
            this._btnDone.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Olive;
            this._btnDone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnDone.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Bold);
            this._btnDone.ForeColor = System.Drawing.Color.White;
            this._btnDone.Location = new System.Drawing.Point(5, 74);
            this._btnDone.Name = "_btnDone";
            this._btnDone.Size = new System.Drawing.Size(96, 28);
            this._btnDone.TabIndex = 1;
            this._btnDone.Text = "Done";
            this._btnDone.UseVisualStyleBackColor = true;
            // 
            // _btnQuery
            // 
            this._btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._btnQuery.BackColor = System.Drawing.Color.Transparent;
            this._btnQuery.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnQuery.FlatAppearance.BorderColor = System.Drawing.Color.Olive;
            this._btnQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkKhaki;
            this._btnQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Olive;
            this._btnQuery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnQuery.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Bold);
            this._btnQuery.ForeColor = System.Drawing.Color.White;
            this._btnQuery.Location = new System.Drawing.Point(5, 3);
            this._btnQuery.Name = "_btnQuery";
            this._btnQuery.Size = new System.Drawing.Size(97, 63);
            this._btnQuery.TabIndex = 0;
            this._btnQuery.Text = "Query";
            this._btnQuery.UseVisualStyleBackColor = false;
            // 
            // _dataGridViewQueryResults
            // 
            this._dataGridViewQueryResults.AllowUserToAddRows = false;
            this._dataGridViewQueryResults.AllowUserToDeleteRows = false;
            this._dataGridViewQueryResults.AllowUserToOrderColumns = true;
            this._dataGridViewQueryResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dataGridViewQueryResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this._dataGridViewQueryResults.BackgroundColor = System.Drawing.Color.Black;
            this._dataGridViewQueryResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._dataGridViewQueryResults.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.YellowGreen;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Yellow;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dataGridViewQueryResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._dataGridViewQueryResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Comic Sans MS", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dataGridViewQueryResults.DefaultCellStyle = dataGridViewCellStyle2;
            this._dataGridViewQueryResults.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._dataGridViewQueryResults.EnableHeadersVisualStyles = false;
            this._dataGridViewQueryResults.GridColor = System.Drawing.Color.DimGray;
            this._dataGridViewQueryResults.Location = new System.Drawing.Point(0, 0);
            this._dataGridViewQueryResults.MultiSelect = false;
            this._dataGridViewQueryResults.Name = "_dataGridViewQueryResults";
            this._dataGridViewQueryResults.ReadOnly = true;
            this._dataGridViewQueryResults.RowHeadersVisible = false;
            this._dataGridViewQueryResults.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this._dataGridViewQueryResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dataGridViewQueryResults.Size = new System.Drawing.Size(689, 222);
            this._dataGridViewQueryResults.TabIndex = 0;
            this._dataGridViewQueryResults.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this._dataGridViewQueryResults_DataBindingComplete);
            this._dataGridViewQueryResults.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dataGridViewQueryResults_CellContentClick);
            // 
            // QueryForm
            // 
            this.AcceptButton = this._btnQuery;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.CancelButton = this._btnDone;
            this.ClientSize = new System.Drawing.Size(690, 335);
            this.Controls.Add(this._splitContQuery);
            this.Name = "QueryForm";
            this.Text = "Binded Title";
            this._splitContQuery.Panel1.ResumeLayout(false);
            this._splitContQuery.Panel2.ResumeLayout(false);
            this._splitContQuery.ResumeLayout(false);
            this._flowLayoutPanelQueryText.ResumeLayout(false);
            this._flowLayoutPanelQueryText.PerformLayout();
            this._panelQueryButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridViewQueryResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer _splitContQuery;
        private System.Windows.Forms.Panel _panelQueryButtons;
        protected System.Windows.Forms.DataGridView _dataGridViewQueryResults;
        protected System.Windows.Forms.FlowLayoutPanel _flowLayoutPanelQueryText;
        protected System.Windows.Forms.Button _btnQuery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button _btnDone;

    }
}