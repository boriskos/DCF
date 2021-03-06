﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DCF.DataLayer;


namespace DCF.QuestionAnswering
{
    public partial class QueryFormMySql : DCF.QuestionAnswering.QueryForm
    {
        public struct ControlInfo
        {
            public Font Font;
            public Color ForeColor;
            public Color BackColor;
            public Padding Margins;
            public Size Size;
        }

        public QueryFormMySql()
        {
            InitializeComponent();
            // get label visual properties
            m_labelInfo.ForeColor = _flowLayoutPanelQueryText.Controls[0].ForeColor;
            m_labelInfo.Font =      _flowLayoutPanelQueryText.Controls[0].Font;
            m_labelInfo.Margins =   _flowLayoutPanelQueryText.Controls[0].Margin;
            m_labelInfo.BackColor = _flowLayoutPanelQueryText.Controls[0].BackColor;

            // get Combo Box visual properties
            m_comboBoxInfo.ForeColor =  _flowLayoutPanelQueryText.Controls[1].ForeColor;
            m_comboBoxInfo.Font =       _flowLayoutPanelQueryText.Controls[1].Font;
            m_comboBoxInfo.Margins =    _flowLayoutPanelQueryText.Controls[1].Margin;
            m_comboBoxInfo.BackColor =  _flowLayoutPanelQueryText.Controls[1].BackColor;
            m_comboBoxInfo.Size =       _flowLayoutPanelQueryText.Controls[1].Size;

            // clear
            _flowLayoutPanelQueryText.Controls.Clear();

            _dataGridViewQueryResults.CellFormatting += 
                new DataGridViewCellFormattingEventHandler(_dataGridViewQueryResults_CellFormatting);
        }

        public void SetQuestionText(MySqlUtils sqlUtils, 
            string title, string questionText, string paramTableName, 
            string paramColNames, string queryBody, TopicType topicType)
        {
            m_sqlUtils = sqlUtils;
            m_queryBody = queryBody;
            Text = title;
            m_topicType = topicType;
            DataSet ds = new DataSet();
            sqlUtils.ExecuteQuery(string.Format("SELECT {0} FROM {1}", paramColNames, paramTableName), ds);
            m_paramNames = paramColNames.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            m_tableName = paramTableName;
            int curPosition = 0;
            while (curPosition < questionText.Length)
            {
                int startParamIndex = questionText.IndexOf('{', curPosition);
                if (startParamIndex < 0) // if there is no more paraeters in the string
                {
                    string labelText = questionText.Substring(curPosition);
                    _flowLayoutPanelQueryText.Controls.Add(new Label() { Text = labelText, AutoSize = true, 
                        Font = m_labelInfo.Font, ForeColor = m_labelInfo.ForeColor, 
                        Margin = m_labelInfo.Margins, BackColor = Color.Transparent });
                    break;
                }
                if (startParamIndex > curPosition) // if there is a need in Label
                {
                    string labelText = questionText.Substring(curPosition, startParamIndex - curPosition);
                    _flowLayoutPanelQueryText.Controls.Add(new Label() { Text = labelText, AutoSize = true, 
                        Font = m_labelInfo.Font, ForeColor = m_labelInfo.ForeColor, 
                        Margin = m_labelInfo.Margins, BackColor = Color.Transparent });
                }
                int endParamIndex = questionText.IndexOf('}', startParamIndex);
                curPosition = endParamIndex + 1;
                int curParamInd = int.Parse(questionText.Substring(startParamIndex + 1, endParamIndex - startParamIndex - 1));
                BBBNOVA.BNComboBox cb = new BBBNOVA.BNComboBox()
                {
                    Size = m_comboBoxInfo.Size,
                    Font = m_comboBoxInfo.Font,
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    DataSource = ds.Tables[0],
                    DisplayMember = m_paramNames[curParamInd],
                    Margin = m_comboBoxInfo.Margins,
                    DropDownWidth = 300
                };
                cb.SelectedValueChanged += new EventHandler(comboBox_SelectedValueChanged);
                m_cbList.Add(cb);
                _flowLayoutPanelQueryText.Controls.Add(cb);
            } 
        }
        private void _btnQuery_Click(object sender, EventArgs e)
        {
            m_minScore = Double.MaxValue;
            m_maxScore = Double.MinValue;
            DataSet ds = new DataSet();
            List<string> paramsList = new List<string>(m_cbList.Count);
            foreach(Control c in m_cbList) paramsList.Add(c.Text);
            m_sqlUtils.ExecuteQuery(string.Format(m_queryBody, paramsList.ToArray()), ds);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                double cur = Convert.ToDouble(row["Confidence"]);
                if (cur < m_minScore) m_minScore = cur;
                if (cur > m_maxScore) m_maxScore = cur;
            }
            m_rowsNum = ds.Tables[0].Rows.Count;
            m_table = ds.Tables[0];
            _dataGridViewQueryResults.DataSource = m_table;
            _dataGridViewQueryResults.Focus();
        }

        private void _dataGridViewQueryResults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // if this is the score
            if (m_table!=null && m_table.Columns.Count > e.ColumnIndex && 
                m_table.Columns[e.ColumnIndex].ColumnName.Contains("onfidence") )
            {
                double d = Convert.ToDouble(e.Value);
                double normalized = d;
                // make normalization only for multiple answers
                if (m_topicType == TopicType.MultipleAnswers)
                {
                    normalized = (d - m_minScore);
                    if (normalized > 0) normalized /= (m_maxScore - m_minScore);

                    if (m_minScore == m_maxScore) normalized = 1;
                }

                e.CellStyle.BackColor = m_3_bins[(int)(2.9999 * normalized)];
                e.CellStyle.ForeColor = Color.Beige;
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
                e.Value = string.Format("{0:F2}%",Math.Round(normalized, 4)*100);
            }
        }

        private void comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            BBBNOVA.BNComboBox comboBox1 = (BBBNOVA.BNComboBox)sender;
            Font font = comboBox1.Font;
            Graphics g = comboBox1.CreateGraphics();
            comboBox1.Size = new Size((int)g.MeasureString(comboBox1.Text, font).Width + 20, comboBox1.Size.Height);
            _btnQuery_Click(null, null);
        }

        private void QueryFormMySql_Shown(object sender, EventArgs e)
        {
            _btnQuery_Click(null, null);
        }

        private Color[] m_3_bins = { 
                                       // red
                                       //Color.FromArgb(248, 105, 107), 
                                       Color.FromArgb(218,0,0),

                                       // yellow
                                       // Color.FromArgb(49, 132, 155), 
                                       // Color.FromArgb(222, 144, 0), 
                                       //Color.FromArgb(237, 187, 5), 
                                       Color.FromArgb(231, 166, 3), 

                                       // green
                                       //Color.FromArgb(99,190,123) 
                                       Color.FromArgb(0,168,0) 
                                   };

        private double m_minScore = Double.MaxValue;
        private double m_maxScore = Double.MinValue;
        private int m_rowsNum = 0;
        private DataTable m_table;


        private ControlInfo m_labelInfo;
        private ControlInfo m_comboBoxInfo;
        private string m_tableName;
        private string[] m_paramNames;
        private MySqlUtils m_sqlUtils;
        private string m_queryBody;
        private TopicType m_topicType;
        private List<BBBNOVA.BNComboBox> m_cbList = new List<BBBNOVA.BNComboBox>();
    }
}
