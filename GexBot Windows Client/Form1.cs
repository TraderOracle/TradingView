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
            foreach (JArray item in clientarray)
                if (Greek.Equals("none") && !item[1].Equals("0.0"))
                {
                    double dd = Convert.ToDouble(item[0]) * Convert.ToDouble(txtConversion.Text);
                    string pr = $"{dd:0.0}";
                    if (cbVolOI.SelectedIndex == 0)
                    {
                        richTextBox1.Text += pr.Replace("17", "!").Replace("18", "@").Replace("19", "#").Replace("20", "$").Replace("21", "*").Replace("22", "=").Replace(".35", "").Replace("0.0", "~").Replace("-0.", ":") +
                        "," +
                        item[1].ToString().Replace("17", "!").Replace("18", "@").Replace("19", "#").Replace("20", "$").Replace("21", "*").Replace("22", "=").Replace(".35", "").Replace("0.0", "~").Replace(",-0.", ":") + "\n";
                    }
                    else
                    {
                        richTextBox1.Text += pr.Replace("17", "!").Replace("18", "@").Replace("19", "#").Replace("20", "$").Replace("21", "*").Replace("22", "=").Replace(".35", "").Replace("0.0", "~").Replace("-0.", ":") +
                        "," +
                        item[2].ToString().Replace("17", "!").Replace("18", "@").Replace("19", "#").Replace("20", "$").Replace("21", "*").Replace("22", "=").Replace(".35", "").Replace("0.0", "~").Replace("-0.", ":") + "\n";
                    }
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetIt();
        }

        private void cbVolOI_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
