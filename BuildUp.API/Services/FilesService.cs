using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services
{
    public class FilesService : IFilesService
    {
        private readonly GridFSBucket _gridFS;

        public FilesService(IMongoSettings mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _gridFS = new GridFSBucket(database);
        }

        public async Task<string> UploadFile(string filename, byte[] fileBytes, bool shouldOverride = true)
        {
            if (shouldOverride)
            {
                var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Filename, filename);
                var currentFile = await (await _gridFS.FindAsync(filter)).FirstOrDefaultAsync();
                
                if (currentFile != null)
                {
                    await _gridFS.DeleteAsync(currentFile.Id);
                }
            }

            var file = _gridFS.UploadFromBytesAsync(filename, fileBytes);

            return file.ToString();
        }

        public Task<byte[]> GetFile(string fileId)
        {
            return _gridFS.DownloadAsBytesAsync(fileId);
        }
    }
}
