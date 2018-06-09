using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LeadersAnalystTool
{
    public partial class StartingForm : Form
    {
        int numberOfElements;
        string[] modDirectories;

        public StartingForm()
        {
            InitializeComponent();
        }

        private void Finish_Click(object sender, EventArgs e)
        {
            foreach (TreeNode parentdNode in triStateTreeView1.Nodes)
            {
                if (parentdNode.StateImageIndex == 1 || parentdNode.StateImageIndex == 2)
                {
                    foreach (TreeNode childNode in parentdNode.Nodes)
                    {
                        if (childNode.StateImageIndex == 1)
                        {
                            MainForm.leaderFiles.Add(parentdNode.Text + "\\" + childNode.Text); 
                        }
                    }
                }
            }
            this.Close();
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            cofd.IsFolderPicker = true;

            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox1.Text = cofd.FileName;
                numberOfElements = 0;
                triStateTreeView1.Nodes.Clear();
                CheckDirectory(cofd.FileName + "\\history\\leaders", true);
                CheckDirectory(cofd.FileName + "\\tfh\\history\\leaders", true);

                if(Directory.Exists(cofd.FileName + "\\tfh\\mod") == true && Directory.GetDirectories(cofd.FileName + "\\tfh\\mod").Length > 0)
                {
                    modDirectories = Directory.GetDirectories(cofd.FileName + "\\tfh\\mod");

                    for (int i = 0; i < Directory.GetDirectories(cofd.FileName + "\\tfh\\mod").Length; i++)
                    {
                        CheckDirectory(modDirectories[i] + "\\history\\leaders", false);
                    }
                }

                if (numberOfElements == 0)
                {
                    label3.Text = "Could not find any leader files!";
                    button2.Enabled = false;
                }
                else
                {
                    CountCheckedFiles();
                    button2.Enabled = true;
                }
            }

            this.Enabled = true;
        }

        private void CheckDirectory(string path, bool checkedStatus)
        {
            DirectoryInfo gameDirectory;
            FileInfo[] leaderFiles;

            if (Directory.Exists(path) == true && IsDirectoryEmpty(path) == false)
            {
                gameDirectory = new DirectoryInfo(path);
                leaderFiles = gameDirectory.GetFiles("*.txt");
                numberOfElements = numberOfElements + leaderFiles.Length;
                TreeNode tn = new TreeNode(path);

                for (int i = 0; i < leaderFiles.Length; i++)
                {
                    tn.Nodes.Add(leaderFiles[i].Name);

                    if (checkedStatus == true)
                    {
                        tn.Nodes[i].StateImageIndex = (int)RikTheVeggie.TriStateTreeView.CheckedState.Checked;
                    }
                    else
                    {
                        tn.Nodes[i].StateImageIndex = (int)RikTheVeggie.TriStateTreeView.CheckedState.UnChecked;
                    }
                }

                if (checkedStatus == true)
                {
                    tn.StateImageIndex = (int)RikTheVeggie.TriStateTreeView.CheckedState.Checked;
                }
                else
                {
                    tn.StateImageIndex = (int)RikTheVeggie.TriStateTreeView.CheckedState.UnChecked;
                }

                triStateTreeView1.Nodes.Add(tn);
            }
        }

        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFiles(path, "*.txt", SearchOption.TopDirectoryOnly).Any();
        }

        private void triStateTreeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ForeColor == SystemColors.GrayText)
            {
                e.Cancel = true;
            }
        }

        private void triStateTreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null && Array.Exists(modDirectories, dir => dir + "\\history\\leaders" == e.Node.Parent.Text) == true)
            {
                bool isCheckedOrMixed = false;

                foreach (TreeNode childNode in e.Node.Parent.Nodes)
                {
                    if (childNode.Checked == true)
                    {
                        isCheckedOrMixed = true;
                        break;
                    }
                }

                if(isCheckedOrMixed == true)
                {
                    foreach (TreeNode parentNode in triStateTreeView1.Nodes)
                    {
                        if (Array.Exists(modDirectories, dir => (dir + "\\history\\leaders" == parentNode.Text) && (dir + "\\history\\leaders" != e.Node.Parent.Text)) == true)
                        {
                            parentNode.ForeColor = SystemColors.GrayText;
                            parentNode.StateImageIndex = 0;

                            foreach (TreeNode childNode in parentNode.Nodes)
                            {
                                childNode.ForeColor = SystemColors.GrayText;
                                childNode.StateImageIndex = 0;
                            }
                        }
                    }
                }
                else
                {
                    foreach (TreeNode parentNode in triStateTreeView1.Nodes)
                    {
                        if (Array.Exists(modDirectories, dir => dir + "\\history\\leaders" == parentNode.Text) == true)
                        {
                            parentNode.ForeColor = SystemColors.WindowText;

                            foreach (TreeNode childNode in parentNode.Nodes)
                            {
                                childNode.ForeColor = SystemColors.WindowText;
                            }
                        }
                    }
                }

                CountCheckedFiles();
                return;
            }

            CountCheckedFiles();
        }

        private void CountCheckedFiles()
        {
            int numberOfCheckedElements = 0;

            foreach (TreeNode parentdNode in triStateTreeView1.Nodes)
            {
                if (parentdNode.StateImageIndex == 1 || parentdNode.StateImageIndex == 2)
                {
                    foreach (TreeNode childNode in parentdNode.Nodes)
                    {
                        if (childNode.StateImageIndex == 1)
                        {
                            numberOfCheckedElements++;
                        }
                    }
                }
            }

            label3.Text = "Number of leader files: " + numberOfCheckedElements.ToString() + " / " + numberOfElements.ToString();
        }
    }
}
