   M S B A M L   `   `   `  ����T  PMahApps.Metro, Version=1.2.2.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  5	       	NewOptics$   NewOptics.Atelier.ListeMonture   5      Adxwui7http://schemas.devexpress.com/winfx/2008/xaml/windowsui  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Adx2http://schemas.devexpress.com/winfx/2008/xaml/core     5      Edxe5http://schemas.devexpress.com/winfx/2008/xaml/editors     5      Bdxg2http://schemas.devexpress.com/winfx/2008/xaml/grid     5      PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5	      9Custom,http://metro.mahapps.com/winfx/xaml/controls   5
      OdxwuinBhttp://schemas.devexpress.com/winfx/2008/xaml/windowsui/navigation   5      <dxc2http://schemas.devexpress.com/winfx/2008/xaml/core   5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006   5      :d2http://schemas.microsoft.com/expression/blend/2008   5        N� 
Background#  � ��6D   .��5      �  � RowDefinitions 5   
   �� 5      $
��80px �6   �� 5      �� 5      $
��40px �6   .+�5   
   ]� $��0��6   $
��Left=�6!   .+�5      �� $"��liste Des commandes montures��6   $��Tahoma)�6;   $��20(�6O   $
��Bold"�6]   $��Center=�6o   $��Center=�6�   5      ]� 5   
   $��	350,0,0,0q�6   $
��Auto=�6(   $��Disabled=�6Z   $��Right=�6�   $
��Left=�6�   ��5      �� 5      .+�5       �� ��. $��Disabled=�6,   $
��Auto=�6`   .��5!      ��  �� Orientation$ 
Horizontal=�6   .+�5"      ]� $��0,6,6,4q�6!    �� PanningMode$ HorizontalOnly=�62   $
��Auto=�6\   $
��Auto=�6�   $
��Left=�6�   $��2��6�   $��1��6�   .+�5#      �� -   5         ��Name$ 	btnFilter��5#   !   $��Right=�6L   $��Right=�6h   $�� ��6�   $��Times New Roman)�6�   $��12(�6�    � ToolTip$ Filtrer��6�   $��20��6�   $��5,0,0,0q�6�   $��20��6�   $��20��6  $��20��6  ��6�   Z� ��5$      �� 5%   "    �� ImageSource$X R/NewOptics;component/Images/acept-verifier-corriger-vert-ok-oui-icone-9668-128.png��6-   ��5'      �� 5(   "    �� 
TargetType" ��6(    �� BasedOn 5)   &   �� 5*   *   " ��60   .�5+   .   �� +   %MahApps.Metro.Controls.ControlsHelper	 �� Property	 �@ ContentCharacterCasing
 �� Value$
 Upper=�6o   �� 5,   .   	 �@ CornerRadius$
 3��6e   �� 5-   .   	 � ��$
 25��6J   �� 5.   .   	 � ��$

 Bold"�6K   �� 5/   .   	 � ��   UpperCaseContentFontSize#
 �   6I   �� 50   .   	 � ��  DefaultFont#
 �  6K   �� 51   .   	 � ��  GrayBrush10#
 �  6K   �� 52   .   	 � ��  TextBoxBorderBrush#
 �  6L   �� 53   .   	 � ��  	TextBrush#
 �  6K   �� 54   .   	 � ��$	
 5,6q�6H   �� 55   .   	 � ��$
 1q�6P   �� 56   .   	 � ��
 57   2   �� 58   6    �� 
