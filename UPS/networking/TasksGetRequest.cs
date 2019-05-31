using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using UPS.models;
using PSNetworking;

namespace UPS.networking {
    public class TasksGetRequest: NetworkGETRequest {

        public TasksGetRequest(string timestamp, string taskGroup) {
            //this.url = "https://projectsuite.aeat.allianz.at/projectsuite/rest/tasksByDateAndGroupAndUser";
            this.url = "https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/tasksByDateAndGroupAndUser";

            this.QueryCollection.Add("userId", AppSettings.shared.UserId);
            this.QueryCollection.Add("tenant", AppSettings.shared.TenantValue);
            this.QueryCollection.Add("date", timestamp);     //new 1499983200000
            this.QueryCollection.Add("incl", "true");
            this.QueryCollection.Add("taskGroup", taskGroup);
            this.QueryCollection.Add("viewType", "time");
        }

        public override object Process(string json) {
            //System.Windows.MessageBox.Show(json);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<object> taskList = serializer.Deserialize<List<object>>(json);
            return taskList;
        }
    }
}
