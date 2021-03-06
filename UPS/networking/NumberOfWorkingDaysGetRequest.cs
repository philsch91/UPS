﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using PSNetworking;
using UPS.models;


namespace UPS.networking {
    public class NumberOfWorkingDaysGetRequest : NetworkGETRequest {

        protected string fromDateStr;
        protected string toDateStr;
        protected string userId;

        public NumberOfWorkingDaysGetRequest(string fromDateStr, string toDateStr, string userId) {
            this.url = "https://am-projectsuite.tiam.aeat.allianz.at/projectsuite/rest/numberOfWorkingDaysByDateRange";
            this.fromDateStr = fromDateStr;
            this.toDateStr = toDateStr;
            this.userId = userId;
            this.QueryCollection.Add("from", this.fromDateStr);
            this.QueryCollection.Add("to", this.toDateStr);
            this.QueryCollection.Add("userId", this.userId);
        }

        public override object Process(string json) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Object days = serializer.Deserialize<Object>(json);

            Int32 idays = (Int32)days;

            return idays;
        }
    }
}
