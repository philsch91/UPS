using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Specialized;
using PSNetworking;
using UPS.models;
using UPS.networking;


namespace UPS {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window {

        private Dictionary<string, string> settings;    //change to settings object
        private List<object> listTask;
        private List<object> categoryList;
        private List<Task> taskList;                    //new
        protected Boolean calendarDateChangeFlag;

        public MainWindow() {
            InitializeComponent();
            this.buttonDayBooking.IsEnabled = false;
            this.buttonGlobalTasks.IsEnabled = false;
            this.calendarDateChangeFlag = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Owner = this;
            loginWindow.DataContext = this;
            
            //this.webclient = new CookieAwareWebClient();
            //loginWindow.WebClient = this.webclient;

            this.settings = new Dictionary<string, string>();
            loginWindow.Settings = this.settings;

            Nullable<bool> res = loginWindow.ShowDialog();
            if(!loginWindow.DialogResult.HasValue || !loginWindow.DialogResult.Value) {
                //MessageBox.Show("return login");
                return;
            }
            
            this.buttonTasks_Click(this, null);
            //MessageBox.Show("Window_Loaded");
            
            //while(NetworkManager.getInstance().IsBusy);

            MonthBooking monthBooking = new MonthBooking();
            monthBooking.ExcludeWeekends = false;
            monthBooking.SetDateTimes();
            String[] timestamps = monthBooking.GetTimestamps();
            
            int hours = 0;
            foreach(String ts in timestamps) {
                
                NetworkManager manager = NetworkManager.getInstance();
                
                UPSWorkingStepGetRequest services = new UPSWorkingStepGetRequest(this.settings["userid"], this.settings["tenantvalue"]);
                services.Webclient = manager;
                Booking[] dayBookings = services.GetWorkingSteps(ts, ts);

                foreach(Booking booking in dayBookings) {
                    hours = hours + booking.Duration;
                }
                
                /*
                //not working 20.03.2019
                WorkingStepGetRequest request = new WorkingStepGetRequest(this.settings["userid"], this.settings["tenantvalue"], ts, ts);
                
                request.callback = delegate(Object data) {
                    Booking[] bookings = (Booking[])data;
                    foreach(Booking booking in bookings) {
                        hours = hours + booking.Duration;
                    }
                };
                
                request.Get();
                */
            }

            this.textBlockMonthHours.Text = "Booked this month: " + hours.ToString();
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e) {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Owner = this;

            //this.webclient = new CookieAwareWebClient();
            //loginWindow.WebClient = this.webclient;
            
            //TODO: use delegate pattern instead of a reference
            this.settings = new Dictionary<string, string>();
            loginWindow.Settings = this.settings;
            
            loginWindow.ShowDialog();
        }

        //change to ComboBox Event
        private void buttonTasks_Click(object sender, RoutedEventArgs e) {            
            //MessageBox.Show(DateTime.Today.);
            //Int32 unixTimestamp = Int32.Parse(DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString());
            string unixTimestamp = DateTime.Today.Subtract(new DateTime(1970,1,1)).TotalSeconds.ToString();
            string timestamp = unixTimestamp + "000";
            //MessageBox.Show(timestamp);
            
            NameValueCollection opts = new NameValueCollection();
            opts.Add("date", timestamp);      //"1499983200000"
            opts.Add("incl", "true");
            opts.Add("taskGroup","globalTasks");
            opts.Add("tenant",settings["tenantvalue"]);
            opts.Add("userId",settings["userid"]);
            opts.Add("viewType","time");

            NetworkManager networkManager = NetworkManager.getInstance();

            networkManager.QueryString = opts;
            networkManager.Headers.Remove(HttpRequestHeader.ContentType);
            
            //byte[] response = webclient.UploadValues("https://projectsuite.aeat.allianz.at/projectsuite/rest/tasksByDateAndGroupAndUser","GET",opts);
            string json = networkManager.DownloadString("https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/tasksByDateAndGroupAndUser");

            networkManager.QueryString.Clear();
            networkManager.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
            
            //string json = Encoding.ASCII.GetString(response);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            this.listTask = serializer.Deserialize<List<object>>(json);
            
            this.comboBoxTasks.Items.Clear();
            foreach(Dictionary<string, object> task in this.listTask) {
                //string taskName = (string)task["name"];
                string taskName = task["name"].ToString();
                this.comboBoxTasks.Items.Add(taskName);
            }

            TasksForAbsenceGetRequest request = new TasksForAbsenceGetRequest(settings["tenantvalue"]);
            request.NetworkManager = NetworkManager.getInstance();

            //while(NetworkManager.getInstance().IsBusy) ;
            
            request.callback = delegate(object data){
                this.taskList = (List<Task>)data;
                foreach(Task task in this.taskList) {
                    this.comboBoxTasks.Items.Add(task.Name);
                }
            };

            request.Get();
        }

