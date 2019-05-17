using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using UPS.models;
using PSNetworking;

namespace UPS {
    public class AbsenceStepDayPostRequest : AbsenceStepPostRequest {

        public AbsenceStepDayPostRequest(Task task, string userId)
            : base(task, 1, 8, userId) {
            
        }
    }
}
