﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using UPS.models;
using PSNetworking;

namespace UPS {
    public class AbsenceStepPostRequest : NetworkPOSTRequest {
        /*
        protected string category;
        protected int duration;
        protected int durationType;
        protected string from;
        protected string fromString;
        protected string taskId;
        protected string tenant;
        protected int to;
        protected string toString;
        protected string userId;
        */
        protected Task task;

        public AbsenceStepPostRequest(Task task, int duration, int durationType, string userId) {
            this.url = "https://am-projectsuite.tiam.aeat.allianz.at/projectsuite/rest/absenceStep";
            this.task = task;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["category"] = this.task.Category;
            dict["duration"] = duration;
            dict["durationType"] = durationType;
            dict["from"] = this.task.Start;
            dict["to"] = this.task.End;
            dict["taskId"] = this.task.ID;
            dict["tenant"] = this.task.Tenant;
            dict["userId"] = userId;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            this.data = serializer.Serialize(dict);
        }

        public override object Process(string json) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<object> taskList = serializer.Deserialize<List<object>>(json);

            List<Task> tasks = new List<Task>();
            foreach(Dictionary<string, object> taskDict in taskList) {
                Task task = new Task();
                task.ID = taskDict["id"].ToString();
                task.Name = taskDict["name"].ToString();
                task.Number = Int64.Parse(taskDict["number"].ToString());
                task.Start = Int64.Parse(taskDict["start"].ToString());
                task.End = Int64.Parse(taskDict["end"].ToString());
                tasks.Add(task);
            }

            return tasks;
        }
    }
}
