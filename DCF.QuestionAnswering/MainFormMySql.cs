using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DCF.QuestionAnswering.Forms;
using DCF.DataLayer;
using System.Configuration;

namespace DCF.QuestionAnswering
{
    public partial class MainFormMySql : DCF.QuestionAnswering.MainForm
    {
        public const string UsernameSettingName = "DBUserName";
        public const string PasswordSettingName = "DBUserName";
        public const string DbNameSettingName = "DBName";
        public const string HostNameSettingName = "HostName";
        public MainFormMySql()
        {
            InitializeComponent();
        }

        private void MainFormMySql_Load(object sender, EventArgs e)
        {
            using (LoginForm lf = new LoginForm())
            {
                lf.UserName = ConfigurationManager.AppSettings[UsernameSettingName];
                lf.Password = ConfigurationManager.AppSettings[PasswordSettingName];
                DialogResult dr = lf.ShowDialog(this);
                if (dr == DialogResult.Cancel)
                {
                    Application.Exit();
                }
                string password = lf.Password;
                string username = lf.UserName;
                m_sqlUtils = new MySqlNativeClientUtils(username, password,
                    ConfigurationManager.AppSettings[DbNameSettingName], 
                    ConfigurationManager.AppSettings[HostNameSettingName]);
            }
            SqlUtils.Connect();

            SqlUtils.ExecuteQuery("SELECT * FROM QueryForms", m_dsQueryForms);
            _lboxQuestions.DataSource = m_dsQueryForms.Tables[0];
            _lboxQuestions.DisplayMember = "FormName";
            _lboxQuestions_SelectedIndexChanged(null, null);
        }
        protected MySqlUtils SqlUtils
        {
            get
            {
                return m_sqlUtils;
            }
        }
        private MySqlUtils m_sqlUtils;
        private DataSet m_dsQueryForms = new DataSet("QueryForms");

        private void OpenCurrentQuestionForm()
        {
            DataRowView currentRow = _lboxQuestions.SelectedValue as DataRowView;
            System.Diagnostics.Debug.Assert(currentRow != null);
            using (QueryFormMySql qf = new QueryFormMySql())
            {
                qf.SetQuestionText(
                    SqlUtils,
                    currentRow["FormTitle"].ToString(),
                    currentRow["FormQuestionText"].ToString(),
                    currentRow["ParamsTableName"].ToString(),
                    currentRow["ParamsColNames"].ToString(),
                    currentRow["QueryBody"].ToString(),
                    (TopicType)Convert.ToInt32(currentRow["TopicType"]));
                qf.ShowDialog(this);
            }

        }

        private void _lboxQuestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            _tbDescription.Text = ((DataRowView)_lboxQuestions.SelectedValue)["FormDescription"].ToString();
        }

        private void _lboxQuestions_DoubleClick(object sender, EventArgs e)
        {
            OpenCurrentQuestionForm();
        }

        private void _btnOpen_Click(object sender, EventArgs e)
        {
            OpenCurrentQuestionForm();
        }
    }
}
