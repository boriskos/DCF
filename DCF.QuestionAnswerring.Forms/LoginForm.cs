using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DCF.QuestionAnswering.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public string Password
        {
            get
            {
                return _textBoxPassword.Text;
            }
            set
            {
                _textBoxPassword.Text = value;
            }
        }
        public string UserName
        {
            get
            {
                return _textBoxUsername.Text;
            }
            set
            {
                _textBoxUsername.Text = value;
            }
        }
    }
}