TargetType" ��6F   .R�59   :   � .+�5:   >   ��  ��Name$ 
Background��6E   #��z��6Y    �� CornerRadius 6�   �� *(&(Controls:ControlsHelper.CornerRadius)+ �� Mode$ OneWay=� �� RelativeSource �� *TemplatedParent+�� 5;   >   $ Border��6E   #��z��6U   #��z��6�    J� SnapsToDevicePixels# z 6I  ��6�   Z�  6�   �� *(&(Controls:ControlsHelper.CornerRadius)+$ OneWay=� �� *TemplatedParent+6I  �� 5<   >   $ DisabledVisualElement��6E     ControlsDisabledBrush#���  6d    J� IsHitTestVisible .   J� Opacity$ 0Q�60  # z 6<   6�   �� *(&(Controls:ControlsHelper.CornerRadius)+$ OneWay=� �� *TemplatedParent+6<  -   'MahApps.Metro.Controls.ContentControlEx  5=   >     Name$ PART_ContentPresenter��6X   #��z��6�   #��z��60   �� ContentStringFormat# z 6T  #��z��6�  #��z��6�     RecognizesAccessKey . # z 6  #��z��6U     ContentCharacterCasing 6w   �� *20(Controls:ControlsHelper.ContentCharacterCasing)+ �� *TemplatedParent+6U  5>   ;    �� Triggers 5?   :   P� 5@   >    P� Property �@J�IsMouseOver P� Value . . �5B   B   ��  �� 
TargetName$ DisabledVisualElement��6\   	 �@J�Opacity$	
 0.3Q�6   �� 5C   B   $ PART_ContentPresenter��6\   	 �@J�Opacity$	
 0.3Q�6   �� 5D   B   $ 
Background��6\   	 �@J�Opacity$	
 0.3Q�6t   5E   ?   P� 5F   >    � �� . . �5G   B   �� $ DisabledVisualElement��6\   	 �@J�Opacity$	
 0.3Q�6   �� 5H   B   $ PART_ContentPresenter��6\   	 �@J�Opacity$	
 0.3Q�6   �� 5I   B   $ 
Background��6\   	 �@J�Opacity$	
 0.3Q�6t   5J   ?   P� 5K   >    �@J�IsKeyboardFocusWithin . . �5L   B   �� $ DisabledVisualElement��6\   	 �@J�Opacity$	
 0.3Q�6   �� 5M   B   $ PART_ContentPresenter��6\   	 �@J�Opacity$	
 0.3Q�6   �� 5N   B   $ 
Background��6\   	 �@J�Opacity$	
 0.3Q�6t   5O   ?   P� 5P   >    � {� .  . �5Q   B   �� $ DisabledVisualElement��6\   	 �@J�Opacity$	
 0.7Q�6   �� 5R   B   $ PART_ContentPresenter��6\   	 �@J�Opacity$	
 0.3Q�6   5S   ?   5U   7   5W   /   5X   +   5Z   #   5]      	� 5^      -   5         	�Name$ ParDateCommande��5^   &    d� 	IsChecked$
 TrueY�6?    	� 	GroupName$ 
DateFiltre��6P   $��Par date commande��6g   $
��Left=�6�   $	��Top=�6�   $��0,15,0,0q�6�   $��Tahoma)�6�   	� 5_      -   5        $ ParDateLivraison��5_   &   $ 
DateFiltre��6@   $��Par date Livraison��6W   $
��Left=�6t   $��Center=�6�   $��-130,20,0,0q�6�   $��Tahoma)�6�   (  "System.Windows.Controls.DatePicker  5a      -   5          Name$ DateSaisieFin��5a   %   $��Center=�6<   $��14(�6W   $��Tahoma)�6e   $
��Bold"�6y   ���	#FF2D3F5D$��0,0,10,0q�6�   $��Right=�6�   $��Right=�6�   ���	#FF2D3F5D$	��130��6�   $	��130��6  �� 5b      $	��au:��6%   $��Center=�60   $
��Bold"�6K   $��14(�6]   $��0,0,10,0q�6k   $��Tahoma)�6}   $��Right=�6�   $��Right=�6�   #��� ��6�     5c      -   5        $ DateSaisieDébut��5c   %   $��Center=�6>   $��14(�6Y   $��Tahoma)�6g   $
��Bold"�6{   ���	#FF2D3F5D$��0,0,10,0q�6�   $��Right=�6�   $��Right=�6�   ���	#FF2D3F5D$	��130��6  $	��130��6  �� 5d      $	��Du:��6%   $��Center=�60   $
��Bold"�6K   $��14(�6]   $
��10,0q�6k   $��Tahoma)�6y   $��Right=�6�   $��Right=�6�   #��� ��6�   5f      �� 5g      -   5        $ btnNew��5g      $��Tahoma)�6-   $�� ��6V   $��12(�6a   #��� ��6o   $ Nouveau��6�   $��20��6�   $��20��6�   $��Center=�6�   $��25��6  $��25��6*  $��20,0,0,0q�66  ��6  Z� ��5h      �� 5i      " ��6$    5j   "   �� 5k   &   " ��6,   .�5l   *   �� 	 �@ ContentCharacterCasing$
 Upper=�6k   �� 5m   *   	 �@ CornerRadius$
 3��6a   �� 5n   *   	 � ��$
 25��6F   �� 5o   *   	 � ��$

 Bold"�6G   �� 5p   *   	 � ��#
 �   6E   �� 5q   *   	 � ��#
 �  6G   �� 5r   *   	 � ��#
 �  6G   �� 5s   *   	 � ��#
 �  6H   �� 5t   *   	 � ��#
 �  6G   �� 5u   *   	 � ��$	
 5,6q�6D   �� 5v   *   	 � ��$
 1q�6L   �� 5w   *   	 � ��
 5x   .   �� 5y   2   " ��6B   .R�5z   6   � .+�5{   :   �� $ 