        private void tasks_Click(object sender, RoutedEventArgs e) {
            //MessageBox.Show(e.Source.ToString() + Environment.NewLine + e.OriginalSource.ToString());
            //Button test = (Button) e.OriginalSource;
            //MessageBox.Show(test.Name);

            string taskGroup = "globalTasks";

            //if(buttonPeriod == this.buttonDayBooking) {
            if(sender == this.buttonMyTasks) {
                //this.buttonMyTasks.BorderBrush = new SolidColorBrush(Colors.LightBlue);
                this.buttonMyTasks.IsEnabled = false;
                this.buttonLastBooked.IsEnabled = true;
                this.buttonGlobalTasks.IsEnabled = true;
                this.buttonTeamTasks.IsEnabled = true;
                taskGroup = "myTasks";
            }
            if(sender == this.buttonLastBooked) {
                this.buttonMyTasks.IsEnabled = true;
                this.buttonLastBooked.IsEnabled = false;
                this.buttonGlobalTasks.IsEnabled = true;
                this.buttonTeamTasks.IsEnabled = true;
                taskGroup = "lastBookedTasks";
            }
            if(sender == this.buttonGlobalTasks) {
                this.buttonMyTasks.IsEnabled = true;
                this.buttonLastBooked.IsEnabled = true;
                this.buttonGlobalTasks.IsEnabled = false;
                this.buttonTeamTasks.IsEnabled = true;
                taskGroup = "globalTasks";
            }
            if(sender == this.buttonTeamTasks) {
                this.buttonMyTasks.IsEnabled = true;
                this.buttonLastBooked.IsEnabled = true;
                this.buttonGlobalTasks.IsEnabled = true;
                this.buttonTeamTasks.IsEnabled = false;
                taskGroup = "teamTasks";
            }

            //MessageBox.Show(DateTime.Today.);
            //Int32 unixTimestamp = Int32.Parse(DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString());
            string unixTimestamp = DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            string timestamp = unixTimestamp + "000";
            //MessageBox.Show(timestamp);

            NameValueCollection opts = new NameValueCollection();
            opts.Add("date", timestamp);      //"1499983200000"
            opts.Add("incl", "true");
            opts.Add("taskGroup", taskGroup);
            opts.Add("tenant", settings["tenantvalue"]);
            opts.Add("userId", settings["userid"]);
            opts.Add("viewType", "time");

            //UPSNetworkManager networkManager = UPSNetworkManager.getInstance();
            NetworkManager networkManager = NetworkManager.getInstance();

            networkManager.QueryString = opts;
            networkManager.Headers.Remove(HttpRequestHeader.ContentType);

            //byte[] response = webclient.UploadValues("https://projectsuite.aeat.allianz.at/projectsuite/rest/tasksByDateAndGroupAndUser","GET",opts);
            string json = networkManager.DownloadString("https://am-projectsuite.iam.aeat.allianz.at/projectsuite/rest/tasksByDateAndGroupAndUser");

            networkManager.QueryString.Clear();
            networkManager.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");

            //string json = Encoding.ASCII.GetString(response);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            this.listTask = serializer.Deserialize<List<object>>(json);

            this.comboBoxTasks.Items.Clear();
            foreach(Dictionary<string, object> task in this.listTask) {
                //string taskName = (string)task["name"];
                string taskName = task["name"].ToString();
                this.comboBoxTasks.Items.Add(taskName);
            }
        }

