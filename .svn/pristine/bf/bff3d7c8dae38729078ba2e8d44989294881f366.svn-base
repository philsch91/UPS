using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Web.Script.Serialization;

namespace UPS {
    public class GetRequest : Request {

        private NameValueCollection queryCollection = null;

        public NameValueCollection QueryCollection {
            get {
                if(this.queryCollection == null) {
                    this.queryCollection = new NameValueCollection();
                }
                return this.queryCollection;
            }
            set {
                this.queryCollection = value;
            }
        }

        public virtual String Get() {
            this.Webclient.QueryString = this.QueryCollection;
            //this.Webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.DownloadStringCompleted);
            //this.Webclient.DownloadStringAsync(new Uri(this.url));
            String response = this.Webclient.DownloadString(this.url);
            this.Webclient.QueryString.Clear();
            return response;
        }


    }
}
