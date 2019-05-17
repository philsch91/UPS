using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UPS.models {
    public class Task {

        protected String category;
        protected TaskCategory taskCategory;

        public String ID { get; set; }
        public String Name { get; set; }
        public String Tenant { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public long Number { get; set; }
        public String Type { get; set; }
        
        public TaskCategory TaskCategory {
            get { return this.taskCategory; }
            set { 
                this.taskCategory = value;
                this.category = value.Value;
            }
        }

        public String Category {
            get { return this.category; }
        }
    }
}
