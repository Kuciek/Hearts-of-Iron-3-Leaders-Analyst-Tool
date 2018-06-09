using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace LeadersAnalystTool
{
    public partial class LoadingBar : Form
    {
        public LoadingBar()
        {
            InitializeComponent();
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            for (int i = MainForm.leaderFiles.Count - 1, j = 0; i >= 0; i--, j++)
            {
                MainForm.leaders = MainForm.leaders.Union(MainForm.Leader.LoadLeadersListFromFile(MainForm.leaderFiles[i], MainForm.leaders)).ToList();
                backgroundWorker.ReportProgress((j * 100) / MainForm.leaderFiles.Count, j);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label1.Text = string.Format("Loaded leader files: {0} / {1} ", e.UserState, MainForm.leaderFiles.Count);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }
    }
}
