// *********************************************************************** Assembly :
// CreateElementPlugin Author : Josef Prinz Created : 12-08-2014
// 
// Last Modified By : Josef Prinz Last Modified On : 12-09-2014 ***********************************************************************
// <copyright file="CreateElementPlugin.cs" company="AutomationML e.V.">
//     Copyright (c) inpro. All rights reserved.
// </copyright>
// <summary>
// </summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Threading;
using AMLEditorPlugin.Contracts;

/// <summary>
/// The AutomationML.Plugin.Examples namespace.
/// </summary>
namespace AMLEditorPlugin
{
    /// <summary>
    /// The Class CreateElementPlugin is an example for a Plugin, which has it's own User Interface
    /// and which is an editing Plugin. Whenever an Editing Plugin is activated, the AutomationML
    /// Editor will block all User Interactions until the Plugin is terminated.
    /// </summary>
    [Export(typeof(IAMLEditorPlugin))]
    public class CreateElementPlugin : AMLEditorPlugin.Contracts.IAMLEditorPlugin
    {
        /// <summary>
        /// <see cref="AboutCommand"/>
        /// </summary>
        private RelayCommand<object> aboutCommand;

        /// <summary>
        /// <see cref="StartCommand"/>
        /// </summary>
        private RelayCommand<object> startCommand;

        /// <summary>
        /// <see cref="StopCommand"/>
        /// </summary>
        private RelayCommand<object> stopCommand;

        /// <summary>
        /// The UI for Plugin Interaction.
        /// </summary>
        private CreateElementUI ui;

        /// <summary>
        /// The view model for the Plugin
        /// </summary>
        private CreateElementViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateElementPlugin"/> class.
        /// </summary>
        public CreateElementPlugin()
        {
            // Add some commands to the Command list, which a user can select.
            Commands = new List<PluginCommand>();

            // Add a StartCommand
            Commands.Add(new PluginCommand()
            {
                CommandName = "Start",
                Command = StartCommand
            });

            // Add a Stop Command
            Commands.Add(new PluginCommand()
            {
                CommandName = "Stop",
                Command = StopCommand
            });

            // Add the About Command (recommended to exist in any Plugin)
            Commands.Add(new PluginCommand()
            {
                CommandName = "About",
                Command = AboutCommand
            });

            this.IsActive = false;
        }

        /// <summary>
        /// Occurs when [plugin activated].
        /// </summary>
        public event EventHandler PluginActivated;

        /// <summary>
        /// Occurs when [plugin terminated].
        /// </summary>
        public event EventHandler PluginTerminated;

        /// <summary>
        /// The AboutCommand - Command
        /// </summary>
        /// <value>The about command.</value>
        public System.Windows.Input.ICommand AboutCommand
        {
            get
            {
                return this.aboutCommand
                ??
                (this.aboutCommand = new RelayCommand<object>(this.AboutCommandExecute, this.AboutCommandCanExecute));
            }
        }

