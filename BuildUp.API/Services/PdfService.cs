using BuildUp.API.Entities.Pdf;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
            PdfFont documentFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var fields = form.GetFormFields();

            foreach (PdfFormField field in fields.Values)
            {
                field.SetFontSize(10);
                field.SetFont(documentFont);
            }

            fields["last_name"].SetValue(values.LastName);
            fields["first_name"].SetValue(values.FirstName);
            fields["birthdate"].SetValue(string.Format("{0:dd/MM/yyyy}", values.Birthdate));
            fields["birthplace"].SetValue(values.BirthPlace);
            fields["email"].SetValue(values.Email);
            fields["phone"].SetValue(values.Phone);
            fields["linkedin"].SetValue(values.LinkedIn ?? "");
            fields["discord"].SetValue(values.DiscordTag ?? "");
            fields["situation"].SetValue(values.Situation);
            fields["address"].SetValue(values.Address);
            fields["postal_code"].SetValue(values.PostalCode.ToString());
            fields["city"].SetValue(values.City);
            fields["keywords"].SetValue(values.Keywords ?? "Inconnue");
            fields["experiences"].SetValue(values.Experience ?? "Inconnue");
            fields["ideal_builder"].SetValue(values.IdealBuilder ?? "Inconnue");
            fields["s_full_name"].SetValue($"{values.FirstName} {values.LastName}");
            fields["s_birthdate"].SetValue(string.Format("{0:dd/MM/yyyy}", values.Birthdate));
            fields["s_birthplace"].SetValue(values.BirthPlace);
            fields["s_address"].SetValue($"{values.Address}, {values.PostalCode} {values.City}");
            fields["s_sign_place"].SetValue(values.City);
            fields["s_sign_date"].SetValue(string.Format("{0:dd/MM/yyyy}", DateTime.Now));

            PdfAcroForm.GetAcroForm(pdf, false).FlattenFields();

            pdf.Close();

            return true;
        }
        public byte[] GenerateCoachCard(string coachId, PdfCoachCard values)
        {
            var filePath = Path.Combine(_env.ContentRootPath, $"PDF/carte_coach_fillable.pdf");
            var savePath = Path.Combine(_env.ContentRootPath, $"wwwroot/pdf/coachs/cards/{coachId}.pdf");
            var saveImagePath = Path.Combine(_env.ContentRootPath, $"wwwroot/pdf/coachs/cards/{coachId}.png");

            //var filePath = "/PDF/integration_coach_fillable.pdf";
            //var savePath = "/PDF/saved/toSave.pdf";

            PdfDocument pdf = new PdfDocument(new PdfReader(filePath), new PdfWriter(savePath));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, false);
            PdfFont documentBoldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            PdfFont documentRegularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var fields = form.GetFormFields();

            fields["last_name"].SetFontSize(42);
            fields["last_name"].SetFont(documentBoldFont);
            fields["last_name"].SetValue(values.LastName);

            fields["first_name"].SetFontSize(42);
            fields["first_name"].SetFont(documentBoldFont);
            fields["first_name"].SetValue(values.FirstName);

            fields["birthdate"].SetFontSize(22);
            fields["birthdate"].SetFont(documentRegularFont);
            fields["birthdate"].SetValue(string.Format("{0:dd/MM/yyyy}", values.Birthdate));

            fields["validity_date"].SetFontSize(22);
            fields["validity_date"].SetFont(documentRegularFont);
            fields["validity_date"].SetValue(string.Format("{0:dd/MM/yyyy}", values.ValidityDate));

            PdfAcroForm.GetAcroForm(pdf, false).FlattenFields();

            pdf.Close();

            $"gs -sDEVICE=pngalpha -sOutputFile={saveImagePath} -r144 {savePath}".Bash();

            return File.ReadAllBytes(saveImagePath);
        }

        public bool SignBuilderIntegration(string builderId, PdfIntegrationBuilder values)
        {
            var filePath = Path.Combine(_env.ContentRootPath, $"PDF/integration_builder_fillable.pdf");
            var savePath = Path.Combine(_env.ContentRootPath, $"wwwroot/pdf/builders/{builderId}.pdf");

            PdfDocument pdf = new PdfDocument(new PdfReader(filePath), new PdfWriter(savePath));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, false);
            PdfFont documentFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var fields = form.GetFormFields();

            foreach (PdfFormField field in fields.Values)
            {
                field.SetFontSize(10);
                field.SetFont(documentFont);
            }

            fields["lastname"].SetValue(values.LastName);
            fields["firstname"].SetValue(values.FirstName);
            fields["birth_place"].SetValue(values.BirthPlace ?? values.City);
            fields["birthdate"].SetValue(string.Format("{0:dd/MM/yyyy}", values.Birthdate));
            fields["email"].SetValue(values.Email);
            fields["mobile"].SetValue(values.Phone);
            fields["linkedin"].SetValue(values.LinkedIn ?? "");
            fields["discord"].SetValue(values.DiscordTag ?? "");
            fields["address"].SetValue(values.Address);
            fields["city"].SetValue(values.City);
            fields["postal_code"].SetValue(values.PostalCode.ToString());
            fields["situation"].SetValue(values.Situation);
            fields["domains"].SetValue(values.ProjectDomaine);
            fields["project_name"].SetValue(values.ProjectName);
            fields["project_launch_date"].SetValue(string.Format("{0:dd/MM/yyyy}", values.ProjectLaunchDate));
            fields["description"].SetValue(values.ProjectDescription);
            fields["team"].SetValue(values.ProjectTeam);
            fields["expectations"].SetValue(values.Expectation ?? "Impossible de récupérer cet information");
            fields["s_fullname"].SetValue($"{values.FirstName} {values.LastName}");
            fields["s_birthdate"].SetValue(string.Format("{0:dd/MM/yyyy}", values.Birthdate));
            fields["s_place_birth"].SetValue(values.BirthPlace);
            fields["s_address"].SetValue($"{values.Address}, {values.PostalCode} {values.City}");
            fields["s_sign_place"].SetValue(values.City);
            fields["s_sign_date"].SetValue(string.Format("{0:dd/MM/yyyy}", DateTime.Now));

            PdfAcroForm.GetAcroForm(pdf, false).FlattenFields();

            pdf.Close();

            return true;
        }

        public byte[] GenerateBuilderCard(string builderId, PdfBuilderCard values)
        {
            var filePath = Path.Combine(_env.ContentRootPath, $"PDF/carte_builder_fillable.pdf");
            var savePath = Path.Combine(_env.ContentRootPath, $"wwwroot/pdf/builders/cards/{builderId}.pdf");
            var saveImagePath = Path.Combine(_env.ContentRootPath, $"wwwroot/pdf/builders/cards/{builderId}.png");

            PdfDocument pdf = new PdfDocument(new PdfReader(filePath), new PdfWriter(savePath));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, false);
            PdfFont documentBoldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            PdfFont documentRegularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var fields = form.GetFormFields();

            fields["last_name"].SetFontSize(42);
            fields["last_name"].SetFont(documentBoldFont);
            fields["last_name"].SetValue(values.LastName);

            fields["first_name"].SetFontSize(42);
            fields["first_name"].SetFont(documentBoldFont);
            fields["first_name"].SetValue(values.FirstName);

            fields["birthdate"].SetFontSize(22);
            fields["birthdate"].SetFont(documentRegularFont);
            fields["birthdate"].SetValue(string.Format("{0:dd/MM/yyyy}", values.Birthdate));

            fields["validity_date"].SetFontSize(22);
            fields["validity_date"].SetFont(documentRegularFont);
            fields["validity_date"].SetValue(string.Format("{0:dd/MM/yyyy}", values.ValidityDate));

            PdfAcroForm.GetAcroForm(pdf, false).FlattenFields();

            pdf.Close();

            $"gs -sDEVICE=pngalpha -sOutputFile={saveImagePath} -r144 {savePath}".Bash();

            return File.ReadAllBytes(saveImagePath);
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

        private byte[] GetImageBytes(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
}
