using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Quiz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string[] awss = {"1","2","3","4"};
            Question question = new Question("Pergunta ", "isso é um pergunta", awss, 1);
            question.Aswered += aswered;
            this.Enabled = false;
            question.ShowDialog();
            this.Enabled = true;
        }

        void aswered(Answer ans)
        {
            if (ans == Answer.RIGHT)
                return;
            return;
        }
    }
}
