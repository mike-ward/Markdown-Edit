using System.IO;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public static class UserTemplate
    {
        public static string Load()
        {
            if (File.Exists(TemplateFile) == false)
            {
                var userTemplate = Resources.GithubTemplateHtml;
                Save(userTemplate);
            }
            return TemplateFile;
        }

        public static void Save(string template)
        {
            Directory.CreateDirectory(UserSettings.SettingsFolder);
            File.WriteAllText(TemplateFile, template);
        }

        public static string TemplateFile => Path.Combine(UserSettings.SettingsFolder, "user_template.html");
    }
}