        private void bookingPeriod_Click(object sender, RoutedEventArgs e) {
            //Button buttonPeriod = (Button)e.OriginalSource;
            //MessageBox.Show(buttonPeriod.Name);

            //if(buttonPeriod == this.buttonDayBooking) {
            if(sender == this.buttonDayBooking) {
                this.buttonDayBooking.IsEnabled = false;
                this.buttonWeekBooking.IsEnabled = true;
                this.buttonMonthBooking.IsEnabled = true;
                this.buttonLastWeekBooking.IsEnabled = true;
                this.buttonLastMonthBooking.IsEnabled = true;

                this.calendar.SelectedDate = DateTime.Today;
            }
            if(sender == this.buttonWeekBooking) {
                this.buttonDayBooking.IsEnabled = true;
                this.buttonWeekBooking.IsEnabled = false;
                this.buttonMonthBooking.IsEnabled = true;
                this.buttonLastWeekBooking.IsEnabled = true;
                this.buttonLastMonthBooking.IsEnabled = true;

                WeekBooking week = new WeekBooking();
                week.SetDateTimes();
                DateTime[] dateTimes = week.DateTimes;
                this.calendar.SelectedDates.AddRange(dateTimes[0], dateTimes[dateTimes.Length - 1]);
            }
            if(sender == this.buttonMonthBooking) {
                this.buttonDayBooking.IsEnabled = true;
                this.buttonWeekBooking.IsEnabled = true;
                this.buttonMonthBooking.IsEnabled = false;
                this.buttonLastWeekBooking.IsEnabled = true;
                this.buttonLastMonthBooking.IsEnabled = true;

                MonthBooking month = new MonthBooking();
                month.SetDateTimes();
                DateTime[] dateTimes = month.DateTimes;
                this.calendar.SelectedDates.AddRange(dateTimes[0], dateTimes[dateTimes.Length - 1]);
            }
            if(sender == this.buttonLastWeekBooking) {
                this.buttonDayBooking.IsEnabled = true;
                this.buttonWeekBooking.IsEnabled = true;
                this.buttonMonthBooking.IsEnabled = true;
                this.buttonLastWeekBooking.IsEnabled = false;
                this.buttonLastMonthBooking.IsEnabled = true;

                WeekBooking week = new WeekBooking();
                week.SubtractWeeks = 1;
                week.SetDateTimes();
                DateTime[] dateTimes = week.DateTimes;
                this.calendar.SelectedDates.AddRange(dateTimes[0], dateTimes[dateTimes.Length - 1]);
            }
            if(sender == this.buttonLastMonthBooking) {
                this.buttonDayBooking.IsEnabled = true;
                this.buttonWeekBooking.IsEnabled = true;
                this.buttonMonthBooking.IsEnabled = true;
                this.buttonLastWeekBooking.IsEnabled = true;
                this.buttonLastMonthBooking.IsEnabled = false;

                MonthBooking month = new MonthBooking();
                month.LastMonth = true;
                month.SetDateTimes();
                DateTime[] dateTimes = month.DateTimes;
                this.calendar.SelectedDates.AddRange(dateTimes[0], dateTimes[dateTimes.Length - 1]);
            }
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) {
            if(!calendarDateChangeFlag) {
                return;
            }
            
            Calendar calendar = (Calendar)sender;

            Nullable<DateTime> selDateTime = calendar.SelectedDate.Value;
            if(selDateTime == null) {
                return;
            }

            DateTime dateTime = (DateTime)selDateTime;
            //MessageBox.Show(dateTime.ToString());
            
            if(!this.buttonWeekBooking.IsEnabled) {
                WeekBooking booking = new WeekBooking(dateTime);
                booking.SetDateTimes();
                DateTime[] dateTimes = booking.DateTimes;
                
                this.calendarDateChangeFlag = false;
                this.calendar.SelectedDates.AddRange(dateTimes[0], dateTimes[dateTimes.Length - 1]);
            } else if(!this.buttonMonthBooking.IsEnabled) {
                MonthBooking booking = new MonthBooking(dateTime);
                booking.SetDateTimes();
                DateTime[] dateTimes = booking.DateTimes;
                
                this.calendarDateChangeFlag = false;
                this.calendar.SelectedDates.AddRange(dateTimes[0], dateTimes[dateTimes.Length - 1]);
            }
            
            calendarDateChangeFlag = true;
        }

        private void calendar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Calendar calendar = (Calendar)sender;
            Nullable<DateTime> dateTime = calendar.SelectedDate.Value;
            if(dateTime != null) {
                MessageBox.Show(dateTime.ToString());
            }
        }

