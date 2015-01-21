// *********************************************************************** Assembly :
// AMLEditorPlugin Author : Josef Prinz Created : 01-19-2015
// 
// Last Modified By : Josef Prinz Last Modified On : 01-20-2015 ***********************************************************************
// <copyright file="HelloAml.xaml.cs" company="AutomationML e.V.">
//     Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary>
// </summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using AMLEditorPlugin.Contracts;
using AMLEngineExtensions;

/// <summary>
/// The AMLEditorPlugin namespace.
/// </summary>
namespace AMLEditorPlugin
{
    /// <summary>
    /// HelloAml is an example Plugin, which implements the IAMLEditorView Interface. The Plugin is
    /// a UserControl, which is managed by the AutomationML Editors Window- and Docking - Manager.
    /// The Export Attribute enables the AutomationML Editor to load the Plugin with the <a
    /// href="http://msdn.microsoft.com/en-us/library/dd460648%28v=vs.110%29.aspx">Microsoft Managed
    /// Extensibility Framework</a>. The Plugin will show the String "Hello AML" and reacts on a
    /// selection event with output of the Name of the Selected CAEX Object. It also has a command,
    /// to change the string from lower- to uppercase and vice versa and an About Command which
    /// displays the Disclaimer
    /// </summary>
    [Export(typeof(IAMLEditorView))]
    public partial class HelloAml : UserControl, IAMLEditorView
    {
        /// <summary>
        /// <see cref="AboutCommand"/>
        /// </summary>
        private RelayCommand<object> aboutCommand;

        /// <summary>
        /// <see cref="InvertCase"/>
        /// </summary>
        private RelayCommand<object> invertCase;

        /// <summary>
        /// Indication if the objectname is shown in lower- or uppercase
        /// </summary>
        private bool isLowerCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelloAml"/> class.
        /// </summary>
        public HelloAml()
        {
            // Defines the Command list, which will contain user commands, which a user can select
            // via the Plugin Menue.
            Commands = new List<PluginCommand>();



            ActivatePlugin = new PluginCommand()
            {
                Command = new RelayCommand<object>(this.StartCommandExecute, this.StartCommandCanExecute),
                CommandName = "Start",
                CommandToolTip = "Start the Plugin"
            };

            TerminatePlugin = new PluginCommand()
            {
                Command = new RelayCommand<object>(this.StopCommandExecute, this.StopCommandCanExecute),
                CommandName = "Stop",
                CommandToolTip = "Stop the Plugin"
            };

            InitializeComponent();


            // Add the StartCommand (should exist in any Plugin)
            Commands.Add(ActivatePlugin);

            // Add the Stop Command (should exist in any Plugin)
            Commands.Add(TerminatePlugin);

            // Add the change spelling command (an additional command)
            Commands.Add(new PluginCommand()
            {
                CommandName = "Change Spelling",
                Command = InvertCase,
                CommandToolTip = "Change from Uppercase to Lowercase and vice versa"
            });

            // Add the About Command (recommended to exist in any Plugin)
            Commands.Add(new PluginCommand()
            {
                CommandName = "About",
                Command = AboutCommand,
                CommandToolTip = "Information about this plugin"
            });

            this.IsActive = false;
        }

        /// <summary>
        /// Occurs when the Plugin is activated (for example via the <see cref="StartCommand"/> ).
        /// </summary>
        public event EventHandler PluginActivated;

