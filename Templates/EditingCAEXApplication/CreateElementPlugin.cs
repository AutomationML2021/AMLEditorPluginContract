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
using System.Windows;
using System.Windows.Threading;
using AMLEditorPlugin.Contracts;

/// <summary>
/// The AutomationML.Plugin.Examples namespace.
/// </summary>
namespace AMLEditorPlugin
{
    /// <summary>
    /// The Class CreateElementPlugin is an example for a Plugin, which has it's own User Interface
    /// and which is an editing Plugin. The Export Attribute of this class enables the AutomationML
    /// Editor to load the Plugin with the <a
    /// href="http://msdn.microsoft.com/en-us/library/dd460648%28v=vs.110%29.aspx">Microsoft Managed
    /// Extensibility Framework</a>.    /// 
    /// Whenever an Editing Plugin is activated, the AutomationML Editor will block all User
    /// Interactions until the Plugin is terminated.    /// 
    /// The UI is startet in its own UI Thread. The Synchronisation of Method Calls between the
    /// AMLEditors Thread and the UI Thread is managed via a synchronisation Context. The Context is
    /// needed for sending events back to the AMLEditor from the Plugin UI
    /// </summary>
    [Export(typeof(IAMLEditorPlugin))]
    public class CreateElementPlugin : AMLEditorPlugin.Base.PluginBase
    {
        /// <summary>
        /// <see cref="AboutCommand"/>
        /// </summary>
        private RelayCommand<object> aboutCommand;
               
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

            // Add the About Command (recommended to exist in any Plugin)
            Commands.Add(new PluginCommand()
            {
                CommandName = "About",
                Command = AboutCommand
            });

            this.DisplayName = "InternalElement Generator"; 
        }

      
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
        /// Gets a value indicating whether this instance is reactive. Reactive Plugin will be
        /// notified, when the actual CAEX-Object changes (Selection of the Treeview Item) <see
        /// cref="ChangeAMLFilePath"/> and <see cref="ChangeSelectedObject"/>.
        /// </summary>
        /// <value><c>true</c> if this instance is reactive; otherwise, <c>false</c>.</value>
        public override bool IsReactive
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
        public override bool IsReadonly
        {
            // this one is not readonly, it can change the AML Document
            get { return false; }
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
        public override void ChangeAMLFilePath(string amlFilePath)
        {
            // the plugin is neither reactive not readonly. Nothing has to be done here
            ;
        }

        /// <summary>
        /// Changes the selected object. The Host Application will call this method when the Plugin
        /// <see cref="IsReactive"/> is set to true and the Current Selection changes in the Host Application.
        /// </summary>
        /// <param name="selectedObject">The selected object.</param>
        public override void ChangeSelectedObject(CAEX_ClassModel.CAEXBasicObject selectedObject)
        {
            // the plugin is neither reactive not readonly. Nothing has to be done here
            ;
        }

       
        /// <summary>
        /// This Method is called on activation of a Plugin. The AutomationML Editor 'publishes' its
        /// current state to the plugin, that is the Path of the loaded AutomationML Document and
        /// the currently selected AutomationML Object'. Please note, that the objects may be empty
        /// or null.
        /// </summary>
        /// <param name="amlFilePath">   The aml file path, may be empty.</param>
        /// <param name="selectedObject">The selected object, may be null.</param>
        public override void PublishAutomationMLFileAndObject(string amlFilePath, CAEX_ClassModel.CAEXBasicObject selectedObject)
        {
            // inform the View Model to load the document the View Model belongs to a different ui
            // thread, we need the dispatcher to send the change to the UI
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
        /// The <see cref="StartCommand"/> Execution Action. A new Dispatcher Tread will be created
        /// for the UI-Window. A Synchronisation Context is needed to send events back to the AMLEditor
        /// </summary>
        /// <param name="parameter">unused parameter.</param>
        protected override void ActivateCommandExecute(object parameter)
        {
            this.IsActive = true;

            // get the current Synchronisation Context (this is the AMLEditors Dispacther Thread of the Main Window)
            var syncContext = SynchronizationContext.Current;

            if (syncContext != null)
            {
                // Create a thread. The new Thread is the owner of all data objects
                Thread newWindowThread = new Thread(new ThreadStart(() =>
                {
                    // create the viewModel for the UI
                    this.viewModel = new CreateElementViewModel();

                    // Create a new context for the UI Thread, and install it:
                    SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(
                            Dispatcher.CurrentDispatcher));

                    // create the UI
                    this.ui = new CreateElementUI();                   

                    // set the Data Context to the View Model
                    this.ui.DataContext = this.viewModel;

                    // close event needs to be caught
                    this.ui.Closed += (s, e) =>
                    {
                        this.IsActive = false;
                        this.viewModel.SaveCommand.Execute(null);

                        // post the Terminated Event on the Synchronisation Context, so that the AMLEditor gets informed
                        syncContext.Post(o =>  this.RaisePluginTerminated (), this);

                        // Shut Down the Dispatcher Thread
                        Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                    };

                    // Showing the UI
                    this.ui.Show();

                    // Notify the Host Application, post the Activation Event on the Synchronisation Context
                    syncContext.Post(o =>  this.RaisePluginActivated(), this);

                    // Start the Dispatcher Processing after the Activation Event was raised
                    System.Windows.Threading.Dispatcher.Run();

                    // Extra Code here will be executed only, when the Dispatcher has been terminated

                    // .... 
                }));

                // Set the apartment state
                newWindowThread.SetApartmentState(ApartmentState.STA);
                
                // Make the thread a background thread (not required)
                newWindowThread.IsBackground = true;
                
                // Start the thread
                newWindowThread.Start();
            }
            else
            {
                MessageBox.Show("Couldn't activate the Plugin UI Thread! No current Synchronisation Context exists!");
            }

        }

         
       
        /// <summary>
        /// The <see cref="StopCommand"/> Execution Action on the Dispatcher Thread of the UI
        /// </summary>
        /// <param name="parameter">unused parameter.</param>
        protected override void TerminateCommandExecute(object parameter)
        {
            // we need the dispatcher again, to send the close command to the UI
            this.ui.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(ui.Close));
        }
    }
}