using System;
using System.IO;
using HtmlAgilityPack;
using MarkdownEdit.Properties;

namespace MarkdownEdit.Models
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

        public static string InsertContent(string content)
        {
            try
            {
                var template = new HtmlDocument();
                template.Load(TemplateFile);
                var contenthtml = new HtmlDocument();
                contenthtml.LoadHtml(content);

                template.GetElementbyId("content").AppendChildren(contenthtml.DocumentNode.ChildNodes);

                using (var stream = new MemoryStream())
                {
                    template.Save(stream);
                    stream.Position = 0;
                    return new StreamReader(stream).ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Utility.Alert(ex.Message);
                return content;
            }
        }
    }
}