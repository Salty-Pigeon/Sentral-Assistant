using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;



namespace Sentral_Assistant
{
    public partial class Form1 : Form
    {
        Dictionary<string, Func<Dictionary<string, string>, Task<string>>> services = new Dictionary<string, Func<Dictionary<string, string>, Task<string>>>();
        List<Dictionary<string, string>> schools = new List<Dictionary<string, string>>();

        static CookieContainer cookieContainer = new CookieContainer();
        private static HttpClientHandler httpClientHandler = new HttpClientHandler()
        {
            CookieContainer = cookieContainer
        };

        private static readonly HttpClient client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
        private static readonly HttpClient client2 = new HttpClient(handler: httpClientHandler, disposeHandler: true);


        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            checkBox1.Enabled = false;
            checkBox1.Visible = false;
            LoadSchools();
            LoadServices();
        }


        public async Task<string> CheckUnsubmitted( Dictionary<string, string> school )
        {
            outputBox.Text += "Running export \r\n";

            if (school["connected"] == "false") {
                client.CancelPendingRequests();
                await InitSentralConnection(school["sentral"]);
                school["connected"] = "true";
            }
            var date = DateTime.Now;
            if (checkBox1.Checked) {
                date = date.AddDays(-1);
            }
            if (date.ToString("dddd").ToLower() == "saturday") {
                date = date.AddDays(-1);
            } else if (date.ToString("dddd").ToLower() == "sunday") {
                date = date.AddDays(-2);
            }

            await ExportSentral(date.ToString("yyyy-MM-dd"), school["sentral"]);

            
            var teacherEmails = LoadEmails(school["code"]);
            var rolls = GetUnsubmittedRolls(teacherEmails);
            // Create list of teachers to email with unsubmitted rolls
            string emailOverall = "";
            string deputyEmail = "These are the unmarked rolls of " + date.ToString("dddd, dd MMMM yyyy") + "\n";

            foreach (var roll in rolls)
            {
                emailOverall += "\r\n" + UpperCase(roll.Key) + "\r\n";

                if ( checkBox1.Checked) {
                    deputyEmail += "\n" + UpperCase(roll.Key) + "\n";
                    foreach (var classDetail in roll.Value) {
                        deputyEmail += "      Period: " + classDetail["period"] + " Class: " + classDetail["class"] + "\n";
                        emailOverall += "      Period: " + classDetail["period"] + " Class: " + classDetail["class"] + "\r\n";
                    }
                } else {
                    string email = "Good afternoon,\n\nYou have an unsubmitted roll for the following class:\n";
                    if (roll.Value.Count > 1)
                        email = "Good afternoon,\n\nYou have unsubmitted rolls for the following classes:\n";

                    foreach (var classDetail in roll.Value)
                    {
                        email += "Period: " + classDetail["period"] + " Class: " + classDetail["class"] + "\n";
                        emailOverall += "      Period: " + classDetail["period"] + " Class: " + classDetail["class"] + "\r\n";
                    }

                    email += "\nPlease submit these ASAP\n\nIf this email is a mistake, email jason.reilly7@det.nsw.edu.au";
                    SendEmail(teacherEmails[roll.Key], "You have unsubmitted Sentral rolls " + DateTime.Now.ToString("dddd, dd MMMM yyyy"), email);

                }
            }

            if (demoBox.Checked) 
                MessageBox.Show("Emails weren't sent due to DEMO mode");
            outputBox.Text += emailOverall + "\r\n";
            outputBox.Text += "Emails sent!" + "\r\n";
            // outputBox.Text += emailOverall;
            
            return "";
        }

        public string UpperCase( string str)
        {
            return Regex.Replace(str, @"(?:(M|m)(c)|(\b))([a-z])", delegate (Match m) {
                return String.Concat(m.Groups[1].Value.ToUpper(), m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value.ToUpper());
            });
        }

        private Dictionary<string, string> LoadEmails( string schoolCode )
        {
            Dictionary<string, string> teacherEmails = new Dictionary<string, string>();
            using (var reader = new StreamReader(schoolCode + "/staffEmails.csv")) {

                while (!reader.EndOfStream) {
                    var line = reader.ReadLine();
                    var valuees = line.Split(',');
                    if (!teacherEmails.ContainsKey(valuees[0].ToLower())) {
                        teacherEmails.Add(valuees[0].ToLower(), valuees[1]);
                    }
                }

            }

            return teacherEmails;
        }

