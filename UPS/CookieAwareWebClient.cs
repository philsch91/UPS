using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace UPS {
    public class CookieAwareWebClient : WebClient{

        public CookieContainer CookieContainer {
            get;
            private set;
        }

        public CookieCollection ResponseCookies {
            get;
            set;
        }

        public CookieAwareWebClient() {
            this.CookieContainer = new CookieContainer();
            this.ResponseCookies = new CookieCollection();
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest) base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request) {
            //HttpWebResponse response = (HttpWebResponse)base.GetWebResponse(request);
            HttpWebResponse response;
            
            try {
                response = (HttpWebResponse)base.GetWebResponse(request);
            } catch(WebException webex) {
                System.Windows.MessageBox.Show(webex.Message);
                return null;
            }

            this.ResponseCookies = response.Cookies;
            return response;
        }
    }
}
