using BuildUp.API.Entities.Pdf;
using BuildUp.API.Services.Interfaces;
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
    public class PdfController : ControllerBase
    {
        IPdfService _pdfService;

        public PdfController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpGet("attestation_mineur")]
        public ActionResult<byte[]> GenerateAttestationMineur([FromBody] PdfAttestationMineur values)
        {
            byte[] result = _pdfService.GenerateAttestationMineur(values);

            return Ok(result);
        }

        [HttpGet("test/{pdfName}")]
        public IActionResult Test(string pdfName)
        {
            string keys = _pdfService.TestPdfFields(pdfName);

            return Ok(keys);
        }
    }
}
