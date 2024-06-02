using System.Threading.Tasks;

namespace Unity.Services.Multiplay.Authoring.Core.Builds
{
    interface ICloudStorageFactory
    {
        Task<ICloudStorage> Build();
    }
}
