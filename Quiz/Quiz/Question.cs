using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

public enum Answer{ RIGHT,WRONG }

namespace Quiz
{
    public partial class Question : Form
    {
        int correct;
        public Question(string name, string question, string[] answers, int correct)
        {
            InitializeComponent();
            questionText.Text = question;
            this.correct = correct;
            this.Text = name;
            answer1.Text = answers[0];
            answer2.Text = answers[1];
            answer3.Text = answers[2];
            answer4.Text = answers[3];

            this.Enabled = true;
            this.TopMost = true; this.TopMost = false;

        }

        public delegate void OnAsweredEventHandler(Answer answer);

        public event OnAsweredEventHandler Aswered;

        private void ans1_Click(object sender, EventArgs e)
        {
            if (Aswered != null)
                if (correct == 1)
                    Aswered(Answer.RIGHT);
                else
                    Aswered(Answer.WRONG);                  
                
        }

        private void ans2_Click(object sender, EventArgs e)
        {
            if (Aswered != null)
                if (correct == 2)
                    Aswered(Answer.RIGHT);
                else
                    Aswered(Answer.WRONG); 
        }

        private void ans3_Click(object sender, EventArgs e)
        {
            if (Aswered != null)
                if (correct == 3)
                    Aswered(Answer.RIGHT);
                else
                    Aswered(Answer.WRONG); 
        }

        private void ans4_Click(object sender, EventArgs e)
        {
            if (Aswered != null)
                if (correct == 4)
                    Aswered(Answer.RIGHT);
                else
                    Aswered(Answer.WRONG); 
        }

        
    }
}
