using DDrop.BE.Models;
using DDrop.Utility.ImageOperations;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration
    {
        public static readonly DependencyProperty UserLoginProperty = DependencyProperty.Register("UserLogin", typeof(UserViewModel), typeof(Registration));

        public bool RegistrationSucceeded = false;
        public byte[] UserPhoto { get; set; }

        public UserViewModel UserLogin
        {
            get { return (UserViewModel)GetValue(UserLoginProperty); }
            set
            {
                SetValue(UserLoginProperty, value);
            }
        }

        public Registration()
        {
            InitializeComponent();
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox1.Visibility = Visibility.Visible;
            PasswordBox1Unmasked.Visibility = Visibility.Hidden;

            PasswordBoxConfirm.Visibility = Visibility.Visible;
            PasswordBoxConfirmUnmasked.Visibility = Visibility.Hidden;
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
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.AddExtension = true;
            if (openFileDialog.ShowDialog() == true)
            {
                UserPhoto = ImageInterpreter.FileToByteArray(openFileDialog.FileName);
                ProfilePicture.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void CancelRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            UserLogin = new UserViewModel()
            {
                Email = TextBoxEmail.Text,
                UserSeries = new ObservableCollection<SeriesViewModel>(),
                FirstName = TextBoxFirstName.Text,
                LastName = TextBoxLastName.Text,
                IsLoggedIn = true,
                Password = PasswordBox1.Password,
                UserId = new Guid(),
                UserPhoto = UserPhoto,
            };
            RegistrationSucceeded = true;
            Close();
        }
    }
}
