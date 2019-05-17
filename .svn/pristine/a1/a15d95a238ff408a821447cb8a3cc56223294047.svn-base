using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UPS {
    public class PSDateTime {

        private DateTime dateTime;

        public PSDateTime(DateTime dateTime) {
            this.dateTime = dateTime;
        }

        public override string ToString() {
            //return base.ToString();
            return this.dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString() + "000";
        }

        public String ToUnixTimestamp() {
            return this.dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
        }
    }
}
