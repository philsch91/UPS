using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSNetworking;

namespace UPS {
    public class Request {

        private CookieWebClient webclient = null;
        protected String url = null;

        public CookieWebClient Webclient {
            get {
                if(this.webclient == null) {
                    this.webclient = new CookieWebClient();
                }
                return this.webclient;
            }
            set {
                this.webclient = value;
            }
        }

        /*
        public Request(String memberId, String tenant) {
            this.memberId = memberId;
            this.tenant = tenant;
        }

        public Request(CookieAwareWebClient webclient, String memberId, String tenant) {
            this.webclient = webclient;
            this.memberId = memberId;
            this.tenant = tenant;
        }
        */
    }
}
