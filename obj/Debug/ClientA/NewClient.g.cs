﻿#pragma checksum "..\..\..\ClientA\NewClient.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "01F548BD9FFFB10F9A487109345C11E1CA8F7ED4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MahApps.Metro.Controls;
using NewOptics.ClientA;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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


namespace NewOptics.ClientA {
    
    
    /// <summary>
    /// NewClient
    /// </summary>
    public partial class NewClient : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid PatientGrid;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label f;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRéf;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRaison;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtadresse;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txttel;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtmf;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtai;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtrc;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtsolde;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\ClientA\NewClient.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCreer;
        
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
            System.Uri resourceLocater = new System.Uri("/NewOptics;component/clienta/newclient.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ClientA\NewClient.xaml"
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
            this.PatientGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.f = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.txtRéf = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.txtRaison = ((System.Windows.Controls.TextBox)(target));
            
            #line 38 "..\..\..\ClientA\NewClient.xaml"
            this.txtRaison.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtRaison_TextChanged_1);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txtadresse = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.txttel = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.txtmf = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.txtai = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.txtrc = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.txtsolde = ((System.Windows.Controls.TextBox)(target));
            
            #line 61 "..\..\..\ClientA\NewClient.xaml"
            this.txtsolde.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtCrédit_KeyDown);
            
            #line default
            #line hidden
            return;
            case 11:
            this.btnCreer = ((System.Windows.Controls.Button)(target));
            
            #line 62 "..\..\..\ClientA\NewClient.xaml"
            this.btnCreer.Click += new System.Windows.RoutedEventHandler(this.btnCreer_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