Background��6A   #��z��6U    6   �� *(&(Controls:ControlsHelper.CornerRadius)+$ OneWay=� �� *TemplatedParent+�� 5|   :   $ Border��6A   #��z��6Q   #��z��6}   # z 6E  ��6�   Z�  6�   �� *(&(Controls:ControlsHelper.CornerRadius)+$ OneWay=� �� *TemplatedParent+6E  �� 5}   :   $ DisabledVisualElement��6A   #���  6`    .  $ 0Q�6,  # z 68   6�   �� *(&(Controls:ControlsHelper.CornerRadius)+$ OneWay=� �� *TemplatedParent+68    5~   :   $ PART_ContentPresenter��6T   #��z��6�   #��z��6,  # z 6P  #��z��6�  #��z��6�   . # z 6  #��z��6Q   6s   �� *20(Controls:ControlsHelper.ContentCharacterCasing)+ �� *TemplatedParent+6Q  5   7    5�   6   P� 5�   :    �@J�IsMouseOver . . �5�   >   �� $ DisabledVisualElement��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ PART_ContentPresenter��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ 
Background��6X   	 �@J�Opacity$	
 0.3Q�6p   5�   ;   P� 5�   :    � �� . . �5�   >   �� $ DisabledVisualElement��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ PART_ContentPresenter��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ 
Background��6X   	 �@J�Opacity$	
 0.3Q�6p   5�   ;   P� 5�   :    �@J�IsKeyboardFocusWithin . . �5�   >   �� $ DisabledVisualElement��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ PART_ContentPresenter��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ 
