using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using UPS.models;
using PSNetworking;

namespace UPS {
    public class TasksForAbsenceGetRequest : NetworkGETRequest{

        private String tenant = null;

        public TasksForAbsenceGetRequest(string tenant) {
            this.url = "https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/tasksForAbsence";
            this.tenant = tenant;
            this.QueryCollection.Add("tenant", tenant);
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
