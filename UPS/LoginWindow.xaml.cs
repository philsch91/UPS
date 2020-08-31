using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.Collections;
using PSNetworking;

namespace UPS {
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow: Window {
        
        public Dictionary<string,string> Settings { get; set; }
        private delegate void UpdateWindow(Window window, string value);

        public LoginWindow() {
            InitializeComponent();

            textBoxUser.Text = System.Environment.UserName.ToUpper();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e) {
            if (this.textBoxUser.Text.Length == 0 || this.passwordBox.Password.Length == 0) {
                MessageBox.Show("Invalid credentials", "Invalid Credentials", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (this.debugCheckBox.IsChecked == true) {
                AppSettings.shared.Debug = true;
                //MessageBox.Show(AppSettings.shared.Debug.ToString());
            }

            this.loginSingleSignOn();
            //this.login();
            
            //MainWindow mw = (MainWindow)Application.Current.MainWindow;
            MainWindow mw = (MainWindow)this.Owner;
            //MainWindow mw = (MainWindow)this.DataContext;

            mw.textBlockLoginStatus.Text = "Logged in as " + AppSettings.shared.UserId;

            //UpdateWindow update = this.UpdateMainWindowLoginStatus;
            //Application.Current.Dispatcher.Invoke(update, mw, userID);
            
            this.DialogResult = true;
            this.Close();
        }

        private void loginSingleSignOn() {
            ////this.webclient = new CookieAwareWebClient();       //important delete
            //webclient.UseDefaultCredentials = true;
            //UPSNetworkManager networkManager = UPSNetworkManager.getInstance();
            NetworkManager networkManager = NetworkManager.getInstance();
            networkManager.Headers.Clear();

            if (this.defaultCredentialsCheckBox.IsChecked == true) {
                networkManager.UseDefaultCredentials = true;
            } else {
                networkManager.UseDefaultCredentials = false;
            }

            // Request 1

            NameValueCollection data = new NameValueCollection();
            data.Add("option", "credential");
            //data.Add("loginButton2", "Login");
            data.Add("target", "https://login.iam.aeat.allianz.at/");
            data.Add("Ecom_User_ID", this.textBoxUser.Text.Trim());
            data.Add("Ecom_Password", this.passwordBox.Password);

            //byte[] resp = networkManager.UploadValues("https://iapam001.iam.aeat.allianz.at/nidp/app/login?sid=0", data);
            try {
                byte[] resp = networkManager.UploadValues("https://iapam001.iam.aeat.allianz.at/nidp/idff/sso?sid=0&option=credential", data);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }

            string cookieString;
            if (AppSettings.shared.Debug) {
                /*
                string response = Encoding.ASCII.GetString(resp);
                MessageBox.Show("Response" + Environment.NewLine + response);
                */
                cookieString = "";
                foreach (Cookie c in networkManager.ResponseCookies) {
                    cookieString = cookieString + c.Name + "=" + c.Value + Environment.NewLine;
                }
                MessageBox.Show("Login Response Cookies" + Environment.NewLine + cookieString);
                /*
                string tst = "";
                foreach(string header in webclient.ResponseHeaders.AllKeys) {
                    tst = tst + header;
                }
                MessageBox.Show(tst);*/
            }

            string jSessionId = "";
            if (networkManager.ResponseCookies["JSESSIONID"] != null) {
                jSessionId = networkManager.ResponseCookies["JSESSIONID"].Name + "=" + networkManager.ResponseCookies["JSESSIONID"].Value;
            }

            if (AppSettings.shared.Debug) {
                MessageBox.Show(jSessionId);
            }
            /*
            if(!jSessionId.Contains(".LX-PWEB34")) {
                jSessionId = jSessionId + ".LX-PWEB34";
            }*/

            // Request 2

            networkManager.Headers.Add(HttpRequestHeader.Cookie, jSessionId);

            networkManager.QueryString.Add("RelayState", "MA==");
            networkManager.QueryString.Add("SAMLart", "AAN7evpLdH7KP80YRNxwTEZ6+3+AoKXtWtHCpSrjJr2Gjv48nCNH4o/7");

            networkManager.DownloadString("https://iapam001.iam.aeat.allianz.at/nesp/idff/spassertion_consumer");

            //MessageBox.Show("Response" + Environment.NewLine + response);
            string cookies = "";
            foreach (Cookie cookie in networkManager.ResponseCookies) {
                if (cookie.Name == "JSESSIONID" && !cookie.Value.Contains(".LX-PWEB34")) {
                    cookie.Value = cookie.Value + ".LX-PWEB34";
                }
                cookies = cookies + ";" + cookie.Name + "=" + cookie.Value;
                //networkManager.CookieContainer.Add(cookie);
            }

            if (AppSettings.shared.Debug) {
                MessageBox.Show("new cookies iapam001" + cookies.Replace(";", Environment.NewLine));
            }

            // Request 3

            networkManager.Headers.Add(HttpRequestHeader.Cookie, cookies);

            networkManager.Headers.Add(HttpRequestHeader.Cookie,
                ";allianzInternet=true" +
                ";allianzPortalView=https://portanova.aeat.allianz.at/portanova/portal/default"
                //";NETMIND_SID=c55b08f4aa-28d01f60aa-9dd11b20aa-a041bb97aa-1499782306" + 
                //";NETMIND_PERMSID=19011588aa-c4237f3eaa-4beaba52aa-ae5ded20aa-1486462515" + 
                //";LtpaToken=AAECAzU3NkE5MUEzNTc2QkUzMjNDTj1QaGlsaXBwIFNjaHVua2VyL09VPUdEL089QWxsaWFuei9DPUFUD1moXEgZfWVleJdiNV9yZIOgiwU=" + 
                //";AAAA030c695759=AQAAAAAAAAAokSReUkL1SgCL4FUYocpe; path=/; domain=iam.aeat.allianz.at" + 
                //";AAAA030c695759=AQAAAAAAAAAokSReUkL1SgCL4FUYocpe" + 
                //";ZNPCQ003-32333400=474d8397"
            );

            networkManager.Headers.Add(HttpRequestHeader.Host, "am-projectsuite.iam.aeat.allianz.at");
            networkManager.Headers.Add(HttpRequestHeader.Referer, "https://am-projectsuite.iam.aeat.allianz.at/projectsuite/");

            networkManager.QueryString.Clear();

            /*
            string headerString;
            headerString = "";
            foreach(string s in webclient.ResponseHeaders.AllKeys) {
                headerString = headerString + s + System.Environment.NewLine;
            }
            MessageBox.Show("Projectsuite Headers" + headerString); */

            string response = networkManager.DownloadString("https://am-projectsuite.iam.aeat.allianz.at/projectsuite/");

            cookies = "";
            foreach (Cookie cookie in networkManager.ResponseCookies) {
                if (cookie.Name == "JSESSIONID" && !cookie.Value.Contains(".LX-PWEB34")) {
                    cookie.Value = cookie.Value + ".LX-PWEB34";
                }
                cookies = cookies + ";" + cookie.Name + "=" + cookie.Value;
            }

            if (AppSettings.shared.Debug) {
                MessageBox.Show("new cookies am-projectsuite/projectsuite/" + cookies.Replace(";", Environment.NewLine));
            }

            // Request 4

            networkManager.Headers.Add(HttpRequestHeader.Cookie, cookies);

            if (AppSettings.shared.Debug) {
                cookieString = "";
                foreach (string cookie in networkManager.Headers.GetValues("Cookie")) {
                    cookieString = cookieString + cookie;
                }

                MessageBox.Show("Set cookies" + Environment.NewLine + cookieString);
            }

            string ssoResponse = "";
            try {
                ssoResponse = networkManager.UploadString("https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/sso", "");
            } catch(WebException webex) {
                MessageBox.Show("Login failed" + Environment.NewLine + webex.Message);
                textBlockStatus.Text = "Login failed. Wrong password?";
                return;
            }

            textBlockStatus.Text = "";

            this.setApplicationSettings(ssoResponse);

            networkManager.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
            //webclient.Headers.Add("X-Authorization", "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJBNDkzOCIsImlzcyI6IkFMTElBTlogQU1PUyBBQlMgQ09SRSIsImlhdCI6MTUwMDAzNjUwOX0.29l8cG88O2UXjVfwJac7Ax7ZsWkLPZY7gh5jASIJNnw");
            //networkManager.Headers.Add("X-Authorization", token);

            if (AppSettings.shared.Debug) {
                string auth = "";
                foreach (string s in networkManager.Headers.GetValues("X-Authorization")) {
                    auth = auth + s;
                }
                MessageBox.Show("WebClient Authorization" + Environment.NewLine + auth);
            }
        }

        private void login() {
            NetworkManager networkManager = NetworkManager.getInstance();
            networkManager.Headers.Clear();
            networkManager.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");

            Dictionary<string, string> loginDict = new Dictionary<string,string>();
            loginDict.Add("loginName", this.textBoxUser.Text.Trim());
            loginDict.Add("password", this.passwordBox.Password);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(loginDict);

            if (AppSettings.shared.Debug) {
                MessageBox.Show(json);
            }

            string response = "";
            try {
                response = networkManager.UploadString("https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/login", json);
            } catch(WebException webex) {
                MessageBox.Show("Login failed" + Environment.NewLine + webex.Message);
                textBlockStatus.Text = "Login failed. Wrong password?";
                return;
            }

            this.setApplicationSettings(response);

            string cookies = "";
            foreach (Cookie cookie in networkManager.ResponseCookies) {
                if (cookie.Name == "JSESSIONID" && !cookie.Value.Contains(".LX-PWEB34")) {
                    cookie.Value = cookie.Value + ".LX-PWEB34";
                }
                cookies = cookies + ";" + cookie.Name + "=" + cookie.Value;
            }

            if (AppSettings.shared.Debug) {
                MessageBox.Show("new cookies am-projectsuite/projectsuite/" + cookies.Replace(";", Environment.NewLine));
            }

            networkManager.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
        }

        private void setApplicationSettings(string json) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Hashtable responseObject = serializer.Deserialize<Hashtable>(json);
            //string userID = (string)responseObject["user"]["id"];

            //MessageBox.Show(responseObject.Count.ToString());

            Dictionary<string, object> user = (Dictionary<string, object>)responseObject["user"];
            string userID = user["id"].ToString();

            Dictionary<string, object> tenant = (Dictionary<string, object>)user["tenant"];
            string tenantName = tenant["name"].ToString();
            string tenantVal = tenant["value"].ToString();

            Dictionary<string, object> tokenObject = (Dictionary<string, object>)responseObject["token"];
            string token = tokenObject["token"].ToString().Trim();

            if (AppSettings.shared.Debug) {
                MessageBox.Show("userID: " + userID + System.Environment.NewLine +
                            "tenantName: " + tenantName + System.Environment.NewLine +
                            "tenantVal: " + tenantVal + System.Environment.NewLine +
                            "token: " + token + System.Environment.NewLine);
            }

            this.Settings["userid"] = userID;
            this.Settings["tenantname"] = tenantName;
            this.Settings["tenantvalue"] = tenantVal;

            AppSettings.shared.UserId = userID;
            AppSettings.shared.TenantName = tenantName;
            AppSettings.shared.TenantValue = tenantVal;

            NetworkManager networkManager = NetworkManager.getInstance();
            networkManager.Headers.Add("X-Authorization", token);
        }

        private void UpdateMainWindowLoginStatus(Window window, string value) {
            MainWindow mainWindow = (MainWindow)window;
            mainWindow.textBlockLoginStatus.Text = "Logged in as " + value;
        }
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            //this.Owner = null;
            base.OnClosing(e);
        }
    }
}
