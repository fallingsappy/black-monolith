using System.Windows.Controls;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using MailSender.Service;
using MailSender.Services;
using MailSender.ViewModel;

namespace MailSendWPFF.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<IDataAccessService, DataBaseAccessService>();
        }

        public MainWindowViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainWindowViewModel>();
            }
        }
    }
}