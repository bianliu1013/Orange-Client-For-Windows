   M S B A M L   `   `   `  ���� W  SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   MenuButtonTemplate  �� 
TargetType � 	Resources  Key �� TargetProperty �� 
TargetName8    2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime �� Value  InformationBtnTemplate ��Name �� Triggers P� Property	 P� Value
 �� 
TargetName �� Property �� Value  ConfigButtonControlTemplate & "DefaultRepeatButtonControlTemplate % !SingleRepeatButtonControlTemplate  RepeatButtonControlTemplate ! PreviousButtonControlTemplate  PauseButtonControlTemplate 	 PlayButtonControlTemplate 
 NextButtonControlTemplate   ShuffleButtonControlTemplate # NonShuffleButtonControlTemplate  VolButtonControlTemplate  MuteButtonControlTemplate  TopButtonControlTemplate  UpButtonControlTemplate  DownButtonControlTemplate  BottomButtonControlTemplate  SaveButtonControlTemplate  LoadButtonControlTemplate  DelButtonControlTemplate�� D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml      %�  &	        &	 �    &	 �    &	 >    &	 �    &	 �    &	 _    &	     &		 �    &	
 m    &	 	    &	 �	    &	 �
    &	 )    &	 �    &	 �    &	 2    &	 �    &	 �    &	 M    &	 �    �� "  �� ���� over_menubtn .���� $* $(Shape.Fill).(SolidColorBrush.Color)�$ 	rectangle��.g�   $ 0:0:0.1��$ Black���� $* $(Shape.Fill).(SolidColorBrush.Color)�$ 
rectangle1��.g�   $ 0:0:0.2��$ Black���� $* $(Shape.Fill).(SolidColorBrush.Color)�$ 
rectangle2��.g�   $ 0:0:0.3��$ Black���� leave_menubtn .���� $* $(Shape.Fill).(SolidColorBrush.Color)�$ 	rectangle��.g�   $ 0��$ Black��   $ 0:0:0.3��$ 	#FFEF9C00���� $* $(Shape.Fill).(SolidColorBrush.Color)�$ 
rectangle1��.g�   $ 0��$ Black��   $ 0:0:0.2��$ 	#FFEF9C00���� $* $(Shape.Fill).(SolidColorBrush.Color)�$ 
rectangle2��.g�   $ 0��$ Black��   $ 0:0:0.1��$ 	#FFEF9C00���� "  ��.R�� .+��� $ image��$��1q�$��../Images/information.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��1q�$��../Images/config.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$%��../Images/player_repeat_btn.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$,��&../Images/player_single_repeat_btn.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$,��&../Images/player_active_repeat_btn.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$#��../Images/player_back_btn.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$$��../Images/player_pause_btn.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$#��../Images/player_play_btn.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$#��../Images/player_next_btn.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$&�� ../Images/player_shuffle_btn.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$)��#../Images/player_nonshuffle_btn.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$��../Images/volume.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$��0q�$��../Images/mute.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$1��+../Images/player_small_playlist_endofup.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$,��&../Images/player_small_playlist_up.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$.��(../Images/player_small_playlist_down.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$3��-../Images/player_small_playlist_endofdown.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$.��(../Images/player_small_playlist_save.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$.��(../Images/player_small_playlist_open.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q��� "  ��.R�� .+��� $ image��$-��'../Images/player_small_playlist_del.png��$
��Fill=� P�  �@J�IsMouseOver	 .  . ��� $
 image�� �@J�Opacity$	 0.5Q