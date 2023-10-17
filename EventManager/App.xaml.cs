using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using EventManager.View;
using GalaSoft.MvvmLight.Ioc;
using EventManager.Model;
using EventManager.Model.VK;
using GalaSoft.MvvmLight.Views;
using EventManager.DataAccess;
using SQLitePCL;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

// Документацию по шаблону "Пустое приложение" см. по адресу http://go.microsoft.com/fwlink/?LinkId=391641

namespace EventManager
{
    /// <summary>
    /// Обеспечивает зависящее от конкретного приложения поведение, дополняющее класс Application по умолчанию.
    /// </summary>
    public sealed partial class App : Application
    {
        private const string dbFileName = "EventManager.sqlite";
        private TransitionCollection transitions;
        public static EventRepository repo;
        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, dbFileName));//DataBase Name 
        private ISocialProvider socialProvider;

        public static ContinuationManager ContinuationManager { get; private set; }

        /// <summary>
        /// Инициализирует одноэлементный объект приложения.  Это первая выполняемая строка разрабатываемого
        /// кода; поэтому она является логическим эквивалентом main() или WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            ContinuationManager = new ContinuationManager();

            this.InitializeComponent();
            this.Suspending += this.OnSuspending;


            repo = new EventRepository(DB_PATH);

            repo.CreateTable();


            var tokenValue = repo.GetTokenValue();
            var groupId = repo.GetGroupIdValue();
            var signSuffix = repo.GetSignSuffixValue();
            int? intGroupId = string.IsNullOrEmpty(groupId) ? null : (int?)int.Parse(groupId);
            socialProvider = new SocialProviderWithCache(new VKSocialProvider(repo.GetLatestNewsDate(), tokenValue));
            socialProvider.OnAccessTokenGetting += SocialProvider_OnAccessTokenGetting;
            repo.AddNewsProvider(socialProvider);
            SimpleIoc.Default.Register<ISocialProvider>(() => socialProvider);
            SimpleIoc.Default.Register<IAuthicatiable>(() => socialProvider);
        }

        private void SocialProvider_OnAccessTokenGetting(string accessToken)
        {
            repo.SaveAccessToken(accessToken);
        }

        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Вызывается при обычном запуске приложения пользователем.  Будут использоваться другие точки входа,
        /// если приложение запускается для открытия конкретного файла, отображения
        /// результатов поиска и т. д.
        /// </summary>
        /// <param name="e">Сведения о запросе и обработке запуска.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Не повторяйте инициализацию приложения, если в окне уже имеется содержимое,
            // только обеспечьте активность окна
            if (rootFrame == null)
            {
                // Создание фрейма, который станет контекстом навигации, и переход к первой странице
                rootFrame = new Frame();

                // TODO: Измените это значение на размер кэша, подходящий для вашего приложения
                rootFrame.CacheSize = 1;

                // Задайте язык по умолчанию
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Загрузить состояние из ранее приостановленного приложения
                }

                // Размещение фрейма в текущем окне
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Удаляет турникетную навигацию для запуска.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                SimpleIoc.Default.Register<INavigationService, NavigationService>();
                (SimpleIoc.Default.GetInstance<INavigationService>() as NavigationService).Configure("/View/EventVisitors.xaml", typeof(EventVisitors));
                (SimpleIoc.Default.GetInstance<INavigationService>() as NavigationService).Configure("/View/MainView.xaml", typeof(MainView));
                (SimpleIoc.Default.GetInstance<INavigationService>() as NavigationService).Configure("/View/SettingsView.xaml", typeof(SettingsView));

                Type type = null;



                if (socialProvider.IsLoginCompleted)
                {
                    if (string.IsNullOrEmpty(repo.GetSignSuffixValue()) || string.IsNullOrEmpty(repo.GetGroupIdValue()))
                        type = typeof(SettingsView);
                    else
                        type = typeof(MainView);
                }
                else
                {
                    type = typeof(LoginView);
                }

                // Если стек навигации не восстанавливается для перехода к первой странице,
                // настройка новой страницы путем передачи необходимой информации в качестве параметра
                // навигации
                if (!rootFrame.Navigate(type, e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }

            }

            // Обеспечение активности текущего окна
            Window.Current.Activate();
        }

        /// <summary>
        /// Восстанавливает переходы содержимого после запуска приложения.
        /// </summary>
        /// <param name="sender">Объект, где присоединен обработчик.</param>
        /// <param name="e">Сведения о событии перехода.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Вызывается при приостановке выполнения приложения.  Состояние приложения сохраняется
        /// без учета информации о том, будет ли оно завершено или возобновлено с неизменным
        /// содержимым памяти.
        /// </summary>
        /// <param name="sender">Источник запроса приостановки.</param>
        /// <param name="e">Сведения о запросе приостановки.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            ContinuationManager.MarkAsStale();
            // TODO: Сохранить состояние приложения и остановить все фоновые операции
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                var continuationEventArgs = args as IContinuationActivatedEventArgs;
                if (continuationEventArgs != null)
                {
                    ContinuationManager.Continue(continuationEventArgs);
                    ContinuationManager.MarkAsStale();
                }

            }
        }



    }
}