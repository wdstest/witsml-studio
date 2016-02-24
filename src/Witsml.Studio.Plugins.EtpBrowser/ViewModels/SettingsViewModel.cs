﻿using Caliburn.Micro;
using PDS.Witsml.Studio.Connections;
using PDS.Witsml.Studio.Plugins.EtpBrowser.Models;
using PDS.Witsml.Studio.ViewModels;

namespace PDS.Witsml.Studio.Plugins.EtpBrowser.ViewModels
{
    /// <summary>
    /// Manages the behavior of the settings view.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.Screen" />
    public class SettingsViewModel : Screen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            DisplayName = "Settings";
        }

        /// <summary>
        /// Gets or Sets the Parent <see cref="T:Caliburn.Micro.IConductor" />
        /// </summary>
        public new MainViewModel Parent
        {
            get { return (MainViewModel)base.Parent; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public EtpSettings Model
        {
            get { return Parent.Model; }
        }

        /// <summary>
        /// Shows the connection dialog.
        /// </summary>
        public void ShowConnectionDialog()
        {
            var viewModel = new ConnectionViewModel(ConnectionTypes.Etp)
            {
                Connection = Model.Connection
            };

            if (App.Current.ShowDialog(viewModel))
            {
                Model.Connection = viewModel.Connection;
                Parent.OnConnectionChanged();
            }
        }
    }
}
