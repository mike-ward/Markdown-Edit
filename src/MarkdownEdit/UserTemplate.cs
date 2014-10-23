using System.IO;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public class UserTemplate
    {
        public string Template { get; set; }

        public UserTemplate()
        {
            Template = string.Empty;
        }

        public static UserTemplate Load()
        {
            if (File.Exists(TemplateFile)) return new UserTemplate {Template = File.ReadAllText(TemplateFile)};
            var userTemplate = new UserTemplate {Template = Resources.GithubTemplateHtml};
            userTemplate.Save();
            return Load();
        }

        public void Save()
        {
            Directory.CreateDirectory(UserSettings.SettingsFolder);
            File.WriteAllText(TemplateFile, Template);
        }

        public static string TemplateFile
        {
            get { return Path.Combine(UserSettings.SettingsFolder, "user_template.html"); }
        }
    }
}