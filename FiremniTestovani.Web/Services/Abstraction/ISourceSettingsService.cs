using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Services.Abstraction
{
    public interface ISourceSettingsService
    {
        T GetSettingsForCurrentSource<T>(string key, bool throwIfNotFound);
        T GetSettingsForCurrentSource<T>(string key, T defaultValue, bool throwIfNotFound);
        ValueTask<T> GetSettingsForCurrentSourceAsync<T>(string key, bool throwIfNotFound);
        ValueTask<T> GetSettingsForCurrentSourceAsync<T>(string key, T defaultValue, bool throwIfNotFound);
        bool ContainsKey(string key);
    }
}
