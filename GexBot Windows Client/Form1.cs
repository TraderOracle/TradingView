using Newtonsoft.Json.Linq;


namespace GrabGex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cbType.SelectedIndex = 0;
            cbSymbol.SelectedIndex = 3;
            cbVolOI.SelectedIndex = 1;
        }

        public async Task GetIt()
        {
            string Greek = "none";
            string sCS = chkState.Checked ? "state" : "classic";

            HttpClient http = new HttpClient();
            HttpResponseMessage response = await http.GetAsync("https://api.gexbot.com/" +
                    cbSymbol.Items[cbSymbol.SelectedIndex] + "/" +
                    sCS + "/" +
                    cbType.Items[cbType.SelectedIndex] + "?key=" +
                    txtKey.Text.Trim());
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();
            JObject jo = JObject.Parse(jsonResponse);
            string sSection = "strikes";
            var clientarray = jo[sSection].Value<JArray>();
            richTextBox1.Clear();
            int ix = 1;
            foreach (JArray item in clientarray)
                if (Greek.Equals("none") && !item[1].Equals("0.0"))
                {
                    double dd = Convert.ToDouble(item[0]) * Convert.ToDouble(txtConversion.Text);
                    string pr = $"{dd:0.0}";
                    double d1 = Convert.ToDouble(item[1]) * Convert.ToDouble(txtConversion.Text);
                    string one = $"{d1:0.00}";
                    double d2 = Convert.ToDouble(item[2]) * Convert.ToDouble(txtConversion.Text);
                    string two = $"{d2:0.00}";
                    if (cbVolOI.SelectedIndex == 0)
                    {
                        if (ix > 1) richTextBox1.Text += "\n";
                        richTextBox1.Text += Encode(pr) + "," + Encode(one);
                    }
                    else
                    {
                        if (ix > 1) richTextBox1.Text += "\n";
                        richTextBox1.Text += Encode(pr) + "," + Encode(two);
                    }
                    ix++;
                }
            Clipboard.SetText(richTextBox1.Text);
        }

        private string Encode(string s)
        {
            return s.Replace("53", "D")
                .Replace("52", "|")
                .Replace("54", "A")
                .Replace("55", "B")
                .Replace("56", "C")
                .Replace("17", "!")
                .Replace("18", "@")
                .Replace("19", "#")
                .Replace("20", "$")
                .Replace("21", "*")
                .Replace("22", "=")
                .Replace(".35", "")
                .Replace("0.0", "~")
                .Replace("-0.", ":");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetIt();
        }

    }
}
