﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Specialized;
using PSNetworking;

namespace UPS {
    public class UPSWorkingStepGetRequest : GetRequest {

        private String memberId = null;
        private String tenant = null;

        public UPSWorkingStepGetRequest(string memberId, string tenant){
            this.url = "https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/workingSteps";
            this.memberId = memberId;
            this.tenant = tenant;
        }

        public Booking[] GetWorkingSteps(string fromUnixTimestamp, string toUnixTimestamp) {
            List<Booking> bookings = new List<Booking>();
            
            this.QueryCollection.Add("memberId", this.memberId);
            this.QueryCollection.Add("tenant", this.tenant);
            this.QueryCollection.Add("from", fromUnixTimestamp);    //"1499983200000"
            this.QueryCollection.Add("to", toUnixTimestamp);        //"1499983200000"
            this.QueryCollection.Add("incl", "true");               //false
            this.QueryCollection.Add("allTenants", "true");
            /*
            foreach(String key in this.Webclient.QueryString.Keys) {
                System.Windows.MessageBox.Show(key + "=" + this.Webclient.QueryString[key]);
            }
            */
            String json = this.Get();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string,object>> taskList = serializer.Deserialize<List<Dictionary<string,object>>>(json);
            foreach(Dictionary<string, object> task in taskList) {
                Booking booking = new Booking();
                booking.TaskID = task["taskId"].ToString();
                booking.Date = task["date"].ToString();
                
                //System.Windows.MessageBox.Show(task["duration"].ToString());
                //booking.Duration = Int32.Parse(task["duration"].ToString());
                Double dDuration = Double.Parse(task["duration"].ToString());
                dDuration = Math.Round(dDuration);
                booking.Duration = (int)dDuration;
                
                Dictionary<string, object> categoryDict = (Dictionary<string, object>)task["category"];
                booking.Category = categoryDict["value"].ToString();
                booking.Tenant = task["tenant"].ToString();
                booking.Activity = task["activity"].ToString();

                bookings.Add(booking);
            }

            Booking[] bookingsArray = bookings.ToArray();
            return bookingsArray;
        }
    }
}
