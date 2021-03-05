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
        
        bool SignCoachIntegration(string coachId, PdfIntegrationCoach values);
        byte[] GenerateCoachCard(string coachId, PdfCoachCard values);

        bool SignBuilderIntegration(string builderId, PdfIntegrationBuilder values);
        byte[] GenerateBuilderCard(string builderId, PdfBuilderCard values);

        string TestPdfFields(String pdfName);
    }
}