Background��6X   	 �@J�Opacity$	
 0.3Q�6p   5�   ;   P� 5�   :    � {� .  . �5�   >   �� $ DisabledVisualElement��6X   	 �@J�Opacity$	
 0.7Q�6{   �� 5�   >   $ PART_ContentPresenter��6X   	 �@J�Opacity$	
 0.3Q�6{   5�   ;   5�   3   5�   +   5�   '   5�      ��5�      �� 5�      $H B/NewOptics;component/Administrateur/Images/1456624077_add_user.ico��6)   5�      �� 5�      -   5        $ btnEdit��5�      $��20,0,0,0q�6D   $�� ��6V   #��� ��6a   $ Modifier��6�   $��20��6�   $��20��6�   $��25��6�   $��25��6�   $��Center=�6�   ��5�      �� 5�      $I C/NewOptics;component/Administrateur/Images/1456624391_edit_user.ico��6)   ��5�      �� 5�      ��5�      �� 5�      " ��6$    5�   "   �� 5�   &   " ��6,   .�5�   *   �� 	 �@ ContentCharacterCasing$
 Upper=�6i   �� 5�   *   	 �@ CornerRadius$
 3��6_   �� 5�   *   	 � ��$
 25��6F   �� 5�   *   	 � ��$

 Bold"�6G   �� 5�   *   	 � ��#
 �   6E   �� 5�   *   	 � ��#
 �  6G   �� 5�   *   	 � ��#
 �  6G   �� 5�   *   	 � ��#
 �  6H   �� 5�   *   	 � ��#
 �  6G   �� 5�   *   	 � ��$	
 5,6q�6D   �� 5�   *   	 � ��$
 1q�6L   �� 5�   *   	 � ��
 5�   .   �� 5�   2   " ��6B   .R�5�   6   � .+�5�   :   �� $ 
Background��6A   #��z��6U    6   �� *&$(Custom:ControlsHelper.CornerRadius)+$ OneWay=� �� *TemplatedParent+�� 5�   :   $ Border��6A   #��z��6Q   #��z��6}   # z 6C  ��6�   Z�  6�   �� *&$(Custom:ControlsHelper.CornerRadius)+$ OneWay=� �� *TemplatedParent+6C  �� 5�   :   $ DisabledVisualElement��6A   #���  6`    .  $ 0Q�6*  # z 66   6�   �� *&$(Custom:ControlsHelper.CornerRadius)+$ OneWay=� �� *TemplatedParent+66    5�   :   $ PART_ContentPresenter��6R   #��z��6�   #��z��6(  # z 6L  #��z��6�  #��z��6�   . # z 6  #��z��6M   6q   �� *0.(Custom:ControlsHelper.ContentCharacterCasing)+ �� *TemplatedParent+6M  5�   7    5�   6   P� 5�   :    �@J�IsMouseOver . . �5�   >   �� $ DisabledVisualElement��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ PART_ContentPresenter��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ 
Background��6X   	 �@J�Opacity$	
 0.3Q�6p   5�   ;   P� 5�   :    � �� . . �5�   >   �� $ DisabledVisualElement��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ PART_ContentPresenter��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ 
Background��6X   	 �@J�Opacity$	
 0.3Q�6p   5�   ;   P� 5�   :    �@J�IsKeyboardFocusWithin . . �5�   >   �� $ DisabledVisualElement��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ PART_ContentPresenter��6X   	 �@J�Opacity$	
 0.3Q�6{   �� 5�   >   $ 
Background��6X   	 �@J�Opacity$	
 0.3Q�6p   5�   ;   P� 5�   :    � {� .  . �5�   >   �� $ DisabledVisualElement��6X   	 �@J�Opacity$	
 0.7Q�6{   �� 5�   >   $ PART_ContentPresenter��6X   	 �@J�Opacity$	
 0.3Q�6{   5�   ;   5�   3   5�   +   5�   '   5�      5�      5�      5�      5�      &   System.Windows.Controls.DataGrid  5�   
   -   5          Name$ PatientDataGrid��5�          AutoGenerateColumns  .  !   
IsReadOnly! . "   SelectionMode$" Single=�6Z   $��1��6r   $��0q�6�   $��Tahoma)�6�   #   AlternatingRowBackground## � ��6'  $   RowBackground#$ � ��6|  
���������6   �� %   ColumnHeaderStyle% 5�      �� 5�      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5�      �� 	 �@ ContentCharacterCasing$
 Lower=�6U   �� 5�      	 � ��$
 SemiBold"�63   �� 5�      	 �@J�SnapsToDevicePixels
 . 6<   �� 5�      	 � ��$
 0��61   �� 5�      	 � ��$
 25��62   �� 5�      	 � ��  
BlackBrush#
 �  63   �� 5�      	 �@�Cursor$

 Hand��6/   �� 5�      	 � ��$

 Left=�6C   �� 5�      	 � ��$
 Center=�6A   �� 5�      	 � ��$
 10,4,4,7q�60   �� 5�      	 � ��$
 0q�6/   �� 5�      	 � ��$
 0,0,0,3q�68   �� 5�      	 � ��  
GrayBrush5#
 �  64   �� 5�      	 � ��  
GrayBrush8#
 �  63   �� 5�      	 � ��
 5�      �� 5�      "  6.   .R�5�   "   � & � ColumnDefinitions& 5�   &   �� 5�   *   $��* �6;   �� 5�   *   $
��Auto �6;   .+�5�   &   �� $ BackgroundBorder��6-   #��z��6G   #��z��6s   #��z��6�   $��2��6�     5�   &   $ HeaderContent��6>   #��z��6�   #��z��6  # z 60  #��z��6l  #��z��6�   . # z 6�  #��z��61   6U   �� *0.(Custom:ControlsHelper.ContentCharacterCasing)+ �� *TemplatedParent+61  F� 5�   &   ' F�Name$' 	SortArrow��6+   $��1��6>    	 
GrayBrush2#��� 	 6N   $��10��6r   $��0,0,8,2q�6~   ( J� RenderTransformOrigin$( 0.5,0.5&�6�   ) �� Stretch$
) Fill=�6�   $y�	Collapsed=�6�   $��Center=�6�   $��8��6�   n� 5�   &   * n�Name$* PART_LeftHeaderGripper��6,   
������ $
��Left=�6e   $
��Left=�6�   ��5   *   �� 5  .   " n�64   .�5  2   �� 	 � ��$
 8��6J   �� 5  2   	 � ��#
 �  6O   �� 5  2   	 �@�Cursor$
 SizeWE��6K   �� 5  2   	 � ��
 5  6   �� 5  :   " n�6J   .R�5  >   �� 
������ + �� Padding#+ z��6^   ._�5	  B   �� #��z��6L   #��z��6p   $��1��6�   5
  ?   5  ;   5  3   5  /   5  '   n� 5  &   $* PART_RightHeaderGripper��6,   $��1��6M   $��Right=�6]   $��Right=�6y   ��5  *   �� 5  .   " n�64   .�5  2   �� 	 � ��$
 8��6J   �� 5  2   	 � ��#
 �  6O   �� 5  2   	 �@�Cursor$
 SizeWE��6K   �� 5  2   	 � ��
 5  6   �� 5  :   " n�6J   .R�5  >   �� 
������ #+ z��6^   ._�5  B   �� #��z��6L   #��z��6p   $��1��6�   5  ?   5  ;   5  3   5   /   5"  '   5#  #    5$  "   P� 5%  &    6G   Z�  �@ SortDirection. �5&  *   �� $ BackgroundBorder��6G    
 
WhiteBrush#
 � 
 6e   	 � ��5'  '   ]� 5(  &   , ]� 
Conditions, 5)  *   �� 5*  .   - �� Property- �@J�IsMouseOver. �� Value. . 6O   �� 5+  .   . 6Q   Z� - �@ SortDirection./�5-  *   �� $ BackgroundBorder��6G   #
 �  6e   	 � ��5.  '   P� 5/  &    �@ SortDirection$ 	AscendingX�6G   . �50  *   �� $ 	SortArrow��6G   	 � y�$
 Visible=�6^   �� 51  *   $ 	SortArrow��6A   	 � ��a
 ����@�eрހ@�eрހ��� u��� upw�р�C��� 6npw�� 6n��р���р�@�e6X   52  '   P� 53  &    �@ SortDirection$ 
DescendingX�6G   . �54  *   �� $ 	SortArrow��6G   	 � y�$
 Visible=�6^   �� 55  *   $ 	SortArrow��6A   	 � ��a
 ���ހ��!р���!р�8��� 6n��� 6nX4�р�C��x� uX4�� u��рހ8��рހ��!6X   �� 56  *   $ 	SortArrow��6C   	 � ��$
 0,0,8,0q�6Z   57  '   P� 58  &    �@ DisplayIndex$ 0��6F   . �59  *   �� $ PART_LeftHeaderGripper��6G   	 � y�$
 	Collapsed=�6k   5:  '   P� 5;  &    � {� .  . �5<  *   �� #
 � ��6G   	 � ��5=  '   5?     5A     5B     /   	CellStyle/ 5D     �� 5E     *  $System.Windows.Controls.DataGridCell"  6   0 �� Triggers0 5F     P� 5G      �@ 
IsSelected . . �5H     �� 	 � ��$
 0,3,0,3q�6B   �� 5J     	 � ��#
 � ��6<   6�   �� 5K     	 � ��$

 Bold"�6;   6J   �� 5L     	 � ��$
 20(�69   6F   �� 5M     	 � ��

 ���� 6;   �� 5N     	 � ��
 �	#FB2D3F5D6O   5O     5Q     1   Columns1 5T     0  *System.Windows.Controls.DataGridTextColumn  5Y     ,  &System.Windows.Controls.DataGridColumn2   Header$2 Date Commande��6%   3   Width5	  /System.Windows.Controls.DataGridLengthConverter$
3 Auto	 5Z  @   4   MinWidth$4 50Q�6N   1
  +System.Windows.Controls.DataGridBoundColumn5 
  Binding5 6   �� *Date+6 �� StringFormat$6 {0:dd/MM/yyyy}��6N     5[     $2 Date Livraison��6%   $
3 Auto	 5\  E   $4 50Q�6T   5 6   �� *	Livraison+$6 {0:dd/MM/yyyy}��6T     5]     $2 Client��6%   7   
Visibility$7 	Collapsed=�5^  '   $
3 Auto	 6?   $4 50Q�6N   5 6   �� *RaisonClient+6N     5_     $
3 Auto	 6&   $2 Client��63   5 5`     `� 5a     $6 {0}  {1}��6'   .1�5b     �� 8 �� Path$8 IdClient�6&   �� 5c     $8 RaisonClient�6&   5d     5g       5l     $2 Montant Total��6%   $
3 Auto	 5m  '   $4 50Q�65   5 6   �� *MontantTotal+65     5n     $2 Remise��6%   $
3 Auto	 5o  !   $4 50Q�6/   5 6   �� *Remise+6/     5p     $2 	Encaissé��6%   $
3 Auto	 5q  #   $4 50Q�61   5 6   �� *	Encaissé+61     5r     $2 Reste��6%   $
3 Auto	 5s      $4 50Q�6.   5 6   �� *Reste+6.     5u     $2 
En Montage��6%   9   
IsReadOnly9 . $
3 Auto	 5v  $   $4 50Q�62   5 6   �� *	EnMontage+62     5w     $2 Montée��6%   9 . $
3 Auto	 5x      $4 50Q�6.   5 6   �� *Monte+6.     5y     $2 
Délivrée��6%   9 . $
3 Auto	 5z  "   $4 50Q�60   5 6   �� *
Délivre+60     5{     $2 N°Commande��6%   9 . $
3 Auto	 5|  $   $4 50Q�62   5 6   �� *	NCommande+62     5}     $2 UserName��6%   9 . $
3 Auto	 5~  #   $4 50Q�61   5 6   �� *
UserName+61   5�     ]� 5�  
   -	   5        : ]�Name$: Footer��5�     $��2��6$   .+�5�     �  5�     �� 5�     $��1* �6$   & 5�     �� 5�     .+�5�     �� -
   5        ; ��Name$; label��5�     $��Trouver Client :��6+   $
��Left=�6C   $��Center=�6^   $
��Bold"�6y   $��16(�6�   $��
10,12,0,10q�6�   $( 4.417,5.766&�6�   $��Tahoma)�6�   #��� ��6�   �� 5�     -   5        < ��Name$< txtRecherche��5�     $
��Left=�60   $��Center=�6K   $��	150,0,0,0q�6f   $	��150��6y   ���	#FF3434346�   �� 5�     -   5        = ��Name$= 	checkeffe��5�     $��Commande montée��6.   $
��Left=�6H   $��Center=�6c   $��	550,0,0,0q�6~   $��Tahoma)�6�   $
��Bold"�6�   �� 5�     -   5        $= checkRéceptionée��5�     $��Commande délivrée��65   $
��Left=�6Q   $��Center=�6l   $��	700,0,0,0q�6�   $
��Bold"�6�   $��Tahoma)�6�   �� 5�     -   5        $= checkEnMontage��5�     $��Commande En Montage��63   $
��Left=�6Q   $��Center=�6l   $��	350,0,0,0q�6�   $
��Bold"�6�   $��Tahoma)�6�   6�   5�     5�     5�     5�     