using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UPS {
    class AppSettings {

        private static AppSettings instance = null;

        public static AppSettings shared {
            get {
                if(instance == null) {
                    instance = new AppSettings();
                }

                return instance;
            }
        }

        public String TenantName { get; set; }
        public String TenantValue { get; set; }
        public String UserId { get; set; }
        public String Token { get; set; }
    }
}
