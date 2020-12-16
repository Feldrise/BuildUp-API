using BuildUp.API.Entities.Pdf;
using BuildUp.API.Services.Interfaces;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services
{
    public class PdfService : IPdfService
    {
        IWebHostEnvironment _env;

        public PdfService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GenerateAttestationMineur(PdfAttestationMineur values)
        {
            var filePath = Path.Combine(_env.ContentRootPath, $"PDF/attesation_mineur_fillable.pdf");
            MemoryStream stream = new MemoryStream();

            //var filePath = "/PDF/integration_coach_fillable.pdf";
            //var savePath = "/PDF/saved/toSave.pdf";

            PdfDocument pdf = new PdfDocument(new PdfReader(filePath), new PdfWriter(stream));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, false);

            var fields = form.GetFormFields();

            foreach (PdfFormField field in fields.Values)
            {
                field.SetFontSize(12);
            }

            fields["name"].SetValue($"{values.FistName} {values.LastName}");
            fields["address"].SetValue($"{values.AddressLine1}\n{values.AddressLine2}");
            fields["city"].SetValue(values.City);
            fields["postal-code"].SetValue(values.PostalCode);
            fields["email"].SetValue(values.Email);
            fields["phone"].SetValue(values.Phone);
            fields["made-at"].SetValue(values.MadeAt);
            fields["made-date"].SetValue(values.MadeDate);

            PdfAcroForm.GetAcroForm(pdf, false).FlattenFields();

            pdf.Close();

            return stream.ToArray();
        }

        public string TestPdfFields(string pdfName)
        {
            var filePath = Path.Combine(_env.ContentRootPath, $"PDF/{pdfName}.pdf");
            var savePath = Path.Combine(_env.ContentRootPath, "PDF/saved/toSave.pdf");

            //var filePath = "/PDF/integration_coach_fillable.pdf";
            //var savePath = "/PDF/saved/toSave.pdf";

            PdfDocument pdf = new PdfDocument(new PdfReader(filePath), new PdfWriter(savePath));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, false);

            var fields = form.GetFormFields();

            return string.Join("\n", fields.Keys.ToList());
        }
    }
}