        /// <summary>
        /// Occurs when the Plugin is deactivated (some UserInteraction inside the Plugin or via the
        /// <see cref="StopCommand"/> ).
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
        /// Gets the activate plugin.
        /// </summary>
        /// <value>The activate plugin.</value>
        public PluginCommand ActivatePlugin
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this UserControl could be closed from the Editor's
        /// WindowManager. When a close occurs from the WindowManager, the StopCommand will be
        /// executed via the <see cref="ExecuteCommand"/> Method.
        /// </summary>
        /// <value><c>true</c> if this instance can close; otherwise, <c>false</c>.</value>
        public bool CanClose
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the List of commands, which are viewed in the Plugin Menue in the Host Application
        /// </summary>
        /// <value>The command List.</value>
        public List<PluginCommand> Commands
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the display name which is shown in the Plugin Menu in the Host Application
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return "Hello AML"; }
        }

        /// <summary>
        /// The InvertCase - Command
        /// </summary>
        /// <value>The invert case.</value>
        public System.Windows.Input.ICommand InvertCase
        {
            get
            {
                return this.invertCase
                ??
                (this.invertCase = new RelayCommand<object>(this.InvertCaseExecute, this.InvertCaseCanExecute));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is active. The Property should be set to
        /// true in the <see cref="StartCommand"/> and set to false in the <see cref="StopCommand"/>
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is reactive. Reactive Plugin will be
        /// notified, when the actual CAEX-Object changes (Selection of the Treeview Item) <see
        /// cref="ChangeAMLFilePath"/> and <see cref="ChangeSelectedObject"/>.
        /// </summary>
        /// <value><c>true</c> if this instance is reactive; otherwise, <c>false</c>.</value>
        public bool IsReactive
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is readonly. No CAEX Objects should be
        /// modified by the Plugin, when set to true. If a Plugin is Readonly, the AmlEditor is
        /// still enabled, when the Plugib is Active. If a Plugin is not readonly the Editor is
        /// disbaled. Please note, that the Editor is disbaled, if only one of the currently
        /// activated Plugins is not readonly.
        /// </summary>
        /// <value><c>true</c> if this instance is readonly; otherwise, <c>false</c>.</value>
        public bool IsReadonly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the terminate plugin.
        /// </summary>
        /// <value>The terminate plugin.</value>
        public PluginCommand TerminatePlugin
        {
            get;
            private set;
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
            this.HelloText.Text = "Hello " + System.IO.Path.GetFileName(amlFilePath);
        }

        /// <summary>
        /// Changes the selected object. The Host Application will call this method when the Plugin
        /// <see cref="IsReactive"/> is set to true and the Current Selection changes in the Host Application.
        /// </summary>
        /// <param name="selectedObject">The selected object.</param>
        public void ChangeSelectedObject(CAEX_ClassModel.CAEXBasicObject selectedObject)
        {
            if (selectedObject != null)
            {
                this.HelloText.Text = "Hello " + "\"" + selectedObject.Name() + "\"";
            }
        }

        /// <summary>
        /// This Method is called from the AutomationML Editor to execute a specific command. The
        /// Editor can only execute those commands, which are identified by the <see
        /// cref="PluginCommandsEnum"/> Enumeration. The Editor may Exceute the termination command
        /// of the plugin, so here some preparations for a clean termination should be performed.
        /// </summary>
        /// <param name="command">    The command.</param>
        /// <param name="amlFilePath">The amlFilePath.</param>
        public void ExecuteCommand(PluginCommandsEnum command, string amlFilePath)
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
            if (!string.IsNullOrEmpty(amlFilePath))
                this.HelloText.Text = "Hello " + System.IO.Path.GetFileName(amlFilePath);
            else
                this.HelloText.Text = "Nobody to say hello to!";

            if (selectedObject != null)
            {
                // ToDo Implementation of object specific handling
            }
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
        /// Test, if the <see cref="InvertCase"/> can execute. The Command can execute only, if the
        /// <see cref="IsActive"/> Property is true:
        /// </summary>
        /// <param name="parameter">unused.</param>
        /// <returns>true, if command can execute</returns>
        private bool InvertCaseCanExecute(object parameter)
        {
            // the command can execute only if the plugin is active
            return this.IsActive;
        }

        /// <summary>
        /// The <see cref="InvertCase"/> Execution Action.
        /// </summary>
        /// <param name="parameter">unused.</param>
        private void InvertCaseExecute(object parameter)
        {
            if (isLowerCase)
                this.HelloText.Text = this.HelloText.Text.ToUpper();
            else
                this.HelloText.Text = this.HelloText.Text.ToLower();

            isLowerCase = !isLowerCase;
        }

        /// <summary>
        /// Test, if the <see cref="StartCommand"/> can execute. The <see cref="IsActive"/> Property
        /// should be false prior to Activation.
        /// </summary>
        /// <param name="parameter">unused</param>
        /// <returns>true, if command can execute</returns>
        private bool StartCommandCanExecute(object parameter)
        {
            return !this.IsActive;
        }

        /// <summary>
        /// The <see cref="StartCommand"/> s execution Action. The <see cref="PluginActivated"/>
        /// event is raised and the <see cref="IsActive"/> Property is set to true.
        /// </summary>
        /// <param name="parameter">unused</param>
        private void StartCommandExecute(object parameter)
        {
            this.IsActive = true;
            if (PluginActivated != null)
                PluginActivated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Test, if the <see cref="StopCommand"/> can execute.
        /// </summary>
        /// <param name="parameter">unused</param>
        /// <returns>true, if command can execute</returns>
        private bool StopCommandCanExecute(object parameter)
        {
            return this.IsActive;
        }

        /// <summary>
        /// The <see cref="StopCommand"/> Execution Action sets the <see cref="IsActive"/> Property
        /// to false. The <see cref="PluginTerminated"/> event will be raised.
        /// </summary>
        /// <param name="parameter">unused</param>
        private void StopCommandExecute(object parameter)
        {
            this.IsActive = false;
            if (PluginTerminated != null)
                PluginTerminated(this, EventArgs.Empty);
        }
    }
}