﻿#pragma checksum "..\..\..\Stock\ListeProduit.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A8A4FA95BEA8497F4014CB9738C1EEDAD82C9145"
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
using NewOptics.Stock;
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


namespace NewOptics.Stock {
    
    
    /// <summary>
    /// ListeProduit
    /// </summary>
    public partial class ListeProduit : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\Stock\ListeProduit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnNew;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\Stock\ListeProduit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSupp;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\Stock\ListeProduit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnEdit;
        
        #line default
        #line hidden
        
        
        #line 202 "..\..\..\Stock\ListeProduit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnImprimer;
        
        #line default
        #line hidden
        
        
        #line 264 "..\..\..\Stock\ListeProduit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock label;
        
        #line default
        #line hidden
        
        
        #line 265 "..\..\..\Stock\ListeProduit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRecherche;
        
        #line default
        #line hidden
        
        
        #line 271 "..\..\..\Stock\ListeProduit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid FournDataGrid;
        
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
            System.Uri resourceLocater = new System.Uri("/NewOptics;component/stock/listeproduit.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Stock\ListeProduit.xaml"
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
            this.btnNew = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\..\Stock\ListeProduit.xaml"
            this.btnNew.Click += new System.Windows.RoutedEventHandler(this.btnNew_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnSupp = ((System.Windows.Controls.Button)(target));
            
            #line 80 "..\..\..\Stock\ListeProduit.xaml"
            this.btnSupp.Click += new System.Windows.RoutedEventHandler(this.btnSupp_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnEdit = ((System.Windows.Controls.Button)(target));
            
            #line 141 "..\..\..\Stock\ListeProduit.xaml"
            this.btnEdit.Click += new System.Windows.RoutedEventHandler(this.btnEdit_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnImprimer = ((System.Windows.Controls.Button)(target));
            
            #line 202 "..\..\..\Stock\ListeProduit.xaml"
            this.btnImprimer.Click += new System.Windows.RoutedEventHandler(this.btnImprimer_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.label = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.txtRecherche = ((System.Windows.Controls.TextBox)(target));
            
            #line 265 "..\..\..\Stock\ListeProduit.xaml"
            this.txtRecherche.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtRecherche_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.FournDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 271 "..\..\..\Stock\ListeProduit.xaml"
            this.FournDataGrid.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.FournDataGrid_MouseDoubleClick);
            
            #line default
            #line hidden
            
            #line 271 "..\..\..\Stock\ListeProduit.xaml"
            this.FournDataGrid.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.FournDataGrid_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

