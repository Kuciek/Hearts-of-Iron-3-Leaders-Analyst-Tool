using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LeadersAnalystTool
{
    public partial class MainForm : Form
    {
        public static List<string> leaderFiles;
        public static List<Leader> leaders;
        public static DataTable table;

        public class Leader
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Country { get; set; }
            public string Type { get; set; }
            public int Skill { get; set; }
            public int Max_skill { get; set; }
            public double Loyalty { get; set; }
            public string Picture { get; set; }
            public DateTime Start_date { get; set; }
            public DateTime End_date { get; set; }
            public string Traits
            {
                get
                {
                    return string.Join(", ", this.ListofTraits);
                }
            }
            public List<string> ListofTraits { get; set; }

            public static List<Leader> LoadLeadersListFromFile(string path, List<Leader> leaders)
            {
                string Texts = File.ReadAllText(path, Encoding.Default);
                Texts = Regex.Replace(Texts, @"(#(?:[^\r]*|[^\n]*|[^\s]|(?<counter>#)|(?<-counter>\r\n))+(?(counter)(?!))\r\n)", " ");
                Texts = Regex.Replace(Texts, @"\s+", " ");
                string[] splitId = Regex.Split(Texts, @"(?<![\.\d])([\d]+)[\s]*=[\s]*");

                for (int i = 0; i < splitId.Length; i++)
                {
                    int leaderId;
                    if (string.IsNullOrEmpty(splitId[i]) == false && int.TryParse(splitId[i], out leaderId) == true && leaders.Exists(x => x.Id == leaderId) == false)
                    {
                        bool isValid = true;
                        string leaderName = "";
                        string leaderCountry = "";
                        string leaderType = "";
                        int leaderSkill = 0;
                        int leaderMaxSkill = 0;
                        double leaderLoyalty = 0.00;
                        string leaderPicture = "";
                        List<string> leaderTraits = new List<string>();
                        DateTime leaderStartDate = new DateTime();
                        DateTime leaderEndDate = new DateTime();

                        MatchCollection name = Regex.Matches(splitId[i + 1], @"(?<=\s|\{)(?i)name(?-i)[\s]*=[\s]*\""((?:[^\""]|(?<counter>\""{)|(?<-counter>\""}))+(?(counter)(?!)))\""(?<!\s|\})");
                        if (name.Count > 0)
                            leaderName = name[0].Groups[1].Value;
                        else
                            isValid = false;

                        MatchCollection country = Regex.Matches(splitId[i + 1], @"(?<=\s|\{)(?i)country(?-i)[\s]*=[\s]*(\w+)(?<!\s|\})");
                        if (country.Count > 0)
                            leaderCountry = country[0].Groups[1].Value;
                        else
                            isValid = false;

                        MatchCollection type = Regex.Matches(splitId[i + 1], @"(?<=\s|\{)(?i)type(?-i)[\s]*=[\s]*([^\s]+)(?<!\s|\})");
                        if (type.Count > 0)
                            leaderType = type[0].Groups[1].Value;
                        else
                            isValid = false;

                        MatchCollection skill = Regex.Matches(splitId[i + 1], @"(?<=\s|\{)(?i)skill(?-i)[\s]*=[\s]*([^\s]+)(?<!\s|\})");
                        if (skill.Count > 0)
                            leaderSkill = Convert.ToInt32(skill[0].Groups[1].Value);
                        else
                            isValid = false;

                        MatchCollection max_skill = Regex.Matches(splitId[i + 1], @"(?<=\s|\{)(?i)max_skill(?-i)[\s]*=[\s]*([^\s]+)(?<!\s|\})");
                        if (max_skill.Count > 0)
                            leaderMaxSkill = Convert.ToInt32(max_skill[0].Groups[1].Value);
                        else
                            isValid = false;

                        MatchCollection loyalty = Regex.Matches(splitId[i + 1], @"(?<=\s|\{)(?i)loyalty(?-i)[\s]*=[\s]*([^\s]+)(?<!\s|\})");
                        if (loyalty.Count > 0)
                            leaderLoyalty = Convert.ToDouble(loyalty[0].Groups[1].Value.Replace(".", ","));
                        else
                            isValid = false;

                        MatchCollection picture = Regex.Matches(splitId[i + 1], @"(?<=\s|\{)(?i)picture(?-i)[\s]*=[\s]*([^\s]+)(?<!\s|\})");
                        if (picture.Count > 0)
                            leaderPicture = picture[0].Groups[1].Value;
                        else
                            isValid = false;

                        MatchCollection traits = Regex.Matches(splitId[i + 1], @"(?<=\s|\{)(?i)add_trait(?-i)[\s]*=[\s]*([^\s]+)(?<!\s|\})");
                        foreach (Match trait in traits)
                            leaderTraits.Add(trait.Groups[1].Value);

                        MatchCollection histories = Regex.Matches(splitId[i + 1], @"history[\s]*=[\s]*\{((?:[^{}]|(?<counter>\{)|(?<-counter>\}))+(?(counter)(?!)))\}");
                        foreach (Match history in histories)
                        {
                            MatchCollection ranks = Regex.Matches(history.Groups[1].Value, @"(?<=\s|\{|\})(\d{4}).(\d{1,2}).(\d{1,2})[\s]*=[\s]*\{[\s]*(?i)rank(?-i)[\s]*=[\s]*([\d]+)[\s]*\}");
                            foreach (Match rank in ranks)
                            {
                                if (Convert.ToInt32(rank.Groups[2].Value) > 12 || Convert.ToInt32(rank.Groups[2].Value) > 31)
                                    continue;

                                string date = rank.Groups[1].Value + "/";
                                if (rank.Groups[2].Value.Length == 1)
                                    date = date + "0" + rank.Groups[2].Value + "/";
                                else
                                    date = date + rank.Groups[2].Value + "/";
                                if (rank.Groups[3].Value.Length == 1)
                                    date = date + "0" + rank.Groups[3].Value;
                                else
                                    date = date + rank.Groups[3].Value;


                                if (rank.Groups[4].Value == "0")
                                {
                                    if (leaderEndDate.Equals(DateTime.MinValue) == true)
                                        leaderEndDate = DateTime.Parse(date);
                                    else
                                        if (DateTime.Compare(leaderEndDate, DateTime.Parse(date)) == 1)
                                        leaderEndDate = DateTime.Parse(date);
                                }
                                else
                                {
                                    if (leaderStartDate.Equals(DateTime.MinValue) == true)
                                        leaderStartDate = DateTime.Parse(date);
                                    else
                                        if (DateTime.Compare(leaderStartDate, DateTime.Parse(date)) == 1)
                                        leaderStartDate = DateTime.Parse(date);
                                }
                            }
                        }

                        if (leaderStartDate.Equals(DateTime.MinValue) == true)
                            isValid = false;

                        if (isValid == true)
                        {
                            leaders.Add(new Leader
                            {
                                Id = leaderId,
                                Name = leaderName,
                                Country = leaderCountry,
                                Type = leaderType,
                                Skill = leaderSkill,
                                Max_skill = leaderMaxSkill,
                                Loyalty = leaderLoyalty,
                                Picture = leaderPicture,
                                ListofTraits = leaderTraits,
                                Start_date = leaderStartDate,
                                End_date = leaderEndDate
                            });
                        }
                    }
                }

                return leaders;
            }
        }

        public class Traits
        {
            public int Id { get; set; }

            public string Trait { get; set; }
        }

        public MainForm()
        {
            InitializeComponent();
            advancedDataGridView1.DoubleBuffered(true);
            Show();
            LoadFiles();
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)
        {
            leaderBindingSource.Filter = advancedDataGridView1.FilterString;
        }

        private void advancedDataGridView1_SortStringChanged(object sender, EventArgs e)
        {
            leaderBindingSource.Sort = advancedDataGridView1.SortString;
        }

        private void leaderBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            label1.Text = string.Format("Total rows: {0}", leaderBindingSource.List.Count);
        }

        private void file_manager_Click(object sender, EventArgs e)
        {
            LoadFiles();
        }

        private void LoadFiles()
        {
            leaderFiles = new List<string>(); ;
            leaders = new List<Leader>();
            table = new DataTable();

            StartingForm sf = new StartingForm();
            sf.ShowDialog();

            LoadingBar lb = new LoadingBar();
            lb.ShowDialog();

            table = ConvertToDataTable(leaders);
            leaderBindingSource.DataSource = table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            advancedDataGridView1.ClearSort(true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < advancedDataGridView1.Columns.Count; i++)
            {
                advancedDataGridView1.DisableFilter(advancedDataGridView1.Columns[i]);
                advancedDataGridView1.EnableFilter(advancedDataGridView1.Columns[i]);
            }
        }
    }

    public static class ExtensionMethods
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
}