   M S B A M L   `   `   `  ����J  FWiresharkParser, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:WiresharkParserWiresharkParser  5   	   "    WiresharkParser.WindowVsData   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   /localclr-namespace:WiresharkParser     5   	     � Title$  WindowVsData��5   	   $	��304��6   $	��728��6+   .��5      �  � RowDefinitions 5   
   �� 5      $
��230* �6   �� 5	      $��50 �6   .+�5   
   �� -   5        $��
BackButton��5      $��
Назад��6#   $��
0,10,10,10q�63   $��1��6G   $��Right=�6m   $	��102��6�   &   System.Windows.Controls.DataGrid  5   
   -   5        $��
ViewOfData��5   /      AutoGenerateColumns .  $��10q�6A    �� IsTextSearchEnabled .    Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$
 Time��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 63   �� *
dateTime+  5      $ Source��6%    65   �� *source+  5      $
 Port��6%    63   �� *
sourcePort+  5      $ Destination��6%    6:   �� *destination+  5      $
 Port��6%    63   �� *destinationPort+  5      $ Protocol��6%    67   �� *
protocol+4  .System.Windows.Controls.DataGridTemplateColumn  5      $
 Data��6)      CellTemplate 5      �� 5      .R�5      �� $��WrapWithOverflow=�6)   $	��800��6K   ��6Z   �� *data+5      5        5      $ Information��6)    5      �� 5      .R�5      �� $��WrapWithOverflow=�6(   $	��680��6J   ��6Z   �� *information+5      5!      5)      5*      5+      