using BuildUp.API.Models;
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

            var file = await _gridFS.UploadFromBytesAsync(filename, fileBytes);

            return file.ToString();
        }

        public async Task<FileModel> GetFile(string fileId)
        {
            var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Id, new ObjectId(fileId));
            var currentFile = await(await _gridFS.FindAsync(filter)).FirstOrDefaultAsync();

            if (currentFile == null)
            {
                return null;
            }

            byte[] bytes = await _gridFS.DownloadAsBytesAsync(currentFile.Id);

            return new FileModel()
            {
                Filename = currentFile.Filename,
                Data = bytes
            }; 
        }

        public async Task<FileModel> GetFileByName(string filename) 
        {
            var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Filename, filename);
            var currentFile = await(await _gridFS.FindAsync(filter)).FirstOrDefaultAsync();

            if (currentFile == null)
            {
                return null;
            }

            byte[] bytes = await _gridFS.DownloadAsBytesAsync(currentFile.Id);

            return new FileModel()
            {
                Filename = currentFile.Filename,
                Data = bytes
            };
        }
    }
}
