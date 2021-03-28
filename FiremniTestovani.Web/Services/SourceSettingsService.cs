using FiremniTestovani.Data.DbContext;
using FiremniTestovani.Models;
using FiremniTestovani.Web.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Services
{
    public class SourceSettingsService : ISourceSettingsService
    {
        private readonly ISourceDetectionService _detectionService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ApplicationDbContext _db;

        private Dictionary<string, ApplicationSettings> _settings;

        public SourceSettingsService(
            ISourceDetectionService detectionService,
            IHttpContextAccessor httpContext,
            ApplicationDbContext db)
        {
            this._detectionService = detectionService;
            this._httpContext = httpContext;
            this._db = db;

            var source = detectionService.ApplicationSource;

            var dbSettings = db.Settings
                .Where(i => i.SourceID == source.SourceID)
                .ToList();

            _settings = dbSettings
                .Select(i => new ApplicationSettings(i))
                .ToDictionary(i => i.Name);
        }

        public bool ContainsKey(string key)
        {
            return this._settings.ContainsKey(key);
        }

        public T GetSettingsForCurrentSource<T>(string key, bool throwIfNotFound)
        {
            if (!this._settings.TryGetValue(key, out ApplicationSettings settings))
            {
                if (throwIfNotFound)
                    throw new KeyNotFoundException($"Settings with key {key} not found.");

                return default(T);
            }

            if (IsSimple(typeof(T)))
            {
                return (T)Convert.ChangeType(settings.Value, typeof(T));
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(settings.Value);
            }
        }

        public async ValueTask<T> GetSettingsForCurrentSourceAsync<T>(string key, bool throwIfNotFound)
        {
            if (!this._settings.TryGetValue(key, out ApplicationSettings settings))
            {
                if (throwIfNotFound)
                    throw new KeyNotFoundException($"Settings with key {key} not found.");

                return default(T);
            }

            if (IsSimple(typeof(T)))
            {
                return (T)Convert.ChangeType(settings.Value, typeof(T));
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(settings.Value);
            }
        }

        public T GetSettingsForCurrentSource<T>(string key, T defaultValue, bool throwIfNotFound)
        {
            if (!this._settings.TryGetValue(key, out ApplicationSettings settings))
            {
                if (throwIfNotFound)
                    throw new KeyNotFoundException($"Settings with key {key} not found.");

                return defaultValue;
            }

            if (IsSimple(typeof(T)))
            {
                return (T)Convert.ChangeType(settings.Value, typeof(T));
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(settings.Value);
            }
        }

        public async ValueTask<T> GetSettingsForCurrentSourceAsync<T>(string key, T defaultValue, bool throwIfNotFound)
        {
            if (!this._settings.TryGetValue(key, out ApplicationSettings settings))
            {
                if (throwIfNotFound)
                    throw new KeyNotFoundException($"Settings with key {key} not found.");

                return defaultValue;
            }

            if (IsSimple(typeof(T)))
            {
                return (T)Convert.ChangeType(settings.Value, typeof(T));
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(settings.Value);
            }
        }

        private bool IsSimple(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(typeInfo.GetGenericArguments()[0]);
            }

            return typeInfo.IsPrimitive
              || typeInfo.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal));
        }
    }
}
