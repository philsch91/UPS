﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Specialized;

namespace UPS {
    public class DayBooking : Booking {

        private int hours;
        private int percentage;

        public int Hours {
            get { return this.hours; }
            set {
                if(value < 1 || value > 24) {
                    throw new FormatException("Value must be between 0 and 25");
                }
                this.hours = value;
            }
        }
        public int Percentage {
            get { return this.percentage; }
            set {
                if(value < 1 || value > 100) {
                    throw new FormatException("Value must be between 0 and 101");
                }
                this.percentage = value;
            }
        }

        public DayBooking() {
            this.Date = DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString() + "000";
        }

        public DayBooking(PSDateTime dateTime) {
            this.Date = dateTime.ToString();
        }

        public string Send() {
            //string unixTimestamp = DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            //string timestamp = unixTimestamp + "000";

            Int16 workHours = (Int16)Math.Round(((double)this.hours / 100) * this.percentage);
            System.Windows.MessageBox.Show("Work hours: " + workHours.ToString());

            this.Duration = workHours;
            
            string response = "";
            response = this.InternalSend();

            return "";
        }
    }
}
