using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DCF.QuestionAnswering
{
    public partial class QueryForm : Form
    {
        public QueryForm()
        {
            InitializeComponent();
        }

        protected virtual void _dataGridViewQueryResults_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            int colWidth = 0;
            foreach (DataGridViewColumn col in _dataGridViewQueryResults.Columns)
                colWidth += col.Width;
            int start_location = Math.Max( _splitContQuery.Panel2.Width / 2 - colWidth / 2, 0);
            Point oldLocation = _dataGridViewQueryResults.Location;
            _dataGridViewQueryResults.Location = new Point(start_location, 0);
            Size oldSize = _dataGridViewQueryResults.Size;
            _dataGridViewQueryResults.Size = new Size(oldLocation.X+oldSize.Width-start_location, oldSize.Height);
        }

    }
}
