using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using DDrop.BE.Models;
using DDrop.Utility.ImageOperations;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DDrop.DAL;
using ToastNotifications;
using ToastNotifications.Messages;

namespace DDrop
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register("User", typeof(UserViewModel), typeof(Account));
        private Notifier _notifier;
        private DDropContext _dDropContext;
        public UserViewModel User
        {
            get { return (UserViewModel)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public Account(UserViewModel user, Notifier notifier, DDropContext dDropContext)
        {
            InitializeComponent();
            _dDropContext = dDropContext;
            _notifier = notifier;
            User = user;

            ProfilePicture.Source = ImageInterpreter.LoadImage(User.UserPhoto);
        }

        private async void ChooseProfilePicture_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.AddExtension = true;
            if (openFileDialog.ShowDialog() == true)
            {
                Properties.Settings.Default.Reference = openFileDialog.FileName;
                CropPhoto croppingWindow = new CropPhoto();
                croppingWindow.Height = new BitmapImage(new Uri(openFileDialog.FileName)).Height;
                croppingWindow.Width = new BitmapImage(new Uri(openFileDialog.FileName)).Width;
                croppingWindow.Owner = this;
                croppingWindow.CroppingControl.SourceImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                croppingWindow.CroppingControl.SourceImage.Height = new BitmapImage(new Uri(openFileDialog.FileName)).Height;
                croppingWindow.CroppingControl.SourceImage.Width = new BitmapImage(new Uri(openFileDialog.FileName)).Width;

                if (SystemParameters.PrimaryScreenHeight < croppingWindow.CroppingControl.SourceImage.Height ||
                    croppingWindow.CroppingControl.SourceImage.Width < croppingWindow.CroppingControl.SourceImage.Width)
                {
                    _notifier.ShowInformation("Вертикальный или горизонтальный размер фотографии слишком велик.");
                }
                else
                {
                    if (croppingWindow.ShowDialog() == true)
                    {
                        if (croppingWindow.UserPhotoForCropp != null)
                        {
                            using(_dDropContext = new DDropContext())
                            {
                                var user = await _dDropContext.Users.FirstOrDefaultAsync(x => x.UserId == User.UserId);
                                if (user != null)
                                {
                                    user.UserPhoto = croppingWindow.UserPhotoForCropp;
                                    User.UserPhoto = croppingWindow.UserPhotoForCropp;

                                    await _dDropContext.SaveChangesAsync();
                                    _notifier.ShowSuccess("Фотография обновлена.");
                                }
                                else
                                {
                                    _notifier.ShowError("Не удалось сохранить изменения. Проверьте интернет соединение.");
                                }
                            }                            
                        }                            
                    }
                    ProfilePicture.Source = ImageInterpreter.LoadImage(User.UserPhoto);
                }
            }
        }

        private void ChangeFirstNameButton_OnClick(object sender, RoutedEventArgs e)
        {
            TextBlockFirstnameValue.IsEnabled = true;
            TextBlockFirstnameValue.Focus();
            ChangeFirstNameButton.Visibility = Visibility.Hidden;
            SaveChangeFirstNameButton.Visibility = Visibility.Visible;           
        }

        private async void SaveChangeFirstNameButton_Click(object sender, RoutedEventArgs e)
        {
            using (_dDropContext = new DDropContext())
            {
                var newFirstName = TextBlockFirstnameValue.Text;
                var user = await _dDropContext.Users.FirstOrDefaultAsync(x => x.UserId == User.UserId);
                if (user != null)
                {
                    user.FirstName = newFirstName;
                    await _dDropContext.SaveChangesAsync();
                }
            }

            TextBlockFirstnameValue.IsEnabled = false;
            ChangeFirstNameButton.Visibility = Visibility.Visible;
            SaveChangeFirstNameButton.Visibility = Visibility.Hidden;
        }

        private void ChangeLastNameButton_OnClick(object sender, RoutedEventArgs e)
        {
            TextBlockLastNameValue.IsEnabled = true;
            TextBlockLastNameValue.Focus();
            ChangeLastNameButton.Visibility = Visibility.Hidden;
            SaveChangeLastNameButton.Visibility = Visibility.Visible;
        }

        private async void SaveChangeLastNameButton_Click(object sender, RoutedEventArgs e)
        {
            using (_dDropContext = new DDropContext())
            {
                var newLastName = TextBlockLastNameValue.Text;
                var user = await _dDropContext.Users.FirstOrDefaultAsync(x => x.UserId == User.UserId);
                if (user != null)
                {
                    user.LastName = newLastName;
                    await _dDropContext.SaveChangesAsync();
                }
            }

            TextBlockLastNameValue.IsEnabled = false;
            ChangeLastNameButton.Visibility = Visibility.Visible;
            SaveChangeLastNameButton.Visibility = Visibility.Hidden;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            CurrentPassword.Visibility = Visibility.Visible;
            CurrentPasswordUnmasked.Visibility = Visibility.Hidden;
            CurrentPassword.Password = CurrentPasswordUnmasked.Text;

            NewPassword.Visibility = Visibility.Visible;
            NewPasswordUnmasked.Visibility = Visibility.Hidden;
            NewPassword.Password = NewPasswordUnmasked.Text;

            NewPasswordConfirm.Visibility = Visibility.Visible;
            NewPasswordConfirmUnmasked.Visibility = Visibility.Hidden;
            NewPasswordConfirm.Password = NewPasswordConfirmUnmasked.Text;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            CurrentPassword.Visibility = Visibility.Hidden;
            CurrentPasswordUnmasked.Visibility = Visibility.Visible;
            CurrentPasswordUnmasked.Text = CurrentPassword.Password;

            NewPassword.Visibility = Visibility.Hidden;
            NewPasswordUnmasked.Visibility = Visibility.Visible;
            NewPasswordUnmasked.Text = NewPassword.Password;

            NewPasswordConfirm.Visibility = Visibility.Hidden;
            NewPasswordConfirmUnmasked.Visibility = Visibility.Visible;
            NewPasswordConfirmUnmasked.Text = NewPasswordConfirm.Password;
        }

        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[] { start, length });
        }

        private void CurrentPassword_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void NewPassword_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void NewPasswordConfirm_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void CurrentPasswordUnmasked_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void NewPasswordUnmasked_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void NewPasswordConfirmUnmasked_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void NewPasswordConfirm_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewPasswordConfirm.Password.Trim()))
            {
                NewPasswordConfirmUnmasked.Text = NewPasswordConfirm.Password;
                SetSelection(NewPasswordConfirm, NewPasswordConfirm.Password.Length, NewPasswordConfirm.Password.Length);
            }
        }

        private void NewPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewPassword.Password.Trim()))
            {
                NewPasswordUnmasked.Text = NewPassword.Password;
                SetSelection(NewPassword, NewPassword.Password.Length, NewPassword.Password.Length);
            }
        }

        private void CurrentPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword.Password.Trim()))
            {
                CurrentPasswordUnmasked.Text = CurrentPassword.Password;
                SetSelection(CurrentPassword, CurrentPassword.Password.Length, CurrentPassword.Password.Length);
            }
        }

        private void NewPasswordConfirmUnmasked_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBoxNewPasswordConfirm = sender as TextBox;
            NewPasswordConfirm.Password = textBoxNewPasswordConfirm?.Text != null ? textBoxNewPasswordConfirm.Text : "";
        }

        private void NewPasswordUnmasked_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBoxNewPassword = sender as TextBox;
            NewPassword.Password = textBoxNewPassword?.Text != null ? textBoxNewPassword.Text : "";
        }

        private void CurrentPasswordUnmasked_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBoxCurrentPassword = sender as TextBox;
            CurrentPassword.Password = textBoxCurrentPassword?.Text != null ? textBoxCurrentPassword.Text : "";
        }

        private async void ChangePasswordButton_OnClick(object sender, RoutedEventArgs e)
        {
            using (_dDropContext = new DDropContext())
            {
                var user = await _dDropContext.Users.FirstOrDefaultAsync(x => x.UserId == User.UserId);

                if (user != null)
                {
                    if (user.Password == CurrentPassword.Password)
                    {
                        if (NewPassword.Password == NewPasswordConfirm.Password &&
                            !string.IsNullOrWhiteSpace(NewPassword.Password))
                        {
                            user.Password = NewPassword.Password;
                            await _dDropContext.SaveChangesAsync();
                        }
                        else
                        {
                            _notifier.ShowError("Новый пароль и его подтверждение не совпадают.");
                        }
                    }
                    else
                    {
                        _notifier.ShowError("Неверный старый пароль.");
                    }
                }
                else
                {
                    _notifier.ShowError("Не удалось сохранить изменения. Проверьте интернет соединение.");
                }
            }
        }
    }
}