        /// <summary>
        /// Gets the commands for the Plugin.
        /// </summary>
        /// <value>The commands.</value>
        public List<AMLEditorPlugin.Contracts.PluginCommand> Commands
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return "InternalElement Generator"; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            private set;
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is reactive. Reactive Plugin will be
        /// notified, when the actual CAEX-Object changes (Selection of the Treeview Item) <see
        /// cref="ChangeAMLFilePath"/> and <see cref="ChangeSelectedObject"/>.
        /// </summary>
        /// <value><c>true</c> if this instance is reactive; otherwise, <c>false</c>.</value>
        public bool IsReactive
        {
            // this one is not reactive
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is readonly. No CAEX Objects should be
        /// modified by the Plugin, when set to true. If a Plugin is Readonly, the AmlEditor is
        /// still enabled, when the Plugin is Active. If a Plugin is not readonly the Editor is
        /// disbaled during activation. Please note, that the Editor will get disbaled, if only one
        /// of the currently activated Plugins of the Editor is not readonly.
        /// </summary>
        /// <value><c>true</c> if this instance is readonly; otherwise, <c>false</c>.</value>
        public bool IsReadonly
        {
            // this one is not readonly, it can change the AML Document
            get { return false; }
        }

        /// <summary>
        /// The Start - Command
        /// </summary>
        /// <value>The start command.</value>
        public System.Windows.Input.ICommand StartCommand
        {
            get
            {
                return this.startCommand
                ??
                (this.startCommand = new RelayCommand<object>(this.StartCommandExecute, this.StartCommandCanExecute));
            }
        }

        /// <summary>
        /// The Stop - Command
        /// </summary>
        /// <value>The stop command.</value>
        public System.Windows.Input.ICommand StopCommand
        {
            get
            {
                return this.stopCommand
                ??
                (this.stopCommand = new RelayCommand<object>(this.StopCommandExecute, this.StopCommandCanExecute));
            }
        }

        /// <summary>
        /// Changes the aml file path. This method is called for a reactive Plugin <see
        /// cref="IsReactive"/> only. Those Plugins will be informed, when the loaded AutomationML
        /// Document in the AMLEditor changes. This can only happen, if the plugin is readonly and
        /// the AMLEditor is not disabled. The AMLEditor will be disabled for active Plugins, which
        /// are not readonly.
        /// </summary>
        /// <param name="amlFilePath">The aml file path.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void ChangeAMLFilePath(string amlFilePath)
        {
            // the plugin is neither reactive not readonly. Nothing has to be done here
            ;
        }

        /// <summary>
        /// Changes the selected object. The Host Application will call this method when the Plugin
        /// <see cref="IsReactive"/> is set to true and the Current Selection changes in the Host Application.
        /// </summary>
        /// <param name="selectedObject">The selected object.</param>
        public void ChangeSelectedObject(CAEX_ClassModel.CAEXBasicObject selectedObject)
        {
            // the plugin is neither reactive not readonly. Nothing has to be done here
            ;
        }

        /// <summary>
        /// This Method is called from the AutomationML Editor to execute a specific command. The
        /// Editor can only execute those commands, which are identified by the <see
        /// cref="PluginCommandsEnum"/> Enumeration. The Editor may Exceute the termination command
        /// of the plugin, so here some preparations for a clean termination should be performed.
        /// </summary>
        /// <param name="command">The command.</param>
        public void ExecuteCommand(AMLEditorPlugin.Contracts.PluginCommandsEnum command)
        {
            switch (command)
            {
                case PluginCommandsEnum.Terminate:
                    StopCommandExecute(null);
                    break;
            }
        }

        /// <summary>
        /// This Method is called on activation of a Plugin. The AutomationML Editor 'publishes' its
        /// current state to the plugin, that is the Path of the loaded AutomationML Document and
        /// the currently selected AutomationML Object'. Please note, that the objects may be empty
        /// or null.
        /// </summary>
        /// <param name="amlFilePath">   The aml file path, may be empty.</param>
        /// <param name="selectedObject">The selected object, may be null.</param>
        public void PublishAutomationMLFileAndObject(string amlFilePath, CAEX_ClassModel.CAEXBasicObject selectedObject)
        {
            // inform the View Model to load the document
            // the View Model belongs to a different ui thread, we need the dispatcher for the change

            this.ui.Dispatcher.Invoke(DispatcherPriority.Normal,
                new ThreadStart(() => { this.viewModel.AmlFilePath = amlFilePath; }));
        }

        /// <summary>
        /// Test, if the <see cref="AboutCommand"/> can execute.
        /// </summary>
        /// <param name="parameter">unused.</param>
        /// <returns>true, if command can execute</returns>
        private bool AboutCommandCanExecute(object parameter)
        {
            // Execution is always possible, also for inactive plugins
            return true;
        }

        /// <summary>
        /// The <see cref="AboutCommand"/> Execution Action.
        /// </summary>
        /// <param name="parameter">unused.</param>
        private void AboutCommandExecute(object parameter)
        {
            var dialog = new About();
            dialog.ShowDialog();
        }

        /// <summary>
        /// Test, if the Plugin in not already active and the <see cref="StartCommand"/> can execute.
        /// </summary>
        /// <param name="parameter">unused parameter.</param>
        /// <returns>true, if command can execute</returns>
        private bool StartCommandCanExecute(object parameter)
        {
            return !this.IsActive;
        }

        /// <summary>
        /// The <see cref="StartCommand"/> Execution Action. A new Dispatcher Tread will be created
        /// for the UI-Window.
        /// </summary>
        /// <param name="parameter">unused parameter.</param>
        private void StartCommandExecute(object parameter)
        {
            this.IsActive = true;

            var syncContext = SynchronizationContext.Current; 

            // Create a thread. The new Thread is the owner of all data objects
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                this.viewModel = new CreateElementViewModel();              

                // Create our context, and install it:
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(
                        Dispatcher.CurrentDispatcher));

                // create the UI
                this.ui = new CreateElementUI();

                // close event needs to be caught
                this.ui.Closed += (s, e) =>
                    {
                        this.IsActive = false;
                        this.viewModel.SaveCommand.Execute(null);

                        if (PluginTerminated != null)
                        {
                            syncContext.Post(o => PluginTerminated(this, EventArgs.Empty), null);
                        };                       

                        Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                    };

                // setting the Data Context to the View Model
                this.ui.DataContext = this.viewModel;

                // Showing the UI
                this.ui.Show();

                // Notify the Host Application
                if (PluginActivated != null)
                    syncContext.Post(o => PluginActivated(this, EventArgs.Empty), this);

                // Start the Dispatcher Processing
                System.Windows.Threading.Dispatcher.Run();              

            }));

            // Set the apartment state
            newWindowThread.SetApartmentState(ApartmentState.STA);
            // Make the thread a background thread
            newWindowThread.IsBackground = true;
            // Start the thread
            newWindowThread.Start();

           
        }

        /// <summary>
        /// Test, if the Plugin is active and the <see cref="StopCommand"/> can execute.
        /// </summary>
        /// <param name="parameter">unused parameter.</param>
        /// <returns>true, if command can execute</returns>
        private bool StopCommandCanExecute(object parameter)
        {
            return this.IsActive;
        }

        /// <summary>
        /// The <see cref="StopCommand"/> Execution Action.
        /// </summary>
        /// <param name="parameter">unused parameter.</param>
        private void StopCommandExecute(object parameter)
        {
            this.ui.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(ui.Close));
        }

       

      
    }
}