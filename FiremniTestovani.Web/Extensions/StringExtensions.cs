using Microsoft.AspNetCore.Html;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Extensions
{
    public static partial class StringExtensions
    {
        public static Random random = new Random();

        public static string Base64Encode(this string str)
        {
            var strBytes = System.Text.Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(strBytes);
        }

        public static byte[] Base64Decode(this string str)
        {
            return System.Convert.FromBase64String(str);
        }

        public static string Sign(this string input, string pathToFile, string password)
        {
            string pubKeyFile = pathToFile;
            byte[] bytesToSign = Encoding.Default.GetBytes(input);

            var t = File.ReadAllBytes(pubKeyFile);

            //X509Certificate2 cert = new X509Certificate2(pubKeyFile, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            X509Certificate2 cert = new X509Certificate2(t, password);

            byte[] signature = cert.GetRSAPrivateKey().SignData(bytesToSign, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signature);
        }

        /// <summary>
        /// Vytvoří SHA-1 z inputu
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSHA1Hash(this string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string ChangeToANSI(this string str)
        {
            //Encoding target = Encoding.GetEncoding("windows-1250");
            Encoding target = Encoding.Unicode;
            Encoding source = Encoding.GetEncoding("windows-1250");
            byte[] sourceBytes = source.GetBytes(str);
            byte[] targetBytes = Encoding.Convert(source, target, sourceBytes);
            string msg = target.GetString(targetBytes);

            return msg;
        }

        public static DateTime ParseSAPDateString(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return DateTime.MinValue;

            if (DateTime.TryParseExact(str, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return result;

            return DateTime.MinValue;
        }

        public static string ReplaceByStringIfEmpty(this string value, string emptyValue)
        {
            if (!String.IsNullOrEmpty(value))
                return value;

            return emptyValue;
        }

        public static IHtmlContent ReplaceByHtmlIfEmpty(this string value, string emptyValue)
        {
            if (!String.IsNullOrEmpty(value))
                return new HtmlString(value);

            return new HtmlString(emptyValue);
        }

        public static string InsertAndOverwrite(this string value, string insertValue, int insertPosition = 0, bool keepLength = false, bool throwExceptionIfLengthExceeded = true)
        {
            // délka vstupního řetězce
            int valueLength = value.Length;
            // délka řetězce pro vepsání
            int insertValueLength = insertValue.Length;

            // pokud vkládáme na vyšší pozici než je celková délka řetězce, vyhodit chybu
            if (valueLength > insertPosition)
            {
                throw new ArgumentOutOfRangeException("insertPosition", insertPosition, $"Hodnota parametru insertPosition [{insertPosition}] je větší než celková délka řetězce [{valueLength}].");
            }

            // zdali bude vsunutím řetězce překročena délka
            bool exceedsLength = insertPosition + insertValueLength > valueLength;

            // pokud chceme zachovat délku a překračuje délku a chceme vyhodit chybu při překročení délky
            // vyhodit chybu
            if (keepLength && exceedsLength && throwExceptionIfLengthExceeded)
            {
                throw new ArgumentOutOfRangeException("insertPosition", insertPosition, $"Přepsáním hodnoty by byla překročena délka řetězce.");
            }

            // obsah řetězce před pozicí na vložení
            string tempString = value.Substring(0, insertPosition);

            tempString += insertValue;

            if (insertPosition + insertValueLength < valueLength)
            {
                tempString += value.Substring(insertPosition + insertValueLength + 1);
            }

            // pokud chci zachovat délku a délka by byla překročena, uříznout
            if (keepLength && exceedsLength)
            {
                tempString = tempString.Substring(0, valueLength);
            }

            return tempString;
        }

        public static bool TryParseJson<T>(this string value, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(value, settings);
            return success;
        }

        public static MailboxAddress ToMailboxAddress(this string input, char delimiter = '/')
        {
            if (String.IsNullOrEmpty(input))
                return null;

            var inputSplit = input.Split(delimiter);

            if (inputSplit.Length > 1)
                return new MailboxAddress(inputSplit[0], inputSplit[1]);

            //return new MailboxAddress(inputSplit[0]);
            return MailboxAddress.Parse(inputSplit[0]);
        }

        public static string EnsureValidExternalURL(this string input)
        {
            if (String.IsNullOrEmpty(input))
                return null;

            if (input.StartsWith("http://") ||
                input.StartsWith("https://") ||
                input.StartsWith("//") || // current protocol
                input.StartsWith("/")) // page relative URL
                return input;

            return "http://" + input;
        }

        public static bool IsFullPath(string path)
        {
            return !String.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(System.IO.Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }

        public static IEnumerable<String> SplitInParts(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        static Dictionary<string, string> foreign_characters = new Dictionary<string, string>
        {
            { "äæǽ", "ae" },
            { "öœ", "oe" },
            { "ü", "ue" },
            { "Ä", "Ae" },
            { "Ü", "Ue" },
            { "Ö", "Oe" },
            { "ÀÁÂÃÄÅǺĀĂĄǍΑΆẢẠẦẪẨẬẰẮẴẲẶА", "A" },
            { "àáâãåǻāăąǎªαάảạầấẫẩậằắẵẳặа", "a" },
            { "Б", "B" },
            { "б", "b" },
            { "ÇĆĈĊČ", "C" },
            { "çćĉċč", "c" },
            { "Д", "D" },
            { "д", "d" },
            { "ÐĎĐΔ", "Dj" },
            { "ðďđδ", "dj" },
            { "ÈÉÊËĒĔĖĘĚΕΈẼẺẸỀẾỄỂỆЕЭ", "E" },
            { "èéêëēĕėęěέεẽẻẹềếễểệеэ", "e" },
            { "Ф", "F" },
            { "ф", "f" },
            { "ĜĞĠĢΓГҐ", "G" },
            { "ĝğġģγгґ", "g" },
            { "ĤĦ", "H" },
            { "ĥħ", "h" },
            { "ÌÍÎÏĨĪĬǏĮİΗΉΊΙΪỈỊИЫ", "I" },
            { "ìíîïĩīĭǐįıηήίιϊỉịиыї", "i" },
            { "Ĵ", "J" },
            { "ĵ", "j" },
            { "ĶΚК", "K" },
            { "ķκк", "k" },
            { "ĹĻĽĿŁΛЛ", "L" },
            { "ĺļľŀłλл", "l" },
            { "М", "M" },
            { "м", "m" },
            { "ÑŃŅŇΝН", "N" },
            { "ñńņňŉνн", "n" },
            { "ÒÓÔÕŌŎǑŐƠØǾΟΌΩΏỎỌỒỐỖỔỘỜỚỠỞỢО", "O" },
            { "òóôõōŏǒőơøǿºοόωώỏọồốỗổộờớỡởợо", "o" },
            { "П", "P" },
            { "п", "p" },
            { "ŔŖŘΡР", "R" },
            { "ŕŗřρр", "r" },
            { "ŚŜŞȘŠΣС", "S" },
            { "śŝşșšſσςс", "s" },
            { "ȚŢŤŦτТ", "T" },
            { "țţťŧт", "t" },
            { "ÙÚÛŨŪŬŮŰŲƯǓǕǗǙǛŨỦỤỪỨỮỬỰУ", "U" },
            { "ùúûũūŭůűųưǔǖǘǚǜυύϋủụừứữửựу", "u" },
            { "ÝŸŶΥΎΫỲỸỶỴЙ", "Y" },
            { "ýÿŷỳỹỷỵй", "y" },
            { "В", "V" },
            { "в", "v" },
            { "Ŵ", "W" },
            { "ŵ", "w" },
            { "ŹŻŽΖЗ", "Z" },
            { "źżžζз", "z" },
            { "ÆǼ", "AE" },
            { "ß", "ss" },
            { "Ĳ", "IJ" },
            { "ĳ", "ij" },
            { "Œ", "OE" },
            { "ƒ", "f" },
            { "ξ", "ks" },
            { "π", "p" },
            { "β", "v" },
            { "μ", "m" },
            { "ψ", "ps" },
            { "Ё", "Yo" },
            { "ё", "yo" },
            { "Є", "Ye" },
            { "є", "ye" },
            { "Ї", "Yi" },
            { "Ж", "Zh" },
            { "ж", "zh" },
            { "Х", "Kh" },
            { "х", "kh" },
            { "Ц", "Ts" },
            { "ц", "ts" },
            { "Ч", "Ch" },
            { "ч", "ch" },
            { "Ш", "Sh" },
            { "ш", "sh" },
            { "Щ", "Shch" },
            { "щ", "shch" },
            { "ЪъЬь", "" },
            { "Ю", "Yu" },
            { "ю", "yu" },
            { "Я", "Ya" },
            { "я", "ya" },
        };

        public static char RemoveDiacritics(this char c)
        {
            foreach (KeyValuePair<string, string> entry in foreign_characters)
            {
                if (entry.Key.IndexOf(c) != -1)
                {
                    return entry.Value[0];
                }
            }
            return c;
        }

        public static string RemoveDiacritics(this string s)
        {
            //StringBuilder sb = new StringBuilder ();
            string text = "";


            foreach (char c in s)
            {
                int len = text.Length;

                foreach (KeyValuePair<string, string> entry in foreign_characters)
                {
                    if (entry.Key.IndexOf(c) != -1)
                    {
                        text += entry.Value;
                        break;
                    }
                }

                if (len == text.Length)
                {
                    text += c;
                }
            }
            return text;
        }

        public static string Truncate(this string source, int length)
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length);
            }

            return source;
        }

        public static string TruncateAtWord(this string input, int length)
        {
            if (input == null || input.Length < length)
                return input;
            int iNextSpace = input.LastIndexOf(" ", length);
            return string.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
        }

        public static string GenerateQuestion()
        {
            int prvni = random.Next(10, 30);
            bool plus;

            if (random.Next(1, 10) > 5)
            {
                plus = true;
            }
            else
            {
                plus = false;
            }

            int druhe = random.Next(1, 10);

            if (plus == true)
            {
                return prvni + " + " + druhe;
            }
            else
            {
                return prvni + " - " + druhe;
            }
        }

        public static string NumberToText(int number)
        {
            switch (number)
            {
                case 1:
                    return "jedničku";
                case 2:
                    return "dvojku";
                case 3:
                    return "trojku";
                case 4:
                    return "čtyřku";
                case 5:
                    return "pětku";
                case 6:
                    return "šestku";
                case 7:
                    return "sedmičku";
                case 8:
                    return "osmičku";
                case 9:
                    return "devítku";
                case 10:
                    return "desítku";
                default:
                    return "jiné";
            }
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