        private void LoadSchools()
        {
            schools = new List<Dictionary<string, string>>();
            List<string> schoolNames = new List<string>();
            using (var reader = new StreamReader("schools.csv"))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    schools.Add(new Dictionary<string, string>()
                    {
                        { "name", values[0] },
                        { "code", values[1] },
                        { "sentral", values[2] },
                        { "connected", "false" }
                    });
                    schoolNames.Add(values[0]);
                }
            }
            SchoolListBox.DataSource = schoolNames;
        }

        private void LoadServices()
        {
            services["Notify teachers of their unsubmitted rolls (PXP)"] = CheckUnsubmitted;

            servicesList.DataSource = services.Keys.ToArray();
        }


        private void ServicesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( servicesList.SelectedIndex == 0)
            {
                checkBox1.Text = "Email deputies yesterday's rolls.";
                checkBox1.Enabled = true;
                checkBox1.Visible = true;
            } else
            {
                checkBox1.Enabled = false;
                checkBox1.Visible = false;
            }
        }


        private Dictionary<string, List<Dictionary<string, string>>> GetUnsubmittedRolls(Dictionary<string, string> teacherEmails)
        {
            Dictionary<string, List<Dictionary<string, string>>> rolls = new Dictionary<string, List<Dictionary<string, string>>>();
            Dictionary<string, List<Dictionary<string, string>>> unsubmittedRolls = new Dictionary<string, List<Dictionary<string, string>>>();
            List<string> bannedClasses = new List<string>
                {
                    "YR9EYM",
                    "YR9AYM",
                    "YR9BYM",
                    "YR9CYM",
                    "YR9DYM",
                    "YR7DYM",
                    "YR7BYM",
                    "YR7CYM",
                    "YR7EYM",
                    "YR7AYM",
                    "YR10EYM",
                    "YR10AYM",
                    "YR10BYM",
                    "YR10CYM",
                    "YR10DYM",
                    "YR8EYM",
                    "YR8CYM",
                    "YR8BYM",
                    "YR8DYM",
                    "YR8AYM",
                    "FASMB1",
                    "FASMB2",
                    "FASMB3",
                    "FASMB4",
                    "FASMB5",
                    "FASMB6",
                    "FASMB7",
                    "FASMB8",
                    "FASMB9",
                    "FASMB10",
                    "FASMB11",
                    "FASMB12",
                    "FASMB13",
                    "FASMB14",
                    "FASMB15",
                    "MA8P311",
                    "MA10P327",
                    "MA8P312",
                    "MA10P328",
                    "MA8P313",
                    "MA8P314",
                    "MA8P38",
                    "MA8P39",
                    "MA9P315",
                    "MA9P316",
                    "MA9P317",
                    "MA7P31",
                    "MA9P318",
                    "MA7P32",
                    "MA9P321",
                    "MA7P33",
                    "MA9P322",
                    "MA7P34",
                    "MA10P323",
                    "MA7P35",
                    "MA10P324",
                    "MA7P36",
                    "MA10P325",
                    "MA7P37",
                    "MA10P326",
                    "XCT4",
                    "XCT4T",
                    "XCT5",
                    "XCT5T",
                    "XCT6",
                    "XCT6T",
                    "10Gnone",
                    "9COM01"
                };
            using (var reader = new StreamReader("export.csv"))
            {
                bool firstRun = true;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    var name = values[3].Replace("\"", "").ToLower();
                    if (name == "")
                        continue;
                    if ( !firstRun )
                    {
                        if (!rolls.ContainsKey(name))
                            rolls[name] = new List<Dictionary<string, string>>();     
                        
                        rolls[name].Add(new Dictionary<string, string>()
                        {
                            { "period", values[1].Replace("\"", "") },
                            { "class", values[2].Replace("\"", "") },
                            { "teacher", name },
                            { "year", values[4].Replace("\"", "") },
                            { "room", values[5].Replace("\"", "") },
                            { "status", values[6].Replace("\"", "") },
                        });
                    }
                    firstRun = false;
                }

            }

            foreach( var teachers in rolls )
            {
                foreach ( var roll in teachers.Value)
                {
                    if (bannedClasses.Contains(roll["class"]) || roll["room"] == "ASS" || roll["class"] == "7SRE" || roll["class"].Contains("Duty") || roll["period"] == "Assembly" || ( roll["year"] != "7" && roll["year"] != "8" && roll["year"] != "9" && roll["year"] != "10" && roll["year"] != "11" && roll["year"] != "12" && roll["year"] != "SU" ) )
                        continue;
                    if (!teacherEmails.ContainsKey(teachers.Key)) {
                        outputBox.Text += teachers.Key + " not found\r\n";
                        continue;
                    }
                    if (!unsubmittedRolls.ContainsKey(teachers.Key))
                        unsubmittedRolls[teachers.Key] = new List<Dictionary<string, string>>();
                    if (roll["status"] == "Unsubmitted")
                        unsubmittedRolls[teachers.Key].Add(roll);
                }
            }
            return unsubmittedRolls;
        }

        private void SendEmail(string email, string subject, string body)
        {
            if (demoBox.Checked) {
                return;
            }
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);

            mail.From = new MailAddress("dubbosouthsentral@gmail.com");
            mail.To.Add(email);
            mail.Subject = subject;
            mail.Body = body;
            SmtpServer.Credentials = new System.Net.NetworkCredential("dubbosouthsentral@gmail.com", "8417local");
            SmtpServer.EnableSsl = true;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                       System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                       System.Security.Cryptography.X509Certificates.X509Chain chain,
                       System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            SmtpServer.Send(mail);

        }

        public string sentralConnectionInitialised;
        public async Task<string> InitSentralConnection( string website )
        {
            //MessageBox.Show("Init Sentral " + website );
            sentralConnectionInitialised = website;
            Uri uri = new Uri(website + "/check_login");
            httpClientHandler.UseCookies = true;
            client.BaseAddress = uri;

            var values = new Dictionary<string, string>
            {
               { "sentral-username", "" },
               { "sentral-password", "" }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(uri, content);
            List<Cookie> cookies = cookieContainer.GetCookies(uri).Cast<Cookie>().ToList();

            return "";
        }

        public async Task<string> ExportSentral(string date, string website)
        {
            Uri uri = new Uri(website + "/attendancepxp/period/administration/roll_report?class_name=1&unsubmitted_only=1&range=single_day&date=" + date + "&start_date=" + date + "&end_date=" + date + "&export=1");

            var response = await client.GetAsync(uri);

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var fileInfo = new FileInfo("export.csv");
                fileInfo.Delete();
                using (var fileStream = fileInfo.OpenWrite())
                {
                    await stream.CopyToAsync(fileStream);
                }
            }

            return "";
        }


        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            outputBox.Clear();
            //Task t = InitSentralConnection(schools[SchoolListBox.SelectedIndex]["sentral"]);
            //t.WaitAsync(System.Threading.CancellationToken.None);
            services[servicesList.Text](schools[SchoolListBox.SelectedIndex]);
        }


    }
}
