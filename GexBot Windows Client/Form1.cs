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
            cbGreek.SelectedIndex = 0;
        }

        public async Task GetIt()
        {
            string Greek = cbGreek.Items[cbGreek.SelectedIndex].ToString().ToLower().Split(' ')[0];
            if (cbGreek.SelectedIndex > 3)
                Greek = "one" + Greek;
            if (Greek.Equals("none"))
                Greek = cbType.Items[cbType.SelectedIndex].ToString();
            string sCS = chkState.Checked || cbGreek.SelectedIndex != 0 ? "state" : "classic";
            string sSection = Greek.Equals("none") ? "strikes" : "mini_contracts";

            HttpClient http = new HttpClient();
            string toSend = "https://api.gexbot.com/" +
                    cbSymbol.Items[cbSymbol.SelectedIndex] + "/" +
                    sCS + "/" +
                    Greek + "?key=" +
                    txtKey.Text.Trim();
            HttpResponseMessage response = await http.GetAsync(toSend);
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();
            JObject jo = JObject.Parse(jsonResponse);
            var clientarray = jo[sSection].Value<JArray>();
            richTextBox1.Clear();
            int ix = 1;
            foreach (JArray item in clientarray)
                if (Greek.Equals("none"))
                {
                    double dd = Convert.ToDouble(item[0]) * Convert.ToDouble(txtConversion.Text);
                    string pr = $"{dd:0.0}";
                    double d1 = Convert.ToDouble(item[1]) * Convert.ToDouble(txtConversion.Text);
                    string one = $"{d1:0.00}";
                    double d2 = Convert.ToDouble(item[2]) * Convert.ToDouble(txtConversion.Text);
                    string two = $"{d2:0.00}";
                    if (cbVolOI.SelectedIndex == 0 && d1 != 0)
                    {
                        if (ix > 1) richTextBox1.Text += "\n";
                        richTextBox1.Text += Encode(pr) + "," + Encode(one);
                    }
                    else if (d2 != 0)
                    {
                        if (ix > 1) richTextBox1.Text += "\n";
                        richTextBox1.Text += Encode(pr) + "," + Encode(two);
                    }
                    ix++;
                }
            else
                {
                    double dd = Convert.ToDouble(item[0]) * Convert.ToDouble(txtConversion.Text);
                    string pr = $"{dd:0.0}";
                    double d1 = Convert.ToDouble(item[1]) * Convert.ToDouble(txtConversion.Text);
                    string one = $"{d1:0.00}";
                    double d2 = Convert.ToDouble(item[2]) * Convert.ToDouble(txtConversion.Text);
                    string two = $"{d2:0.00}";
                    double d3 = Convert.ToDouble(item[3]) * Convert.ToDouble(txtConversion.Text);
                    string three = $"{d3:0.00}";
                    if (d3 != 0)
                    {
                        if (ix > 1) richTextBox1.Text += "\n";
                        richTextBox1.Text += Encode(pr) + "," + Encode(three);
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
                .Replace("-0.", ":")
                .Replace(".1", "E");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetIt();
        }

    }
}
