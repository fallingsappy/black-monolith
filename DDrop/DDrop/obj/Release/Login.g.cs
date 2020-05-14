﻿#pragma checksum "..\..\Login.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "EF8A31832612492E82E43E63736A0EF3AAEE5B0CE0DD78077D254277487A9250"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DDrop.Controls.LoadingSpinner;
using SimpleSample;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace DDrop {
    
    
    /// <summary>
    /// Login
    /// </summary>
    public partial class Login : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DDrop.Controls.LoadingSpinner.AdornedControl LoadingAdorner;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ErrorMessage;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBoxEmail;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox LoginPasswordBox;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PasswordUnmask;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button LoginButton;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox RememberMe;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RegistrationButton;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox LocalUsersCombobox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DDrop;component/login.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Login.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.LoadingAdorner = ((DDrop.Controls.LoadingSpinner.AdornedControl)(target));
            return;
            case 2:
            this.ErrorMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.TextBoxEmail = ((System.Windows.Controls.TextBox)(target));
            
            #line 21 "..\..\Login.xaml"
            this.TextBoxEmail.TextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBoxLogin_OnTextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.LoginPasswordBox = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 23 "..\..\Login.xaml"
            this.LoginPasswordBox.PasswordChanged += new System.Windows.RoutedEventHandler(this.LoginPasswordBox_PasswordChanged);
            
            #line default
            #line hidden
            
            #line 25 "..\..\Login.xaml"
            this.LoginPasswordBox.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.LoginPasswordBox_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.PasswordUnmask = ((System.Windows.Controls.TextBox)(target));
            
            #line 26 "..\..\Login.xaml"
            this.PasswordUnmask.TextInput += new System.Windows.Input.TextCompositionEventHandler(this.TextBoxLogin_OnTextChanged);
            
            #line default
            #line hidden
            
            #line 27 "..\..\Login.xaml"
            this.PasswordUnmask.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.PasswordUnmask_TextChanged);
            
            #line default
            #line hidden
            
            #line 28 "..\..\Login.xaml"
            this.PasswordUnmask.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.PasswordUnmask_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.LoginButton = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\Login.xaml"
            this.LoginButton.Click += new System.Windows.RoutedEventHandler(this.LoginButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.RememberMe = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 8:
            this.RegistrationButton = ((System.Windows.Controls.Button)(target));
            
            #line 38 "..\..\Login.xaml"
            this.RegistrationButton.Click += new System.Windows.RoutedEventHandler(this.RegistrationButton_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 40 "..\..\Login.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Unchecked += new System.Windows.RoutedEventHandler(this.ToggleButton_OnUnchecked);
            
            #line default
            #line hidden
            
            #line 40 "..\..\Login.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Checked += new System.Windows.RoutedEventHandler(this.ToggleButton_OnChecked);
            
            #line default
            #line hidden
            return;
            case 10:
            this.LocalUsersCombobox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 46 "..\..\Login.xaml"
            this.LocalUsersCombobox.DropDownClosed += new System.EventHandler(this.LocalUsersCombobox_DropDownClosed);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

