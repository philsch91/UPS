using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UPS {
    public class UPSNetworkManager : CookieAwareWebClient {

        private static UPSNetworkManager instance = null;

        protected UPSNetworkManager() { }

        public static UPSNetworkManager getInstance() {
            if(instance == null) {
                instance = new UPSNetworkManager();
            }

            return instance;
        }
    }
}
