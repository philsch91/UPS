﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Specialized;
using PSNetworking;

namespace UPS {
    public class Booking {

        private string activity;

        public string Date { get; set; }
        public string Tenant { get; set; }
        public int Duration { get; set; }

        public string Activity {
            get { return this.activity; }
            set {
                //System.Windows.MessageBox.Show(value);
                byte[] bytes = Encoding.Default.GetBytes(value);
                this.activity = Encoding.UTF8.GetString(bytes);
            }
        }

        public string UserID { get; set; }
        public string TaskID { get; set; }
        public string Category { get; set; }
        public CookieWebClient webclient = null;

        protected string InternalSend(string timestamp) {
            //Hashtable postdata = new Hashtable();
            //postdata.Add("taskId", "2017-01-26 17:11:27.030806");       //taskId in listTask    //Jboss,Linux,Apache,Loadbalancing-AEV
            //postdata.Add("taskId", taskId);
            //postdata.Add("taskName", "Citrix-General");
            //postdata.Add("taskName", "Jboss, Linux, Apache, Loadbalancing - AEV");
            //postdata.Add("date", "1499810400000");    //added 000
            //postdata.Add("tenant", settings["tenantvalue"]);

            Hashtable postdata = new Hashtable();
            postdata.Add("userId", this.UserID);
            postdata.Add("taskId", this.TaskID);
            postdata.Add("tenant", this.Tenant);
            postdata.Add("date", timestamp);
            postdata.Add("duration", this.Duration);
            postdata.Add("category", this.Category);
            postdata.Add("activity", this.Activity);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(postdata);

            // TODO Serialize Booking Object

            //System.Windows.MessageBox.Show(json);
            //MessageBox.Show(webclient.ToString());

            /*
            string cookies = "";
            foreach(string cookie in webclient.Headers.GetValues("Cookie")) {
                cookies = cookies + cookie;
            }
            MessageBox.Show("WebClient Cookies" + Environment.NewLine + cookies);
            */

            if(webclient.Headers[HttpRequestHeader.ContentType] == null) {
                webclient.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
            }

            if(webclient.Headers[HttpRequestHeader.ContentType] != "application/json;charset=UTF-8") {
                webclient.Headers[HttpRequestHeader.ContentType] = "application/json;charset=UTF-8";
            }

            string response = "";
            response = webclient.UploadString("https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/workingStep", json);
            //MessageBox.Show(s);

            return response;
        }

        protected string InternalSend(string timestamp, int hours) {
            Hashtable postdata = new Hashtable();
            postdata.Add("userId", this.UserID);
            postdata.Add("taskId", this.TaskID);
            postdata.Add("tenant", this.Tenant);
            postdata.Add("date", timestamp);
            postdata.Add("duration", hours);
            postdata.Add("category", this.Category);
            postdata.Add("activity", this.Activity);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(postdata);

            // TODO Serialize Booking Object

            //System.Windows.MessageBox.Show(json);
            //MessageBox.Show(webclient.ToString());

            /*
            string cookies = "";
            foreach(string cookie in webclient.Headers.GetValues("Cookie")) {
                cookies = cookies + cookie;
            }
            System.Windows.MessageBox.Show("WebClient Cookies" + Environment.NewLine + cookies);*/
            
            /*
            foreach(string header in webclient.Headers.AllKeys) {
                foreach(string val in webclient.Headers.GetValues(header)){
                    System.Windows.MessageBox.Show(val);
                }
            }*/

            if(webclient.Headers[HttpRequestHeader.ContentType] == null) {
                webclient.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
            }

            if(webclient.Headers[HttpRequestHeader.ContentType] != "application/json;charset=UTF-8") {
                webclient.Headers[HttpRequestHeader.ContentType] = "application/json;charset=UTF-8";
            }

            string response = "";
            response = webclient.UploadString("https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/workingStep", json);

            return response;
        }

        protected string InternalSend() {
            Hashtable postdata = new Hashtable();
            postdata.Add("userId", this.UserID);
            postdata.Add("taskId", this.TaskID);
            postdata.Add("tenant", this.Tenant);
            postdata.Add("date", this.Date);
            postdata.Add("duration", this.Duration);
            postdata.Add("category", this.Category);
            postdata.Add("activity", this.Activity);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(postdata);

            // TODO Serialize Booking Object

            if(webclient.Headers[HttpRequestHeader.ContentType] == null) {
                webclient.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
            }

            if(webclient.Headers[HttpRequestHeader.ContentType] != "application/json;charset=UTF-8") {
                webclient.Headers[HttpRequestHeader.ContentType] = "application/json;charset=UTF-8";
            }

            string response = "";
            response = webclient.UploadString("https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/workingStep", json);

            return response;
        }

    }
}
