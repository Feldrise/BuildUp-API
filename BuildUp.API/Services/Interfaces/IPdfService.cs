using BuildUp.API.Entities.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateAttestationMineur(PdfAttestationMineur values);

        string TestPdfFields(String pdfName);
    }
}
