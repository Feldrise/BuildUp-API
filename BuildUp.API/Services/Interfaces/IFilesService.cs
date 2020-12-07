using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface IFilesService
    {
        Task<string> UploadFile(string filename, byte[] fileBytes, bool shouldOverride = true);
        
        Task<byte[]> GetFile(string fileId);
        Task<byte[]> GetFileByName(string filename);
    }
}