        private void calendar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Calendar calendar = (Calendar)sender;
            Nullable<DateTime> dateTime = calendar.SelectedDate.Value;
            if(dateTime != null) {
                MessageBox.Show(dateTime.ToString());
            }
        }

        private void comboBoxTasks_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //MessageBox.Show(comboBoxTasks.SelectedItem.ToString());
            string taskId = "";
            foreach(Dictionary<string, object> task in this.listTask) {
                //SelectedItem.ToString()
                if(task["name"].ToString() == comboBoxTasks.SelectedValue.ToString()) {
                    //MessageBox.Show(task["id"].ToString());
                    taskId = task["id"].ToString();
                }
            }
            foreach(Task task in this.taskList) {
                if(task.Name == comboBoxTasks.SelectedValue.ToString()) {
                    taskId = task.ID;
                }
            }
            if(taskId == "") {
                return;
            }

            string unixTimestamp = DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            string timestamp = unixTimestamp + "000";
            /*
            NameValueCollection opts = new NameValueCollection();
            opts.Add("id", taskId);
            //opts.Add("date", "1500328800000");
            opts.Add("date", timestamp);
            opts.Add("tenant", settings["tenantvalue"]);
            //opts.Add("userId", settings["userid"]);

            NetworkManager networkManager = NetworkManager.getInstance();
            networkManager.QueryString = opts;
            networkManager.Headers.Remove(HttpRequestHeader.ContentType);

            //?date=1500328800000&id=2017-06-09+09:22:35.542451&tenant=O
            string json = networkManager.DownloadString("https://projectsuite.aeat.allianz.at/projectsuite/rest/categoryDomainsByTaskIdAndDate");

            networkManager.QueryString.Clear();
            //networkManager.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            this.categoryList = serializer.Deserialize<List<object>>(json);
            
            this.comboBoxCategory.Items.Clear();
            foreach(Dictionary<string, object> cat in this.categoryList) {
                string categoryName = cat["name"].ToString();
                this.comboBoxCategory.Items.Add(categoryName);
            }
            */

            NetworkManager networkManager = NetworkManager.getInstance();
            networkManager.Headers.Remove(HttpRequestHeader.ContentType);
            networkManager.QueryString.Clear();

            Task taskobj = new Task();
            taskobj.ID = taskId;

            CategoryByTaskAndDateGetRequest request = new CategoryByTaskAndDateGetRequest(taskobj, timestamp, this.settings["tenantvalue"]);
            request.callback = delegate(Object data) {
                List<TaskCategory> categories = (List<TaskCategory>)data;

                this.categoryList = new List<object>();
                this.comboBoxCategory.Items.Clear();

                foreach(TaskCategory cat in categories) {
                    this.categoryList.Add(cat);
                    this.comboBoxCategory.Items.Add(cat.Name);
                }
            };

            request.Get();
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e) {
            NetworkManager networkManager = NetworkManager.getInstance();
            //MessageBox.Show(networkManager.QueryString.Count.ToString());

            if(networkManager.Headers["X-Authorization"] == null){
                MessageBox.Show("You are not logged in", "Login Failure", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if(comboBoxTasks.SelectedValue == null) {
                comboBoxCategory.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }
            if(comboBoxCategory.SelectedValue == null) {
                //MessageBox.Show("No Category selected");
                comboBoxCategory.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }
            this.comboBoxTasks.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            this.comboBoxCategory.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            //MessageBox.Show(comboBoxCategory.SelectedValue.ToString() + Environment.NewLine + comboBoxCategory.SelectedItem.ToString());
            //return;

            string taskId = "";
            foreach(Dictionary<string, object> task in this.listTask) {
                if(task["name"].ToString() == comboBoxTasks.SelectedValue.ToString()) {
                    taskId = task["id"].ToString();
                }
            }

            //MessageBox.Show("taskId: " + taskId);

            string categoryId = "";
            /*
            foreach(Dictionary<string, object> cat in this.categoryList) {
                if(cat["name"].ToString() == comboBoxCategory.SelectedValue.ToString()) {
                    categoryId = cat["value"].ToString();
                }
            }*/

            foreach(TaskCategory taskCat in this.categoryList) {
                if(taskCat.Name == this.comboBoxCategory.SelectedValue.ToString()) {
                    categoryId = taskCat.Value;
                }
            }

            //MessageBox.Show("categoryId: " + categoryId);

            Int16 workAmount;
            if(textBoxWorkAmount.Text.Length==0 || !Int16.TryParse(textBoxWorkAmount.Text, out workAmount)) {
                this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }

            Int16 percentage;
            if(textBoxPercentage.Text.Length==0 || !Int16.TryParse(textBoxPercentage.Text, out percentage)) {
                this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }

            this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.DarkGray);

            /*
            string unixTimestamp = DateTime.Today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            string timestamp = unixTimestamp + "000";

            byte[] bytes = Encoding.Default.GetBytes(textBoxActivity.Text);
            string activity = Encoding.UTF8.GetString(bytes);

            Hashtable postdata = new Hashtable();
            //postdata.Add("taskName", "Citrix-General");
            //postdata.Add("taskName", "Jboss, Linux, Apache, Loadbalancing - AEV");
            //postdata.Add("date", "1499810400000");        //added 000
            postdata.Add("date", timestamp);
            postdata.Add("tenant", settings["tenantvalue"]);
            postdata.Add("duration", 1);
            //postdata.Add("category", "OP");             //in comboBoxCategory
            postdata.Add("category", categoryId);
            //postdata.Add("activity", "Test");
            postdata.Add("activity", activity);
            postdata.Add("userId", settings["userid"]);
            //postdata.Add("taskId", "2017-01-26 17:11:27.030806");       //taskId in listTask    //Jboss,Linux,Apache,Loadbalancing-AEV
            postdata.Add("taskId", taskId);
            */

            if(this.buttonDayBooking.IsEnabled == false) {
                MessageBox.Show("Day Booking");
                DayBooking booking = new DayBooking();
                
                //if(this.startDatePicker.SelectedDate.HasValue) {
                if(this.calendar.SelectedDate.HasValue){
                    //DateTime dt = this.startDatePicker.SelectedDate.Value;   //Nullable
                    DateTime dt = this.calendar.SelectedDate.Value;   //Nullable
                    PSDateTime dateTime = new PSDateTime(dt);
                    booking = new DayBooking(dateTime);
                }

                booking.webclient = networkManager;
                booking.TaskID = taskId;
                booking.Category = categoryId;
                booking.UserID = settings["userid"];
                booking.Tenant = settings["tenantvalue"];
                booking.Duration = 1;
                booking.Activity = textBoxActivity.Text;

                if(workAmount < 1 || workAmount > 24) {
                    this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }
                booking.Hours = workAmount;

                if(percentage < 1 || percentage > 100) {
                    this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }
                booking.Percentage = percentage;

                this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.DarkGray);

                booking.Send();
            }

            if(this.buttonWeekBooking.IsEnabled == false) {
                MessageBox.Show("Week Booking");
                Nullable<DateTime> dateTime = this.calendar.SelectedDates[this.calendar.SelectedDates.Count-1];
                //MessageBox.Show(dateTime.ToString());

                WeekBooking booking;
                if(dateTime != null) {
                    MessageBox.Show("calendar.SelectedDate is not null");
                    DateTime dt = (DateTime)dateTime;
                    booking = new WeekBooking(dt);
                } else {
                    booking = new WeekBooking();
                }
                
                booking.webclient = networkManager;
                booking.TaskID = taskId;
                booking.Category = categoryId;
                booking.UserID = settings["userid"];
                booking.Tenant = settings["tenantvalue"];
                booking.Duration = 1;
                booking.Activity = textBoxActivity.Text;
                
                int subtractWeeks = 0;
                if(textBoxWeekNumber.Text!="" && Int32.TryParse(textBoxWeekNumber.Text, out subtractWeeks)){
                    if(subtractWeeks < 0 || subtractWeeks > 4) {
                        this.textBoxWeekNumber.BorderBrush = new SolidColorBrush(Colors.Red);
                        return;
                    }
                    
                    /*
                    if(subtractWeeks > 0 && subtractWeeks <= 4) {
                        booking.SubtractWeeks = subtractWeeks;
                    }*/

                    booking.SubtractWeeks = subtractWeeks;
                }

                if(workAmount < 1 || workAmount > 130) {
                    this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }

                if(percentage < 1 || percentage > 100) {
                    this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }

                booking.Hours = workAmount;
                booking.Percentage = percentage;

                this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.DarkGray);

                booking.Send();
            }

            if(this.buttonMonthBooking.IsEnabled == false) {
                MessageBox.Show("Month Booking");
                Nullable<DateTime> dateTime = this.calendar.SelectedDates[this.calendar.SelectedDates.Count - 1];
                
                //MonthBooking booking = new MonthBooking();
                MonthBooking booking;
                if(dateTime != null) {
                    DateTime dt = (DateTime)dateTime;
                    booking = new MonthBooking(dt);
                } else {
                    booking = new MonthBooking();
                }
                
                booking.webclient = networkManager;
                booking.TaskID = taskId;
                booking.Category = categoryId;
                booking.UserID = settings["userid"];
                booking.Tenant = settings["tenantvalue"];
                booking.Duration = 1;
                booking.Activity = textBoxActivity.Text;

                if(workAmount < 1 || workAmount > 400) {
                    this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }

                if(percentage < 1 || percentage > 100) {
                    this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }

                booking.Hours = workAmount;
                booking.Percentage = percentage;

                this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.DarkGray);

                booking.Send();
            }

            if(this.buttonLastWeekBooking.IsEnabled == false) {
                MessageBox.Show("Last Week Booking");
                Nullable<DateTime> dateTime = this.calendar.SelectedDates[this.calendar.SelectedDates.Count - 1];

                //WeekBooking booking = new WeekBooking();
                WeekBooking booking;
                if(dateTime != null) {
                    MessageBox.Show("calendar.SelectedDate is not null");
                    DateTime dt = (DateTime)dateTime;
                    booking = new WeekBooking(dt);
                } else {
                    booking = new WeekBooking();
                }
                
                booking.webclient = networkManager;
                booking.TaskID = taskId;
                booking.Category = categoryId;
                booking.UserID = settings["userid"];
                booking.Tenant = settings["tenantvalue"];
                booking.Duration = 1;
                booking.Activity = textBoxActivity.Text;
                //booking.SubtractWeeks = 1;  //property for last week

                if(workAmount < 1 || workAmount > 130) {
                    this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }

                if(percentage < 1 || percentage > 100) {
                    this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }

                booking.Hours = workAmount;
                booking.Percentage = percentage;

                this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.DarkGray);

                booking.Send();
            }

            if(this.buttonLastMonthBooking.IsEnabled == false) {
                MessageBox.Show("Last Month Booking");

                Nullable<DateTime> dateTime = this.calendar.SelectedDates[this.calendar.SelectedDates.Count - 1];

                //MonthBooking booking = new MonthBooking();
                MonthBooking booking;
                if(dateTime != null) {
                    DateTime dt = (DateTime)dateTime;
                    booking = new MonthBooking(dt);
                } else {
                    booking = new MonthBooking();
                }

                booking.webclient = networkManager;
                booking.TaskID = taskId;
                booking.Category = categoryId;
                booking.UserID = settings["userid"];
                booking.Tenant = settings["tenantvalue"];
                booking.Duration = 1;
                booking.Activity = textBoxActivity.Text;
                //Property for last month
                booking.LastMonth = true;

                if(workAmount < 1 || workAmount > 400) {
                    this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }
                
                if(percentage < 1 || percentage > 100) {
                    this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }

                booking.Hours = workAmount;
                booking.Percentage = percentage;

                this.textBoxWorkAmount.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                this.textBoxPercentage.BorderBrush = new SolidColorBrush(Colors.DarkGray);

                booking.Send();
            }

            //booking.Send();
        }

        private void loginMenuItem_Click(object sender, RoutedEventArgs e) {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Owner = this;

            //this.webclient = new CookieAwareWebClient();
            //loginWindow.WebClient = this.webclient;

            //TODO: use delegate pattern instead of a reference
            this.settings = new Dictionary<string, string>();
            loginWindow.Settings = this.settings;

            loginWindow.ShowDialog();
        }

        private void infoMenuItem_Click(object sender, RoutedEventArgs e) {
            //string productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion
            string fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
            String infoMessage = "Version " + fileVersion;
            System.Windows.MessageBox.Show(infoMessage, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }    
    }
}
