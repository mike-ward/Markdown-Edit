using System.IO;
using System.Linq;
using System.Text;

namespace MarkdownEdit.Models
{
    public class MyEncodingInfo
    {
        public MyEncodingInfo()
        {
        }

        public MyEncodingInfo(EncodingInfo encodingInfo)
        {
            CodePage = encodingInfo.CodePage;
            Name = encodingInfo.Name;
            DisplayName = encodingInfo.DisplayName;
        }

        public int CodePage { get; set; } = 65001;
        public string Name { get; set; } = "auto-detect";
        public string DisplayName { get; set; } = i18n.TranslationProvider.Translate("settings-encode-automatic") as string;

        public static MyEncodingInfo[] GetEncodings() => new[] {new MyEncodingInfo()}
            .Concat(Encoding.GetEncodings()
                .Select(encodingInfo => new MyEncodingInfo(encodingInfo))
                .OrderBy(ei => ei.DisplayName))
            .ToArray();

        public static bool IsAutoDetectEncoding(MyEncodingInfo encodingInfo)
            => encodingInfo != null && encodingInfo.Name == "auto-detect";

        public static Encoding DetectEncoding(string filename)
        {
            using (var fs = File.OpenRead(filename))
            {
                var cd = new Ude.CharsetDetector();
                cd.Feed(fs);
                cd.DataEnd();
                return cd.Charset != null ? Encoding.GetEncoding(cd.Charset) : Encoding.UTF8;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as MyEncodingInfo;
            return other != null && other.Name == Name;
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => Name.GetHashCode();

        public override string ToString() => $"{CodePage}, {Name}, {DisplayName}";
    }
}