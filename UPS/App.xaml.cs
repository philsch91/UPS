﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace UPS {
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App: Application {

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            //MessageBox.Show("OnStartup");
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            //MessageBox.Show("OnExit");
        }
    }
}
