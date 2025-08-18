using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace CountdownProject.Services
{
    public static class DictionaryService
    {
        static readonly string FileName = "cdwords.txt";
        static readonly string Url = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/cdwords.txt";
        static HashSet<string> _words;
        static readonly object _gate = new();

        public static async Task EnsureLoadedAsync()
        {
            if (_words != null) return;

            var path = Path.Combine(FileSystem.AppDataDirectory, FileName);
            if (!File.Exists(path))
            {
                using var http = new HttpClient();
                var data = await http.GetStringAsync(Url);
                Directory.CreateDirectory(FileSystem.AppDataDirectory);
                File.WriteAllText(path, data);
            }

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using var sr = File.OpenText(path);
            while (true)
            {
                var line = await sr.ReadLineAsync();
                if (line == null) break;
                line = line.Trim();
                if (line.Length > 0) set.Add(line);
            }

            lock (_gate) _words = set;
        }

        public static bool IsWord(string w)
        {
            var s = _words;
            if (s == null) return false;
            return s.Contains(w);
        }
    }
}
