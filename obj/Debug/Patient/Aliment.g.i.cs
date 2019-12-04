﻿#pragma checksum "..\..\..\Patient\Aliment.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "172ECEDE9C7FB5E43825BCC236F8DCBE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DevExpress.Core;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.DXBinding;
using GestionClinique.Patient;
using MahApps.Metro.Controls;
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


namespace GestionClinique.Patient {
    
    
    /// <summary>
    /// Aliment
    /// </summary>
    public partial class Aliment : DevExpress.Xpf.Core.DXWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnNew;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSupp;
        
        #line default
        #line hidden
        
        
        #line 137 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnEdit;
        
        #line default
        #line hidden
        
        
        #line 195 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnImprimer;
        
        #line default
        #line hidden
        
        
        #line 253 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnVider;
        
        #line default
        #line hidden
        
        
        #line 311 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnImport;
        
        #line default
        #line hidden
        
        
        #line 370 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnVisualiser;
        
        #line default
        #line hidden
        
        
        #line 429 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock label;
        
        #line default
        #line hidden
        
        
        #line 430 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRecherche;
        
        #line default
        #line hidden
        
        
        #line 431 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbFamille;
        
        #line default
        #line hidden
        
        
        #line 434 "..\..\..\Patient\Aliment.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid FamilleAlimentDataGrid;
        
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
            System.Uri resourceLocater = new System.Uri("/Diabetosoft;component/patient/aliment.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Patient\Aliment.xaml"
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
            
            #line 19 "..\..\..\Patient\Aliment.xaml"
            this.btnNew.Click += new System.Windows.RoutedEventHandler(this.btnNew_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnSupp = ((System.Windows.Controls.Button)(target));
            
            #line 79 "..\..\..\Patient\Aliment.xaml"
            this.btnSupp.Click += new System.Windows.RoutedEventHandler(this.btnSupp_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnEdit = ((System.Windows.Controls.Button)(target));
            
            #line 137 "..\..\..\Patient\Aliment.xaml"
            this.btnEdit.Click += new System.Windows.RoutedEventHandler(this.btnEdit_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnImprimer = ((System.Windows.Controls.Button)(target));
            
            #line 195 "..\..\..\Patient\Aliment.xaml"
            this.btnImprimer.Click += new System.Windows.RoutedEventHandler(this.btnImprimer_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnVider = ((System.Windows.Controls.Button)(target));
            
            #line 253 "..\..\..\Patient\Aliment.xaml"
            this.btnVider.Click += new System.Windows.RoutedEventHandler(this.btnVider_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnImport = ((System.Windows.Controls.Button)(target));
            
            #line 311 "..\..\..\Patient\Aliment.xaml"
            this.btnImport.Click += new System.Windows.RoutedEventHandler(this.btnImport_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnVisualiser = ((System.Windows.Controls.Button)(target));
            
            #line 370 "..\..\..\Patient\Aliment.xaml"
            this.btnVisualiser.Click += new System.Windows.RoutedEventHandler(this.btnVisualiser_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.label = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.txtRecherche = ((System.Windows.Controls.TextBox)(target));
            
            #line 430 "..\..\..\Patient\Aliment.xaml"
            this.txtRecherche.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtRecherche_TextChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.cbFamille = ((System.Windows.Controls.ComboBox)(target));
            
            #line 431 "..\..\..\Patient\Aliment.xaml"
            this.cbFamille.DropDownClosed += new System.EventHandler(this.cbFamille_DropDownClosed);
            
            #line default
            #line hidden
            
            #line 431 "..\..\..\Patient\Aliment.xaml"
            this.cbFamille.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.cbFamille_MouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            case 11:
            this.FamilleAlimentDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 434 "..\..\..\Patient\Aliment.xaml"
            this.FamilleAlimentDataGrid.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.FamilleAlimentDataGrid_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

