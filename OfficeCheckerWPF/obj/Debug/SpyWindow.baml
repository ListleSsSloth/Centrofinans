   M S B A M L   `   `   `  ����L  HOfficeChecker, Version=5.0.0.18465, Culture=neutral, PublicKeyToken=null3clr-namespace:OfficeCheckerWPFOfficeCheckerWPF  5   	        OfficeCheckerWPF.SpyWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:OfficeCheckerWPF     5   	   -   5          � Title$  Настройка iSpy��5   	   $	��215��6    $	��350��6,    � 
ResizeMode$ NoResize=�69    � WindowStartupLocation$ CenterOwner=�6e   ��5	      �� 5
   
    �� EndPoint$ 0.5,1&�6    �� 
StartPoint$ 0.5,0&�6/   .��5      �  � Color$ 	#FFE864F5��6    � Offset$ 0Q�6-   � 5      $ 	#FF8FFBF6��6   $ 1Q�6-   � 5      $ 	#FF9EDFF5��6   $ 0.742Q�6-   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $%��Параметры камеры��6   $��0��6-   $��3q�6:   .��5      �� .+�5      �� -   5        $��CamModelBox��5      $��3q�6.    �� SelectedIndex$ 0��69   $��Center=�6K   �� 5      	 �� Orientation$	 
Horizontal=�6   $��Center=�66   $
��Auto��6S   .+�5      �� -   5        $��IpBox1��5      $��3q�6,   $	��3,2q�67   $	��192��6E   
 �� TextAlignment$
 Center=�6P    �� 	MaxLength$ 3��6g   $��35��6u   6�   �� 5      $��Bottom=�6    $��20(�6;   $��.��5!   *   5"      �� 5#      -   5        $��IpBox2��5#      $��3q�6,   $	��3,2q�67   $	��168��6E   $
 Center=�6P   $ 3��6g   $��35��6u   6�   �� 5$      $��Bottom=�6    $��20(�6;   $��.��5'   *   5(      �� 5)      -   5        $��IpBox3��5)      $��3q�6,   $	��3,2q�67   $	��180��6E   $
 Center=�6P   $ 3��6g   $��35��6u   6�   �� 5*      $��Bottom=�6    $��20(�6;   $��.��5-   *   5.      �� 5/      -   5        $��IpBox4��5/      $��3q�6,   $	��3,2q�67   $��3��6E   $
 Center=�6N   $ 3��6e   $��35��6s   6�   50      �� 52      -   5        $��CamLoginTextBox��52      $��3q�63   $	��5,2q�6>   $��admin��6L   $
 Center=�6Y   $ 20��6p   .��6   ��53      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 54   5   �� 6   6sys&clr-namespace:System;assembly=mscorlib     65    �� 
TargetType" ��6     �� 	Resources 55         CueBannerBrush j� 
AlignmentX j� 
AlignmentY j� Stretch !� Visual��%�   &	        !� 56   "   $ Center=�6E   $ Center=�6Y   $
 None=�6m    57   &   �� 58   *   $��
Логин��60   
�������6@   5:   #   5;       �� Triggers 5<      P� 5=   "    P� Property � �� �� Empty P� Value# Z 6:   . �5>   &   ��  �� Property � �� �� Value# [  6C   5?   #   P� 5@   "    � �� 6:   Z� . �5A   &   ��  � ��# [  6C   5B   #   P� 5C   "    �@J�IsKeyboardFocused . . �5D   &   ��  � ��
 �����6C   5E   #   5G      .��5H   %   5I      �� 5J      -   5        $��CamPasswordTextBox��5J      $��3q�64   $	��5,2q�6?   $�� ��6M   $
 Center=�6U   $ 20��6l   .��6{   ��5K      1&clr-namespace:System;assembly=mscorlibSystem 5L   5   �� 6   6sys&clr-namespace:System;assembly=mscorlib     65   " ��6     5M      ��%�   &	        !� 5N   "   $ Center=�6E   $ Center=�6Y   $
 None=�6m    5O   &   �� 5P   *   $��Пароль��60   
�������6A   5R   #   5S       5T      P� 5U   "    � ��# Z 6:   . �5V   &   ��  � ��# [  6C   5W   #   P� 5X   "    � �� 6:   Z� . �5Y   &   ��  � ��# [  6C   5Z   #   P� 5[   "    �@J�IsKeyboardFocused . . �5\   &   ��  � ��
 �����6C   5]   #   5_      .��5`   %   5a      5b      5c      �� 5e   
   -	   5        $��CameraAccessLabel��5e      $��1��6)   $��Center=�66   $��Center=�6Q   $��16(�6n   �� 5g   
   $��2��6   $��5,5,5,0q�6"   .+�5h      �� -
   5        $��StartISpyButton��5h      $��3q�6,   $��Запустить iSpy��67   $y�	Collapsed=�6P   6g   �� 5i      -   5        $��BaseTestButton��5i      $��3q�6+   $.��(Проверка установки iSpy��66   6X   �� 5j      -   5        $��LookSourceButon��5j      $��3q�6,   $��Текущий поток��67   6O   �� 5k      -   5        $��XmlSettingButton��5k      $��3q�6-   $#��Настроить поток��68   6R   �� 5l      -   5        $��BackupSettingButton��5l      $��3q�60   $��Backup настроек��6;   6U   �� 5m      -   5        $ ��BackupRestoreSettingButton��5m      $��3q�67   $3��-Восстановление настроек��6B   6d   5n      � 5p   
   -   5        $��RunningProgressBar��5p   #   $��3��6   $��3��6=    � IsIndeterminate . $	��8,1q�6_   $y�Hidden=�6l   5q      5r      