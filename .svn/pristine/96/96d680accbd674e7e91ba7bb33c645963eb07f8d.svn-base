using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSNetworking;
using System.Web.Script.Serialization;
using UPS.models;

namespace UPS.networking {
    public class WorkingStepGetRequest : NetworkGETRequest{

        protected string memberId = null;
        protected string tenant = null;
        protected string fromUnixTimestamp = null;
        protected string toUnixTimestamp = null;

        public WorkingStepGetRequest(string memberId, string tenant, string fromUnixTimestamp, string toUnixTimestamp) {
            this.url = "https://am-projectsuite.tiam.aeat.allianz.at/projectsuite/rest/workingSteps";
            this.memberId = memberId;
            this.tenant = tenant;
            this.fromUnixTimestamp = fromUnixTimestamp;
            this.toUnixTimestamp = toUnixTimestamp;

            this.QueryCollection.Add("memberId", this.memberId);
            this.QueryCollection.Add("tenant", this.tenant);
            this.QueryCollection.Add("from", this.fromUnixTimestamp);    //"1499983200000"
            this.QueryCollection.Add("to", this.toUnixTimestamp);        //"1499983200000"
            this.QueryCollection.Add("incl", "true");
            this.QueryCollection.Add("allTenants", "true");
            /*
            foreach(String key in this.QueryCollection.Keys) {
                System.Windows.MessageBox.Show(key + "=" + this.QueryCollection[key]);
            } 
            */
        }

        public override object Process(string json) {
            //System.Windows.MessageBox.Show(json);
            foreach(String key in this.NetworkManager.QueryString.Keys) {
                System.Windows.MessageBox.Show(key + "=" + this.NetworkManager.QueryString[key]);
            }

            List<Booking> bookings = new List<Booking>();
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> taskList = serializer.Deserialize<List<Dictionary<string, object>>>(json);
            
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
