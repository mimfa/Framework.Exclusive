using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using MiMFa.Engine.Template;
using Aspose.Words.Drawing.Charts;
using MiMFa.Model;
using System.Collections;
using MiMFa.Model.IO;
using System.Text.RegularExpressions;
using MiMFa.Service;

namespace MiMFa.Engine.Translate
{
    [Serializable]
    public class Language : IDictionary<string,string>
    {
        public ChainedFile Source { get; set; }
        public string Name { get => Options.GetOrDefault("Name", null); set => Options.AddOrSet("Name", value); }
        public string Alias { get => Options.GetOrDefault("Alias", null); set => Options.AddOrSet("Alias", value); }
        public string CharSet { get => Options.GetOrDefault("CharSet", "UTF8"); set => Options.AddOrSet("Alias", value); }
        public bool IsRightToLeft
        {
            get => Statement.Apply(v => v == null ? false : Convert.ToBoolean(v), Options.GetOrDefault("IsRightToLeft", null));
            set => Options.AddOrSet("IsRightToLeft", value+""); 
        }
        //public bool AllowOnline
        //{
        //    get => Statement.Apply(v => v == null ? false : Convert.ToBoolean(v), Options.GetOrDefault("AllowOnline", null));
        //    set => Options.AddOrSet("AllowOnline", value+""); 
        //}
        public bool AllowCache
        {
            get => Statement.Apply(v=>v==null?true: Convert.ToBoolean(v), Options.GetOrDefault("AllowCache", null));
            set => Options.AddOrSet("AllowCache", value+"");
        }
        public bool IgnoreCase
        {
            get => Statement.Apply(v => v == null ? true : Convert.ToBoolean(v), Options.GetOrDefault("IgnoreCase", null));
            set => Options.AddOrSet("IgnoreCase", value + "");
        }
        public bool Flexible
        {
            get => Statement.Apply(v => v == null ? true : Convert.ToBoolean(v), Options.GetOrDefault("Flexible", null));
            set => Options.AddOrSet("Flexible", value + "");
        }
        public string ValidKeyPattern { get => Options.GetOrDefault("ValidKeyPattern", "([A-Za-z][\\s\\-]?)+|(^[^\\\"\\w]+$)"); set => Options.AddOrSet("ValidKeyPattern", value); }
        public string InvalidKeyPattern { get => Options.GetOrDefault("InvalidKeyPattern", "(^\\w+\\:(\\\\|\\//)+)");/*|(^[\\d\\W](\\W?\\d)*$)*/ set => Options.AddOrSet("InvalidKeyPattern", value); }
        public string PartitionKeyPattern { get => Options.GetOrDefault("PartitionKeyPattern", "(?<not>\\\"[\\s\\S]*\\\")|(([0-9A-Za-z][\\s\\-]?)+)|([^\\\"\\w]+)"); set => Options.AddOrSet("PartitionKeyPattern", value); }
        public string RemoveKeyPattern { get => Options.GetOrDefault("RemoveKeyPattern", "(?<=\\\")[\\S\\s]*(?=\\\")|^\\s|\\s$"); set => Options.AddOrSet("RemoveKeyPattern", value); }
        public string ReplaceKeyPattern { get => Options.GetOrDefault("ReplaceKeyPattern", "(?<=\\\")[\\S\\s]*(?=\\\")"); set => Options.AddOrSet("ReplaceKeyPattern", value); }
        public string ReplaceValuePattern { get => Options.GetOrDefault("ReplaceValuePattern", "(?<=«)[\\S\\s]*(?=»)"); set => Options.AddOrSet("ReplaceValuePattern", value); }

        private SmartDictionary<string, string> _Options = new SmartDictionary<string, string>();
        public SmartDictionary<string, string> Options
        {
            get
            {
                if (_Options.Count < 1)
                    foreach (var item in from v in Source.Row(0) select v.Split(':'))
                        _Options.AddOrSet(item.First(), item.Last());
                return _Options;
            }
            set
            {
                _Options = value;
            }
        }
        public ICollection<string> Keys => Source.Column(0).ToList();
        public ICollection<string> Values => Source.Row(1).ToList();

        public int Count => (int)Source.LinesCount;

        public bool IsReadOnly => false;

        public string this[string key]
        {
            get => FindIndexOrCache(key).Value;
            set => Statement.Apply(v=> 
            {
                if (v.Key > 0) Source.ChangeCell(1, v.Key, value); 
            }, FindIndexOrCache(key, value));
        }

        public Language():this(null,Config.Language)
        {
        }
        public Language(string path, string name = null,string alias = null)
        {
            Source = new ChainedFile(path ?? Config.LanguageDirectory + (name ?? Config.Language) + Config.LanguageExtension) {ColumnsLabelsIndex = -1,RowsLabelsIndex = 0 };
            Name = name ?? Name?? Source.NameWithoutExtension;
            Alias = alias ?? Alias?? Name ?? Source.NameWithoutExtension;
            if (Source.LinesCount == 0) Source.WriteLine("");
            Source.IsAutoSave = true;
        }

        public bool ContainsKey(string key)
        {
            return FindIndex(key).Key > 0;
        }

        public bool AddOrSet(string key, string value)
        {
            if (Regex.IsMatch(key, ValidKeyPattern, RegexOptions.Multiline, new TimeSpan(0, 0, 1)))
            {
                var rows = Source.FindRows(ToPattern(key), 1, RegexOptions.None, 5);
                foreach (var item in rows)
                {
                    Source.ChangeCell(1, item.Key, value);
                    return false;
                }
                Source.WriteRow(key, value);
                return true;
            }
            return false;
        }
        public void Add(string key, string value)
        {
            if (Regex.IsMatch(key, ValidKeyPattern, RegexOptions.Multiline, new TimeSpan(0, 0, 1)))
                    Source.WriteRow(key, value);
        }

