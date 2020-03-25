using DDrop.BE.Models;
using DDrop.Utility.ImageOperations;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DDrop.DAL;
using ToastNotifications;
using ToastNotifications.Messages;
using DDrop.Utility.Cryptography;
using DDrop.Utility.Mappers;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration
    {
        public static readonly DependencyProperty UserLoginProperty = DependencyProperty.Register("UserLogin", typeof(User), typeof(Registration));
        private IDDropRepository _dDropRepository;
        private readonly Notifier _notifier;

        public bool RegistrationSucceeded;
        public byte[] UserPhoto { get; set; }

        public User UserLogin
        {
            get { return (User)GetValue(UserLoginProperty); }
            set
            {
                SetValue(UserLoginProperty, value);
            }
        }

        public Registration(IDDropRepository dDropRepository, Notifier notifier)
        {
            _notifier = notifier;
            _dDropRepository = dDropRepository;
            InitializeComponent();
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox1.Visibility = Visibility.Visible;
            PasswordBox1Unmasked.Visibility = Visibility.Hidden;
            PasswordBox1.Password = PasswordBox1Unmasked.Text;

            PasswordBoxConfirm.Visibility = Visibility.Visible;
            PasswordBoxConfirmUnmasked.Visibility = Visibility.Hidden;
            PasswordBoxConfirm.Password = PasswordBoxConfirmUnmasked.Text;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            PasswordBox1.Visibility = Visibility.Hidden;
            PasswordBox1Unmasked.Visibility = Visibility.Visible;
            PasswordBox1Unmasked.Text = PasswordBox1.Password;

            PasswordBoxConfirm.Visibility = Visibility.Hidden;
            PasswordBoxConfirmUnmasked.Visibility = Visibility.Visible;
            PasswordBoxConfirmUnmasked.Text = PasswordBoxConfirm.Password;
        }

        private void ChooseProfilePicture_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*",
                Multiselect = false,
                AddExtension = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CropPhoto croppingWindow = new CropPhoto
                {
                    Height = new BitmapImage(new Uri(openFileDialog.FileName)).Height,
                    Width = new BitmapImage(new Uri(openFileDialog.FileName)).Width,
                    Owner = this
                };
                croppingWindow.CroppingControl.SourceImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                croppingWindow.CroppingControl.SourceImage.Height = new BitmapImage(new Uri(openFileDialog.FileName)).Height;
                croppingWindow.CroppingControl.SourceImage.Width = new BitmapImage(new Uri(openFileDialog.FileName)).Width;

                if (SystemParameters.PrimaryScreenHeight < croppingWindow.CroppingControl.SourceImage.Height ||
                    SystemParameters.PrimaryScreenWidth < croppingWindow.CroppingControl.SourceImage.Width)
                {
                    _notifier.ShowInformation("Вертикальный или горизонтальный размер фотографии слишком велик.");
                }
                else
                {
                    if (croppingWindow.ShowDialog() == true)
                    {
                        if (croppingWindow.UserPhotoForCropp != null)
                            UserPhoto = croppingWindow.UserPhotoForCropp;
                    }
                    ProfilePicture.Source = ImageInterpreter.LoadImage(UserPhoto);
                }
            }
        }

        private void CancelRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            if (TextBoxEmail.Text.Length == 0)
            {
                ErrorMessage.Text = "Введите электронную почту.";
                TextBoxEmail.Focus();
            }
            else if (!Regex.IsMatch(TextBoxEmail.Text,
                @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                ErrorMessage.Text = "Электронная почта имеет не верный формат.";
                TextBoxEmail.Select(0, TextBoxEmail.Text.Length);
                TextBoxEmail.Focus();
            }
            else if (string.IsNullOrWhiteSpace(PasswordBox1.Password))
            {
                ErrorMessage.Text = "Введите пароль.";
            }
            else if (PasswordBox1.Password != PasswordBoxConfirm.Password)
            {
                ErrorMessage.Text = "Пароли не совпадают.";
            }
            else
            {
                var emailToCheck = TextBoxEmail.Text;
                RegistrationWindowLoading();
                var userToRegister = await Task.Run(() => _dDropRepository.GetUserByLogin(emailToCheck));
                if (userToRegister == null)
                {
                    try
                    {
                        var user = new User()
                        {
                            Email = TextBoxEmail.Text.Trim(),
                            UserSeries = new ObservableCollection<Series>(),
                            FirstName = TextBoxFirstName.Text,
                            LastName = TextBoxLastName.Text,
                            IsLoggedIn = true,
                            Password = PasswordOperations.HashPassword(PasswordBox1.Password),
                            UserId = Guid.NewGuid(),
                            UserPhoto = UserPhoto,
                        };

                        await Task.Run(() => _dDropRepository.CreateUserAsync(DDropDbEntitiesMapper.UserToDbUser(user)));

                        UserLogin = user;

                        _notifier.ShowSuccess($"Пользователь {UserLogin.Email} успешно зарегистрирован.");
                        RegistrationSucceeded = true;
                        RegistrationWindowLoading();
                        Close();
                    }
                    catch
                    {
                        RegistrationWindowLoading();
                        _notifier.ShowError("Не удалось установить соединение. Проверьте интернет подключение.");
                    }
                }
                else
                {
                    RegistrationWindowLoading();
                    _notifier.ShowError("Пользователь с таким Email уже существует.");
                    RegistrationSucceeded = false;
                }
            }
        }

        private void PasswordBox1_OnPasswordChangedPasswordBox1_OnTextInput(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox1)
            {
                if (string.IsNullOrWhiteSpace(passwordBox1.Password.Trim()))
                {
                    PasswordBox1Unmasked.Text = passwordBox1.Password;
                    SetSelection(passwordBox1, passwordBox1.Password.Length, passwordBox1.Password.Length);
                }
            }
        }

        private void PasswordBox1Unmasked_OnTextChangedswordBox1Unmasked_OnTextInput(object sender, TextChangedEventArgs e)
        {
            var textBox1 = sender as TextBox;
            PasswordBox1.Password = textBox1?.Text != null ? textBox1.Text : "";
        }

        private void PasswordBoxConfirm_OnPasswordChangedsswordBoxConfirm_OnTextInput(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBoxConfirm)
            {
                PasswordBoxConfirmUnmasked.Text = passwordBoxConfirm.Password;
                SetSelection(passwordBoxConfirm, passwordBoxConfirm.Password.Length,
                    passwordBoxConfirm.Password.Length);
            }
        }

        private void PasswordBoxConfirmUnmasked_OnTextChangedPasswordBoxConfirmUnmasked_OnTextInput(object sender, TextChangedEventArgs e)
        {
            var textBoxConfirm = sender as TextBox;
            PasswordBoxConfirm.Password = textBoxConfirm?.Text != null ? textBoxConfirm.Text : "";
        }

        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[] { start, length });
        }

        private void PasswordBox1_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void TextBoxEmail_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void PasswordBox1Unmasked_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void PasswordBoxConfirm_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void PasswordBoxConfirmUnmasked_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void RegistrationWindowLoading()
        {
            CancelRegistrationButton.IsEnabled = !CancelRegistrationButton.IsEnabled;
            Submit.IsEnabled = !Submit.IsEnabled;
            ChooseProfilePicture.IsEnabled = !ChooseProfilePicture.IsEnabled;
            RegistrationLoading.IsAdornerVisible = !RegistrationLoading.IsAdornerVisible;
        }
    }
}
