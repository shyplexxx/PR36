using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR32
{
    public static class SessionManager
    {
        public static string CurrentUserFullName { get; set; } = null;
        public static bool IsGuest { get; set; } = false;

        public static string ID { get; set; } = null;

        // Параметр бездействия пользователя в секундах (по умолчанию 30)
        public static int InactivityTimeoutSeconds { get; set; } = 30;

        // Событие для сброса таймера в главной форме
        public static event Action OnUserActivity;

        // Вызывать этот метод при любом клике или нажатии клавиши
        public static void RegisterActivity()
        {
            OnUserActivity?.Invoke();
        }

        public static void Logout()
        {
            CurrentUserFullName = null;
            IsGuest = false;
        }

    }
}
