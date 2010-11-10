using System;
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
        }

        public QueryFormMySql()
        {
            InitializeComponent();
            // get label visual properties
            m_labelInfo.ForeColor = _flowLayoutPanelQueryText.Controls[0].ForeColor;
            m_labelInfo.Font = _flowLayoutPanelQueryText.Controls[0].Font;
            m_labelInfo.Margins = _flowLayoutPanelQueryText.Controls[0].Margin;
            m_labelInfo.BackColor = _flowLayoutPanelQueryText.Controls[0].BackColor;

            // get Combo Box visual properties
            m_comboBoxInfo.ForeColor = _flowLayoutPanelQueryText.Controls[1].ForeColor;
            m_comboBoxInfo.Font = _flowLayoutPanelQueryText.Controls[1].Font;
            m_comboBoxInfo.Margins = _flowLayoutPanelQueryText.Controls[1].Margin;
            m_labelInfo.BackColor = _flowLayoutPanelQueryText.Controls[1].BackColor;

            // clear
            _flowLayoutPanelQueryText.Controls.Clear();
        }

        public void SetQuestionText(MySqlUtils sqlUtils, string title, string questionText, string paramTableName, string paramColNames, string queryBody)
        {
            m_sqlUtils = sqlUtils;
            m_queryBody = queryBody;
            Text = title;
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
                ComboBox cb = new ComboBox()
                {
                    DataSource = ds.Tables[0],
                    DisplayMember = m_paramNames[curParamInd],
                    Font = m_comboBoxInfo.Font,
                    ForeColor = m_comboBoxInfo.ForeColor,
                    Margin = m_comboBoxInfo.Margins,
                    BackColor = m_comboBoxInfo.BackColor,
                    FlatStyle = FlatStyle.Flat
                };
                m_cbList.Add(cb);
                _flowLayoutPanelQueryText.Controls.Add(cb);
            } 
        }

        private ControlInfo m_labelInfo;
        private ControlInfo m_comboBoxInfo;
        private string m_tableName;
        private string[] m_paramNames;
        private MySqlUtils m_sqlUtils;
        private string m_queryBody;
        private List<ComboBox> m_cbList = new List<ComboBox>();

        private void _btnQuery_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            List<string> paramsList = new List<string>(m_cbList.Count);
            foreach(Control c in m_cbList) paramsList.Add(c.Text);
            m_sqlUtils.ExecuteQuery(string.Format(m_queryBody, paramsList.ToArray()), ds);
            _dataGridViewQueryResults.DataSource = ds.Tables[0];
        }
    }
}
