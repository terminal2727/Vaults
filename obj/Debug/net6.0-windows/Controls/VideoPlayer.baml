   M S B A M L   `   `   `  ����C  ?VaultsII, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:VaultsII.ControlsVaultsII.Controls  5      #    VaultsII.Controls.VideoPlayer   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:VaultsII.Controls     5      .��5	      � -   5          �Name$  Exterior��5	       � RowDefinitions 5
   
   �� 5      $	��95* �6   �� 5      $��5* �6   .+�5   
   � -   5        $  	VideoBody��5      $��0��5      5      �� 5   
   -   5         ��Name$ 
PlayButton��5      $��1��5      $��20��5      $��20��5      
������ 
������ $
��Left=�5      $��Center=�5      $��
5, 0, 0, 0q�5      .��5      �� $��..\Icons\PlayIcon.png��6    �� BitmapScalingMode$ HighQuality=�5      5       �� 5"   
   -   5         ��Name$ VideoTimeline��5#      $��1��5"      $��30, 0, 30, 0q�5&      $��Stretch=�5'      $��Center=�5(       � Minimum$ 0Q�5)       � Maximum$	 100Q�5*       � SmallChange$	 0.1Q�5+       � LargeChange$ 1Q�5,      �� 5.   
   -   5        $ 
MuteButton��5/      $��1��5.      $��20��51      $��20��52      
������ 
������ $��Right=�55      $��Center=�56      $��
0, 0, 5, 0q�57      .��59      �� $��..\Icons\AudioIcon.png��6   $ HighQuality=�5:      5<      5=      5>      