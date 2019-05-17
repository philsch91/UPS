using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Specialized;
using PSNetworking;

namespace UPS {
    public class MonthBooking : Booking {

        private int hours;
        private int percentage;
        private DateTime startDay;
        private DateTime[] dateTimes;

        public int Hours {
            get { return this.hours; }
            set {
                if(value < 1 || value > 400) {
                    throw new FormatException("Value must be between 0 and 401");
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

        public DateTime[] DateTimes {
            get { return this.dateTimes; }
        }

        public bool LastMonth { get; set; }
        public bool ExcludeWeekends { get; set; }

        public MonthBooking() {
            this.LastMonth = false;
            this.ExcludeWeekends = true;
            this.startDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }

        public MonthBooking(DateTime dateTime) {
            this.LastMonth = false;
            this.ExcludeWeekends = true;
            //this.month = dateTime.Month;
            //this.year = dateTime.Year;
            this.startDay = dateTime;
        }

        public void SetDateTimes() {
            int daysInMonth = DateTime.Now.Day;
            
            if(DateTime.Today.Year != this.startDay.Year && DateTime.Today.Month != this.startDay.Month) {
                daysInMonth = DateTime.DaysInMonth(this.startDay.Year, this.startDay.Month);
            }

            if(this.LastMonth) {
                DateTime today = DateTime.Today;
                int year = today.Year;
                int month = today.Month;
                //=== for last month ===
                month = today.Month - 1;
                if(month <= 0) {
                    year = year - 1;
                    month = 12;
                }
                this.startDay = new DateTime(year, month, 1);
                daysInMonth = DateTime.DaysInMonth(year, month);
            }

            List<DateTime> dateTimeList = new List<DateTime>();
            int day = 1;
            while(day <= daysInMonth) {
                DateTime dayInMonth = new DateTime(this.startDay.Year, this.startDay.Month, day);
                if(this.ExcludeWeekends) {
                    if(dayInMonth.DayOfWeek != DayOfWeek.Saturday && dayInMonth.DayOfWeek != DayOfWeek.Sunday) {
                        dateTimeList.Add(dayInMonth);
                    }
                } else {
                    dateTimeList.Add(dayInMonth);
                }
                day++;
            }

            DateTime[] dateTimes = dateTimeList.ToArray();
            this.dateTimes = dateTimes;
            return;
        }

        public String[] GetTimestamps() {
            String[] timeStamps = new String[this.dateTimes.Length];

            for(int i=0;i<dateTimes.Length;i++){
                string dayUnixTimestamp = dateTimes[i].Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
                string ts = dayUnixTimestamp + "000";
                timeStamps[i] = ts;
            }

            return timeStamps;
        }

        public string Send() {
            /*
            System.Windows.MessageBox.Show(
                "hours:" + this.hours.ToString() + Environment.NewLine +
                "percentage:" + this.percentage.ToString());
            */
            string debugmsg = "";

            //string unixTimestamp = DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            //string timestamp = unixTimestamp + "000";

            //double splittedHours = Math.Round(((double)this.hours / 100) * this.percentage);
            Int16 workHours = (Int16)Math.Round(((double)this.hours / 100) * this.percentage);
            //System.Windows.MessageBox.Show("Work hours: " + workHours.ToString());

            this.SetDateTimes();
            
            //System.Windows.MessageBox.Show("Days in Month: " + daysInMonth);
            debugmsg = debugmsg + "Days in Month: " + this.dateTimes.Length.ToString() + Environment.NewLine;

            //check bookings for these days

            List<string> listTimestamps = new List<string>();
            
            foreach(String ts in this.GetTimestamps()) {
                
                //UPSNetworkManager manager = UPSNetworkManager.getInstance();
                NetworkManager manager = NetworkManager.getInstance();
                UPSWorkingStepGetRequest services = new UPSWorkingStepGetRequest(this.UserID, this.Tenant);
                services.Webclient = manager;
                Booking[] bookingsToday = services.GetWorkingSteps(ts, ts);
                int hours = 0;
                foreach(Booking booking in bookingsToday) {
                    hours = hours + booking.Duration;
                }
                //System.Windows.MessageBox.Show("Day: " + ts + " Hours: " + hours);
                if(hours < 10) {
                    listTimestamps.Add(ts);
                }
            }

            int avgHoursPerDay = workHours / listTimestamps.Count;
            //double hoursPerDayPrec = (double)(workHours / listTimestamps.Count);
            //System.Windows.MessageBox.Show("Average hours per day: " + avgHoursPerDay.ToString());
            debugmsg = debugmsg + "Average hours per day: " + avgHoursPerDay.ToString() + Environment.NewLine;

            int min=avgHoursPerDay-1;
            int max=avgHoursPerDay+1;   //+2

            if(min < 0) {
                //min is -1
                min=min+2;
            }
            if(min == 0) {
                //min is 0
                min++;
            }
            //if(min == max-1)
            if(min == max) {
                max++;
            }

            //System.Windows.MessageBox.Show("Min: " + min.ToString() + Environment.NewLine
            //    + "Max: " + max.ToString());
            debugmsg = debugmsg + ("Min: " + min.ToString() + Environment.NewLine + "Max: " + max.ToString() + Environment.NewLine
                + "Booking Days: " + listTimestamps.Count);
            System.Windows.MessageBox.Show(debugmsg);

            //spread bookings over days with random factor evenly

            //List<int> listWorkHours = new List<int>();
            Dictionary<string, int> bookings = new Dictionary<string, int>();
            int j = 0;
            int sumHours = 0;
            Random random = new Random();
            while(j < listTimestamps.Count && sumHours < workHours){
                string ts = listTimestamps[j];
                //System.Windows.MessageBox.Show(ts);
                int randomHours = random.Next(min, max+1);
                /*
                if(sumHours + randomHours > workHours) {
                    if(j < listTimestamps.Count) {
                        randomHours = min;
                    } else {
                        randomHours = workHours - sumHours;
                    }
                }*/
                if(j < listTimestamps.Count - 1) {
                    if(sumHours + randomHours > workHours) {
                        randomHours = min;
                    }
                    if((j + 1) * 100 / (listTimestamps.Count) > 50 && (sumHours * 100 / workHours) < 60) {
                        //bookings count over 60% and hours booked less than 60%
                        System.Windows.MessageBox.Show("increasing hours" + System.Environment.NewLine +
                            "Bookings: " + ((j + 1) * 100 / (listTimestamps.Count)).ToString() + "%" + System.Environment.NewLine +
                            "Hours: " + ((sumHours * 100 / workHours)).ToString() + "%");
                        randomHours++;
                    }

                    //check bookings for this days
                    //UPSNetworkManager manager = UPSNetworkManager.getInstance();
                    NetworkManager manager = NetworkManager.getInstance();
                    UPSWorkingStepGetRequest services = new UPSWorkingStepGetRequest(this.UserID, this.Tenant);
                    services.Webclient = manager;
                    Booking[] bookingsToday = services.GetWorkingSteps(ts, ts);
                    int hours = 0;
                    foreach(Booking booking in bookingsToday) {
                        hours = hours + booking.Duration;
                    }
                    if(hours + randomHours > 10) {
                        int randomHoursTmp = randomHours;
                        randomHours = 10 - hours;
                        System.Windows.MessageBox.Show("reducing added hours on " + ts + " by " + (randomHoursTmp - randomHours));
                    }
                } else {
                    //last booking
                    //System.Windows.MessageBox.Show("last booking");
                    if(sumHours + randomHours > workHours) {
                        randomHours = workHours - sumHours;
                    } else if(sumHours + randomHours < workHours) {
                        randomHours = workHours - sumHours;
                    }
                }
                //System.Windows.MessageBox.Show(randomHours.ToString());
                sumHours = sumHours + randomHours;
                //listWorkHours.Add(randomHours);
                bookings.Add(ts, randomHours);
                j++;
            }

            string response = "";
            /*
            int i = 0;
            //foreach(string date in listTimestamps) {
                //int hours = listWorkHours[i];
            foreach(int hours in listWorkHours){
                if(listTimestamps[i] != null) {
                    string date = listTimestamps[i];
                    response = this.InternalSend(date, hours);
                }
                i++;
            }*/

            foreach(KeyValuePair<string, int> booking in bookings) {
                response = this.InternalSend(booking.Key, booking.Value);
            }

            return response;
        }
    }
}
