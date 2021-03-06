﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Specialized;
using PSNetworking;

namespace UPS {
    public class WeekBooking : Booking {

        private int hours;
        private int percentage;
        private int subtractWeeks;
        private DateTime startDay;
        private DateTime[] dateTimes;

        public int Hours {
            get { return this.hours; }
            set {
                if(value < 1 || value > 130) {
                    throw new FormatException("Value must be between 0 and 131");
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

        public int SubtractWeeks {
            get { return this.subtractWeeks;}
            set { //this.subtractWeeks = this.subtractWeeks * (-1);
                if(value < 1 || value > 3) {
                    throw new FormatException("Value must be between 0 and 3");
                }
                this.subtractWeeks = value;
            }
        }

        public DateTime[] DateTimes {
            get { return this.dateTimes; }
        }

        public WeekBooking() {
            this.subtractWeeks = 0;
            this.startDay = DateTime.Today;
        }

        public WeekBooking(DateTime dateTime) {
            this.subtractWeeks = 0;
            this.startDay = dateTime;
        }

        public void SetDateTimes() {
            DayOfWeek weekDay = DayOfWeek.Sunday;
            
            //System.Windows.MessageBox.Show(weekDay.ToString());
            //System.Windows.MessageBox.Show("SubtractWeeks: " + this.SubtractWeeks.ToString());

            if(startDay.DayOfWeek == DayOfWeek.Monday && this.subtractWeeks > 0) {
                this.subtractWeeks--;
            }

            //replace startDay
            int k = 0;
            while(k <= this.subtractWeeks) {
                while(weekDay != DayOfWeek.Monday) {
                    startDay = startDay.AddDays(-1);
                    weekDay = startDay.DayOfWeek;
                    //System.Windows.MessageBox.Show("Type of day: " + weekDay.ToString() + System.Environment.NewLine + "Day: " + startDay.ToString());
                }
                weekDay = DayOfWeek.Sunday;
                k++;
            }

            List<DateTime> dateTimeList = new List<DateTime>();

            int day = 0;
            while(day <= 5) {
                //DateTime dayInWeek = new DateTime(year, month, day);
                DateTime dayInWeek = startDay.AddDays(day);
                if(dayInWeek.DayOfWeek != DayOfWeek.Saturday && dayInWeek.DayOfWeek != DayOfWeek.Sunday) {
                    //System.Windows.MessageBox.Show(dayInWeek.ToString() + Environment.NewLine + ts);
                    dateTimeList.Add(dayInWeek);
                }
                day++;
            }

            DateTime[] dateTimes = dateTimeList.ToArray();
            this.dateTimes = dateTimes;
            return;
        }

        public String[] GetTimestamps() {
            String[] timeStamps = new String[this.dateTimes.Length];

            for(int i = 0;i < dateTimes.Length;i++) {
                string dayUnixTimestamp = dateTimes[i].Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
                string ts = dayUnixTimestamp + "000";
                timeStamps[i] = ts;
            }

            return timeStamps;
        }

        public string Send() {
            Int16 workHours = (Int16)Math.Round(((double)this.hours / 100) * this.percentage);
            //System.Windows.MessageBox.Show("Work hours: " + workHours.ToString());

            string debugmsg = "";

            //System.Windows.MessageBox.Show(weekDay.ToString() + subDays.ToString());
            //System.Windows.MessageBox.Show(startDay.ToString() + " " + startDay.DayOfWeek.ToString());

            //int year = DateTime.Today.Year;
            //int month = DateTime.Today.Month;

            this.SetDateTimes();

            //check bookings for these days

            List<string> listTimestamps = new List<string>();
            
            foreach(String ts in this.GetTimestamps()) {
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

            int min = avgHoursPerDay - 1;
            int max = avgHoursPerDay + 1;   //+2

            if(min < 0) {
                //min is -1
                min = min + 2;
            }
            if(min == 0) {
                //min is 0
                min++;
            }
            //if(min == max-1)
            if(min == max) {
                max++;
            }

            debugmsg = debugmsg + ("Min: " + min.ToString() + Environment.NewLine + "Max: " + max.ToString() + Environment.NewLine
                + "Booking Days: " + listTimestamps.Count);
            System.Windows.MessageBox.Show(debugmsg);

            Dictionary<string, int> bookings = new Dictionary<string, int>();
            int j = 0;
            int sumHours = 0;
            Random random = new Random();
            while(j < listTimestamps.Count && sumHours < workHours) {
                string ts = listTimestamps[j];
                //System.Windows.MessageBox.Show(ts);
                int randomHours = random.Next(min, max + 1);
                if(j < listTimestamps.Count-1) {
                    if(sumHours + randomHours > workHours) {
                        randomHours = min;
                    }
                    if((j+1)*100/(listTimestamps.Count) > 50 && (sumHours*100/workHours) < 60) {
                        //bookings count over 60% and hours booked less than 60%
                        System.Windows.MessageBox.Show("increasing hours" + System.Environment.NewLine +
                            "Bookings: " + ((j + 1) * 100 / (listTimestamps.Count)).ToString() + "%" + System.Environment.NewLine +
                            "Hours: " + ((sumHours * 100 / workHours)).ToString() + "%");
                        randomHours++;
                    }

                    //check bookings for this day
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
                }else{
                    //last booking
                    //System.Windows.MessageBox.Show("last booking");
                    if(sumHours + randomHours > workHours) {
                        randomHours = workHours - sumHours;
                    }else if(sumHours + randomHours < workHours) {
                        randomHours = workHours - sumHours;
                    }
                }
                //System.Windows.MessageBox.Show(randomHours.ToString());
                sumHours = sumHours + randomHours;
                bookings.Add(ts, randomHours);
                j++;
            }

            string response = "";
            foreach(KeyValuePair<string, int> booking in bookings) {
                response = this.InternalSend(booking.Key, booking.Value);
            }

            return response;
        }
    }
}
