using FiremniTestovani.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Services.Abstraction
{
    public interface ISourceDetectionService
    {
        ApplicationSource ApplicationSource { get; }

        ValueTask<ApplicationSource> LoadApplicationSource(int? sourceID = null);
    }
}