        public bool Remove(string key)
        {
            return Source.DeleteLine(FindIndex(key).Key) > 0;
        }

        public bool TryGetValue(string key, out string value)
        {
            var kvp = FindIndex(key);
            value = key;
            if (kvp.Key < 1) return false;
            value = kvp.Value;
            return true;
        }

        public void Add(KeyValuePair<string, string> item)
        {
            Source.WriteRow(item.Key, item.Value);
        }

        public void Clear()
        {
           Source.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return FindIndex(item.Key).Key > 0;
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            foreach (var item in array) Source.WriteRow(item.Key,item.Value);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return  Source.DeleteLine(FindIndex(item.Key).Key) > 0;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return (from v in Source.Rows select new KeyValuePair<string, string>(v.First(),v.Last())).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Source.AsEnumerable().GetEnumerator();
        }


        public string ToKey(string key) => Regex.Replace(key, RemoveKeyPattern, "");
        public string ToValue(string key,string standardKey, string value)
        {
            var nmaches = Regex.Matches(standardKey, ReplaceKeyPattern);
            if (nmaches.Count < 1) return key.Replace(standardKey,value);
            var kmaches = Regex.Matches(key, ReplaceKeyPattern);
            var vmaches = Regex.Matches(value, ReplaceValuePattern);
            for (int i = 0; i < MathService.Minimum(kmaches.Count, vmaches.Count, nmaches.Count); i++)
            {
                value = value.Substring(0, vmaches[i].Index) + kmaches[i].Value + value.Substring(vmaches[i].Index+ vmaches[i].Value.Length);
                standardKey = standardKey.Substring(0, nmaches[i].Index) + kmaches[i].Value + standardKey.Substring(nmaches[i].Index+ nmaches[i].Value.Length);
                //value = value.Replace(vmaches[i].Value, kmaches[i].Value);
                //normalKey = normalKey.Replace(nmaches[i].Value, kmaches[i].Value);
            }
            return key.Replace(standardKey, value);
        }
        public string ToPattern(string key) => string.Join("", "^", Regex.Escape(Source.Connector.EscapeChars(key)), "(?=(", Regex.Escape(Source.WarpsSplitter), "))");
        
        public KeyValuePair<long, string> FindIndex(string pattern,RegexOptions options)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return new KeyValuePair<long, string>(-1, pattern);
            foreach (var v in Source.FindRows(ToPattern(string.Join("", "^", pattern, "(?=", Source.WarpsSplitter, ")")), 1, RegexOptions.None, 5))
                return new KeyValuePair<long, string>(v.Key, ToValue(v.Value.ElementAtOrDefault(0),ToKey(v.Value.ElementAtOrDefault(0)), v.Value.ElementAtOrDefault(1)));
            return new KeyValuePair<long, string>(-1, null);
        }
        public KeyValuePair<long, string> FindIndex(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || Regex.IsMatch(key, InvalidKeyPattern)) return new KeyValuePair<long, string>(-1, key);
            var nkey = ToKey(key);
            foreach (var v in Source.FindRows(ToPattern(nkey), 1, RegexOptions.None, 5))
                return new KeyValuePair<long, string>(v.Key, ToValue(key, nkey, v.Value.ElementAtOrDefault(1) ?? key));
            if (IgnoreCase)
                foreach (var v in Source.FindRows(ToPattern(nkey), 1, RegexOptions.IgnoreCase, 5))
                    return new KeyValuePair<long, string>(v.Key, ToValue(key, nkey, v.Value.ElementAtOrDefault(1) ?? key));
            return new KeyValuePair<long, string>(-1, key);
        }
        public KeyValuePair<long, string> FindIndexOrCache(string key,string defValue = null)
        {
            if(string.IsNullOrWhiteSpace(key)) return new KeyValuePair<long, string>(-1, defValue?? key);
            if(Regex.IsMatch(key,InvalidKeyPattern)) return new KeyValuePair<long, string>(-1, key);
            var nkey = ToKey(key);
            var pat = ToPattern(nkey);
            foreach (var v in Source.FindRows(pat, 1, RegexOptions.None, 5))
                return new KeyValuePair<long, string>(v.Key, ToValue(key, nkey, v.Value.ElementAtOrDefault(1) ?? nkey));
            if (IgnoreCase)
                foreach (var v in Source.FindRows(pat, 1, RegexOptions.IgnoreCase, 5))
                    return new KeyValuePair<long, string>(v.Key, ToValue(key, nkey, v.Value.ElementAtOrDefault(1) ?? nkey));
            if (defValue == null)
            {
                defValue = key;
                if (Flexible)
                {
                    var matches = Regex.Matches(key, PartitionKeyPattern);
                    if (matches.Count > 1 || (matches.Count == 1 && matches[0].Value.Length < defValue.Length))
                        foreach (Match item in matches)
                            if (!item.Groups["not"].Success)
                            {
                                var kvp = FindIndex(item.Value);
                                if (kvp.Key > 0) defValue = ToValue(defValue, item.Value, kvp.Value);
                            }
                }
                if(AllowCache) AddOrSet(nkey, defValue);
            }
            else AddOrSet(nkey, defValue = defValue ?? key);
            return new KeyValuePair<long, string>(-1, defValue);
        }
    }
}
