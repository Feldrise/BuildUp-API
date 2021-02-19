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
        readonly IWebHostEnvironment _env;

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

        public bool SignCoachIntegration(string coachId, PdfIntegrationCoach values)
        {
            var filePath = Path.Combine(_env.ContentRootPath, $"PDF/integration_coach_fillable.pdf");
            var savePath = Path.Combine(_env.ContentRootPath, $"wwwroot/pdf/coachs/{coachId}.pdf");

            //var filePath = "/PDF/integration_coach_fillable.pdf";
            //var savePath = "/PDF/saved/toSave.pdf";

            PdfDocument pdf = new PdfDocument(new PdfReader(filePath), new PdfWriter(savePath));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, false);

            var fields = form.GetFormFields();

            foreach (PdfFormField field in fields.Values)
            {
                field.SetFontSize(12);
            }

            fields["Nom"].SetValue(values.LastName);
            fields["Prénom"].SetValue(values.FirstName);
            fields["Birthdate"].SetValue(values.Birthdate.ToString());
            fields["keywords"].SetValue(values.Keywords ?? "Inconnue");
            fields["situation"].SetValue(values.Situation);
            fields["Code_postal"].SetValue(values.PostalCode.ToString());
            fields["ville"].SetValue(values.City);
            fields["adresse"].SetValue(values.Address);
            fields["phone"].SetValue(values.Phone);
            fields["mail"].SetValue(values.Email);
            fields["Where"].SetValue(values.BirthPlace);
            fields["accroche"].SetValue(values.Accroche ?? "Inconnue");
            fields["experiences"].SetValue(values.Experience ?? "Inconnue");
            fields["ideal_builder"].SetValue(values.IdealBuilder ?? "Inconnue");
            fields["objectifs_coach"].SetValue(values.Objectifs ?? "Inconnue");
            fields["full_name"].SetValue($"{values.FirstName} {values.LastName}");
            fields["date_naissance"].SetValue(values.Birthdate.ToString());
            fields["lieu_residence"].SetValue($"{values.Address}, {values.PostalCode} {values.City}");
            fields["lieu_naissance"].SetValue(values.BirthPlace);
            fields["lieu_signe"].SetValue(values.City);
            fields["date_signe"].SetValue(DateTime.Now.ToString());

            PdfAcroForm.GetAcroForm(pdf, false).FlattenFields();

            pdf.Close();

            return true;
        }

        public bool SignBuilderIntegration(string builderId, PdfIntegrationBuilder values)
        {
            var filePath = Path.Combine(_env.ContentRootPath, $"PDF/integration_builder_fillable.pdf");
            var savePath = Path.Combine(_env.ContentRootPath, $"wwwroot/pdf/builders/{builderId}.pdf");

            //var filePath = "/PDF/integration_coach_fillable.pdf";
            //var savePath = "/PDF/saved/toSave.pdf";

            PdfDocument pdf = new PdfDocument(new PdfReader(filePath), new PdfWriter(savePath));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, false);

            var fields = form.GetFormFields();

            foreach (PdfFormField field in fields.Values)
            {
                field.SetFontSize(12);
            }

            fields["Nom"].SetValue(values.LastName);
            fields["Prénom"].SetValue(values.FirstName);
            fields["Where"].SetValue(values.BirthPlace);
            fields["Birthdate"].SetValue(values.Birthdate.ToString());
            fields["mail"].SetValue(values.Email);
            fields["adresse"].SetValue(values.Address);
            fields["phone"].SetValue(values.Phone);
            fields["Code_postal"].SetValue(values.PostalCode.ToString());
            fields["ville"].SetValue(values.City);
            fields["situation"].SetValue(values.Situation);
            fields["keywords"].SetValue(values.Keywords);
            fields["accroche"].SetValue(values.Accroche);
            fields["domaines"].SetValue(values.ProjectDomaine);
            fields["nom_projet"].SetValue(values.ProjectName);
            fields["date_lancement"].SetValue(values.ProjectLaunchDate.ToString());
            fields["description"].SetValue(values.ProjectDescription);
            fields["team_members"].SetValue(values.ProjectTeam);
            fields["Attentes"].SetValue(values.Expectation);
            fields["Objectifs"].SetValue(values.Objectifs);
            fields["full_name"].SetValue($"{values.FirstName} {values.LastName}");
            fields["date_naissance"].SetValue(values.Birthdate.ToString());
            fields["lieu_residence"].SetValue($"{values.Address}, {values.PostalCode} {values.City}");
            fields["lieu_naissance"].SetValue(values.BirthPlace);
            fields["lieu_signe"].SetValue(values.City);
            fields["date_signe"].SetValue(DateTime.Now.ToString());

            PdfAcroForm.GetAcroForm(pdf, false).FlattenFields();

            pdf.Close();

            return true;
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
