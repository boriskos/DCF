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
            Point oldLocation = _dataGridViewQueryResults.Location;
            Size oldSize = _dataGridViewQueryResults.Size;

            int colWidth = 0;
            foreach (DataGridViewColumn col in _dataGridViewQueryResults.Columns)
            {
                colWidth += col.Width;
            }
            int absoluteHeight = _dataGridViewQueryResults.ColumnHeadersHeight;
            foreach (DataGridViewRow row in _dataGridViewQueryResults.Rows)
            {
                absoluteHeight += row.Height;
            }

            int start_location_x = Math.Max( _splitContQuery.Panel2.Width / 2 - colWidth / 2, 0);
            int start_location_y = Math.Max(_splitContQuery.Panel2.Height / 2 - absoluteHeight / 2, 40);

            int newHeight = Math.Min(_splitContQuery.Panel2.Height - 40 , absoluteHeight);

            _dataGridViewQueryResults.Location = new Point(start_location_x, start_location_y);
            _dataGridViewQueryResults.Size = new Size(oldLocation.X + oldSize.Width - start_location_x, newHeight);
        }

        private void QueryForm_Resize(object sender, EventArgs e)
        {
            _dataGridViewQueryResults_DataBindingComplete(null, null);
        }
    }
}
