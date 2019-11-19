using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.IO;

namespace UPS {
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App: Application {

        string logfilePath = @"UPS.log";    //C:\logs\UPS.log

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            //MessageBox.Show("OnStartup");

            this.SetupExceptionHandling();
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            //MessageBox.Show("OnExit");
        }

        private void SetupExceptionHandling() {
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            // Process unhandled exception
            Exception exception = e.Exception;
            
            this.WriteLog(exception.Message);
            this.WriteLog(exception.StackTrace);

            // Prevent default unhandled exception processing
            e.Handled = true;
        }

        private bool WriteLog(string text) {
            if(Path.IsPathRooted(this.logfilePath)) {
                string path = Path.GetFullPath(this.logfilePath);

                if(!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
            }

            text = DateTime.Now.ToString() + " " + text;

            if(text.Substring(text.Length-Environment.NewLine.Length, Environment.NewLine.Length) != Environment.NewLine){
                text += Environment.NewLine;
            }

            try {
                File.AppendAllText(this.logfilePath, text);
            } catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            
            return true;
        }
    }
}
