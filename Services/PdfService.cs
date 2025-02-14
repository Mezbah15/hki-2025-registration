
using hki_2025_registration.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;

namespace hki_2025_registration.Services
{
    public class PdfService
    {
        public static void GenerateHallTicketPdf(string pdfFilePath, Participant participant)
        {
            using (FileStream fs = new FileStream(pdfFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (Document document = new Document(PageSize.A4, 30, 30, 30, 30))
            using (PdfWriter writer = PdfWriter.GetInstance(document, fs))
            {
                document.Open();

                // Register Unicode font for Bengali support
                string fontPath = Path.Combine("wwwroot", "fonts", "ArialUnicodeMS.ttf"); // Or any other Unicode font path
                if (!File.Exists(fontPath))
                {
                    fontPath = Path.Combine("wwwroot", "fonts", "Vrinda.ttf"); // Try Vrinda if ArialUnicodeMS is not found
                    if (!File.Exists(fontPath))
                    {
                        Console.WriteLine("Warning: Unicode font not found. Bengali characters might not render correctly.");
                        fontPath = null; // Fallback to default font
                    }
                }

                BaseFont bf_bengali = null;
                if (fontPath != null)
                {
                    bf_bengali = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                }
                else
                {
                    bf_bengali = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED); // Fallback font
                }


                BaseFont bf_bold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                BaseFont bf_regular = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font font_header_bengali = new Font(bf_bengali, 18, Font.NORMAL, BaseColor.BLACK);
                Font font_body_bengali = new Font(bf_bengali, 12, Font.NORMAL, BaseColor.BLACK);
                Font font_bold_label = new Font(bf_bold, 12, Font.NORMAL, BaseColor.BLACK);
                Font font_regular_text = new Font(bf_regular, 12, Font.NORMAL, BaseColor.BLACK);
                Font font_footer_bengali = new Font(bf_bengali, 10, Font.NORMAL, BaseColor.BLACK);


                // Header
                Paragraph headerTitle = new Paragraph("হামদ্-নাত, কিরাত ও ইসলামিক সাধারণজ্ঞান প্রতিযোগীতার প্রবেশপত্র - ২০২৫", font_header_bengali);
                headerTitle.Alignment = Element.ALIGN_CENTER;
                document.Add(headerTitle);

                Paragraph headerLocation1 = new Paragraph("স্থান: হারাগাছ বহুমুখী উচ্চ বিদ্যালয়।", font_body_bengali);
                headerLocation1.Alignment = Element.ALIGN_CENTER;
                document.Add(headerLocation1);

                Paragraph headerLocation2 = new Paragraph("হারাগাছ পৌরসভা, কাউনিয়া, রংপুর।", font_body_bengali);
                headerLocation2.Alignment = Element.ALIGN_CENTER;
                document.Add(headerLocation2);

                // Body
                PdfPTable bodyTable = new PdfPTable(2);
                bodyTable.WidthPercentage = 100;
                bodyTable.SpacingBefore = 20f;
                float[] widthsBody = new float[] { 30f, 70f };
                bodyTable.SetWidths(widthsBody);

                AddTableCellBengali(bodyTable, "ট্রাকিং নম্বর", font_bold_label, font_regular_text, participant.InvoiceNumber);
                AddTableCellBengali(bodyTable, "প্রতিযোগীতার বিষয়", font_bold_label, font_regular_text, "হামদ্-নাত, কিরাত ও ইসলামিক সাধারণজ্ঞান"); // Assuming fixed value based on requirement
                AddTableCellBengali(bodyTable, "নাম", font_bold_label, font_regular_text, participant.Name);
                AddTableCellBengali(bodyTable, "পিতার নাম", font_bold_label, font_regular_text, participant.FatherName);
                AddTableCellBengali(bodyTable, "জন্মতারিখ", font_bold_label, font_regular_text, participant.DoB.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)); // Format DateOnly
                AddTableCellBengali(bodyTable, "মোবাইল নম্বর", font_bold_label, font_regular_text, participant.Contact);
                AddTableCellBengali(bodyTable, "ই-মেইল", font_bold_label, font_regular_text, participant.Email);
                AddTableCellBengali(bodyTable, "ঠিকানা", font_bold_label, font_regular_text, participant.Address);

                document.Add(bodyTable);

                // Footer
                Paragraph footer1 = new Paragraph("১. সকল প্রতিযোগীতা হারাগাছ হাই স্কুলে (হারাগাছ পৌরসভা, কাউনিয়া, রংপুর) অনুষ্ঠিত হবে ইনশাআল্লাহ। ", font_footer_bengali);
                footer1.Alignment = Element.ALIGN_LEFT;
                footer1.SpacingBefore = 30f;
                document.Add(footer1);

                Paragraph footer2 = new Paragraph("২. আপনার প্রতিযোগীতার তারিখ ও সময় মোবাইল নাম্বারে এস এম এস-এর মাধ্যমে জানানো হবে ইনশাআল্লাহ।", font_footer_bengali);
                footer2.Alignment = Element.ALIGN_LEFT;
                document.Add(footer2);

                Paragraph footer3 = new Paragraph("৩. প্রতিযোগীকে অবশ্যই এডমিটের প্রিন্ট সাথে নিয়ে আসতে হবে।", font_footer_bengali);
                footer3.Alignment = Element.ALIGN_LEFT;
                document.Add(footer3);


                document.Close();
            }
        }


        static void AddTableCellBengali(PdfPTable table, string label, Font labelFont, Font textFont, string textValue)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label + ":", labelFont));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.PaddingLeft = 5;
            labelCell.PaddingTop = 2;
            labelCell.PaddingBottom = 2;
            table.AddCell(labelCell);

            PdfPCell textCell = new PdfPCell(new Phrase(textValue, textFont));
            textCell.Border = Rectangle.NO_BORDER;
            textCell.PaddingLeft = 5;
            textCell.PaddingTop = 2;
            textCell.PaddingBottom = 2;
            table.AddCell(textCell);
        }
    }
}
