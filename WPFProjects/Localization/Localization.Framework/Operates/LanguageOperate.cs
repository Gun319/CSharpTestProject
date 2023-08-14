using System;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace Localization.Framework.Operates
{
    /// <summary>
    /// 语言操作
    /// </summary>
    public class LanguageOperate
    {
        private const string KEY_OF_LANGUAGE = "language";

        /// <summary>
        /// 语言本地化
        /// </summary>
        /// <param name="language">语言环境</param>
        public static void SetLanguage(string language = "")
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                language = ConfigurationManager.AppSettings[KEY_OF_LANGUAGE];

                if (string.IsNullOrWhiteSpace(language))
                    language = System.Globalization.CultureInfo.CurrentCulture.ToString();
            }

            string languagePath = $@"I18nResources\{language}.xaml";

            var lanRd = Application.LoadComponent(new Uri(languagePath, UriKind.RelativeOrAbsolute)) as ResourceDictionary;
            var old = Application.Current.Resources.MergedDictionaries.FirstOrDefault(o => o.Contains("WindowsTitle"));

            if (old != null)
                Application.Current.Resources.MergedDictionaries.Remove(old);

            Application.Current.Resources.MergedDictionaries.Add(lanRd);

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[KEY_OF_LANGUAGE].Value = language;
            configuration.Save();
            ConfigurationManager.RefreshSection("AppSettings");

            var culture = new System.Globalization.CultureInfo(language);
            System.Globalization.CultureInfo.CurrentCulture = culture;
            System.Globalization.CultureInfo.CurrentUICulture = culture;
        }
    }
}
