﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using UPS.models;
using PSNetworking;

namespace UPS.networking {
    public class CategoryByTaskAndDateGetRequest : NetworkGETRequest {

        protected Task task;
        protected string date;
        protected string tenant;

        public CategoryByTaskAndDateGetRequest(Task task, string date, string tenant) {
            this.url = "https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/categoryDomainsByTaskIdAndDate";
            this.task = task;
            this.date = date;
            this.tenant = tenant;

            this.QueryCollection.Add("id", this.task.ID);
            this.QueryCollection.Add("date", date);     //new
            this.QueryCollection.Add("tenant", this.tenant);
        }

        public override object Process(string json) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<object> categoryList = serializer.Deserialize<List<object>>(json);

            List<TaskCategory> categories = new List<TaskCategory>();
            foreach(Dictionary<string, object> catDict in categoryList) {
                TaskCategory category = new TaskCategory();
                category.Name = catDict["name"].ToString();
                category.Value = catDict["value"].ToString();
                categories.Add(category);
            }

            return categories;
        }
    }
}
