   M S B A M L   `   `   `  ����C  ?VaultsII, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:VaultsIIVaultsII  5   	   /clr-namespace:VaultsII.ViewsVaultsII.Views  5   	       VaultsII.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:VaultsII     5   	   .viewsclr-namespace:VaultsII.Views     5   	   -   5          � Title$  
MainWindow��5
   	   $	��450��5   	   $	��800��5   	    � WindowStyle$
 None=�5   	    � WindowState$ 	Maximized=�5   	    � DataContext 5          VaultsII.Views.ViewControl  5   
   .��5      � ���#353943 � RowDefinitions 5   
   �� 5      $��17 �6   �� 5      $��40 �6   �� 5      �� 5      $��15 �6   .+�5   
   �� -   5        $��0��5      ���#474f635      �� 5!   
    �� Orientation$ 
Horizontal=�6   $��Right=�5"      .+�5$      �� -   5         ��Name$ Minimize��5$      $��_��5&         BackgroundButtons#��[  5'      �� 5)      -   5        $ Maximize��5)      $	��⧠��5+      #��[  5,      �� 5.      -   5        $ Close��5.      $��X��50      #��[  51      53      �� 55   
   $��0��6   $��1��56      $��Bottom=�57        DividingLine#��[ 58      �� 5:   
   $��1��6   $��1��5;      $��Bottom=�5<      #��[ 5=      � 5?   
   $��1��6    � ColumnDefinitions 5@      �� 5A      �� 5B      .+�5E      �� $��0��6   $��5q�5F      $
��Left=�5G      $ 
Horizontal=�5H      .+�5J      �� -   5        $
 Back��5K      $��1��5J      $��15(�5M      $��Center=�5N      $��Center=�5O      $��50��5P      #��[  5Q      .��5S      �� $F��@C:\Users\commi\source\repos\VaultsII\VaultsII\Icons\BackIcon.png��6     �� BitmapScalingMode$ NearestNeighbor=�5T      5V      5X      �� 5Z      $��1��6   $��5q�5[      $��Right=�5\      $ 
Horizontal=�5]      .+�5_      �� -   5        $
 Home��5`      $��1��5_      $��15(�5b      $��Center=�5c      $��Center=�5d      $��50��5e      #��[  5f      .��5h      �� $F��@C:\Users\commi\source\repos\VaultsII\VaultsII\Icons\HomeIcon.png��6   $ NearestNeighbor=�5i      5k      5m      5o      �� 5q   
   $��2��6   $��1��5r      $��Bottom=�5s      #��[ 5t      � 5v   
   $��2��6    5w      �� 5x      $	��10* �6#   �� 5y      $	��90* �6#   .+�5|      �� $��0��6   $��1��5}      $��Right=�5~      #��[ 5      �� 5�      $��0��6   $��Hidden=�5�      .��5�      �� -   5        	 ��Name$	 ViewButtonsColumn��5�      $ Vertical=�5�      5�      �� 5�      -	   5        
 ��Name$

 View��5�      $��1��5�      $��5q�5�      ��5�      �� *CurrentView+5�      5�      5�      