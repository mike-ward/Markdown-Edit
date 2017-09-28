using System;
using System.IO;
using Infrastructure;
using ServicesModule.Properties;

namespace ServicesModule.Services
{
    public class TemplateLoader : ITemplateLoader
    {
        private static readonly string UserSettingsFolder = Path.Combine(Utility.AssemblyFolder(), "user-settings");
        private readonly string _templateFile = Path.Combine(UserSettingsFolder, "user_template.html");
        private readonly string _emojiSpritesFile = Path.Combine(UserSettingsFolder, "emoji-sprite.png");

        public Uri DefaultTemplate()
        {
            if (File.Exists(_templateFile) == false)
            {
                var userTemplate = Resources.template_html_txt;
                Save(userTemplate);
            }
            if (File.Exists(_emojiSpritesFile) == false)
            {
                var emjoiSprites = Resources.emoji_sprites;
                emjoiSprites.Save(_emojiSpritesFile);
            }
            return new Uri(_templateFile);
        }

        private void Save(string template)
        {
            Directory.CreateDirectory(UserSettingsFolder);
            File.WriteAllText(_templateFile, template);
        }
    }
}
