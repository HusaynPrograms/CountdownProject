using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using CountdownProject.Models;

namespace CountdownProject.Services
{
    public static class HistoryService
    {
        static readonly string FileName = "history.json";
        static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
        static List<HistoryEntry> cache;

        static string PathFile => System.IO.Path.Combine(FileSystem.AppDataDirectory, FileName);

        public static async Task<List<HistoryEntry>> GetAllAsync()
        {
            if (cache != null) return new List<HistoryEntry>(cache);

            if (!File.Exists(PathFile))
            {
                cache = new List<HistoryEntry>();
                return new List<HistoryEntry>();
            }

            using var fs = File.OpenRead(PathFile);
            cache = await JsonSerializer.DeserializeAsync<List<HistoryEntry>>(fs) ?? new List<HistoryEntry>();
            return new List<HistoryEntry>(cache);
        }

        public static async Task AddAsync(HistoryEntry e)
        {
            if (cache == null) await GetAllAsync();
            cache.Add(e);
            Directory.CreateDirectory(FileSystem.AppDataDirectory);
            using var fs = File.Create(PathFile);
            await JsonSerializer.SerializeAsync(fs, cache, JsonOpts);
        }

        public static async Task ClearAsync()
        {
            cache = new List<HistoryEntry>();
            Directory.CreateDirectory(FileSystem.AppDataDirectory);
            using var fs = File.Create(PathFile);
            await JsonSerializer.SerializeAsync(fs, cache, JsonOpts);
        }
    }
}
