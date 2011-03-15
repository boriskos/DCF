namespace DCF.QuestionAnswering
{
    partial class QueryFormMySql
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
            this.SuspendLayout();
            // 
            // _btnQuery
            // 
            this._btnQuery.FlatAppearance.BorderColor = System.Drawing.Color.Olive;
            this._btnQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkKhaki;
            this._btnQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Olive;
            this._btnQuery.Click += new System.EventHandler(this._btnQuery_Click);
            // 
            // QueryFormMySql
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(1184, 664);
            this.Name = "QueryFormMySql";
            this.Shown += new System.EventHandler(this.QueryFormMySql_Shown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
