using BuildUp.API.Entities;
using BuildUp.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Controllers
{
    [Route("buildup/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.Admin)]
    public class FilesController : ControllerBase
    {
        private readonly IFilesService _filesService;

        public FilesController(IFilesService filesService)
        {
            _filesService = filesService;
        }

        /// <summary>
        /// (Admin) Get a file from its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The file</returns>
        /// <response code="401">You are not allowed to view files</response>
        /// <response code="200">Return the file</response>
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<byte[]>> GetFile(string id)
        {
            byte[] file = await _filesService.GetFile(id);

            return Ok(file);
        }

        /// <summary>
        /// (Admin) Get a file from its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The file</returns>
        /// <response code="401">You are not allowed to view files</response>
        /// <response code="200">Return the file</response>
        [HttpGet("name/{name}")]
        public async Task<ActionResult<byte[]>> GetFileByName(string name)
        {
            byte[] file = await _filesService.GetFileByName(name);

            if (file == null)
            {
                return NotFound();
            }

            return Ok(file);
        }

        /// <summary>
        /// Upload file to the server
        /// </summary>
        /// <param name="name" example="profil_898935a43"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <response code="400">Their is an error in the request</response>
        /// <response code="401">You are not allowed to upload files</response>
        /// <response code="200">Return the file's id on the server</response>
        [HttpPost("upload/{name}")]
        public async Task<ActionResult<string>> UploadFile(string name, [FromBody] byte[] file)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Your file need a name");
            }

            string id = await _filesService.UploadFile(name, file);

            return Ok(id);
        }
    }
}
