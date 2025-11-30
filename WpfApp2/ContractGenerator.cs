using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    internal class ContractGenerator
    {
        public static string GenerateContract(string phoneNumber, string templatePath = null)
        {
            // Находим шаблон
            if (string.IsNullOrEmpty(templatePath))
            {
                string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                templatePath = "D:\\учёба\\3 курс\\cellServiceWPF\\WpfApp2\\WpfApp2\\bin\\Debug\\Resources\\dogovor.docx";
            }

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Шаблон не найден: {templatePath}");

            // Получаем данные из БД
            var phoneObj = AppConnect.model0db.clientPhones.FirstOrDefault(x => x.phone == phoneNumber);

            var client = phoneObj.clients;
            var tariff = AppConnect.model0db.enableTariffs
                .Where(et => et.idClientPhone == phoneObj.idClientPhone)
                .OrderByDescending(et => et.date)
                .FirstOrDefault()?.tariffs;

            // Создаем путь для сохранения
            string outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Договоры");
            Directory.CreateDirectory(outputDir);
            string outputPath = Path.Combine(outputDir, $"Договор_{client.surname}_{client.name}_{DateTime.Now:yyyyMMdd_HHmmss}.docx");

            // Копируем и редактируем шаблон
            File.Copy(templatePath, outputPath, true);
            using (var doc = WordprocessingDocument.Open(outputPath, true))
            {
                var body = doc.MainDocumentPart?.Document?.Body;
                if (body == null) throw new Exception("Не удалось открыть документ");

                // Заменяем все плейсхолдеры
                var replacements = new[]
                {
                    ("[ФИО клиента]", client.surname + " " + client.name + " " + client.patronymic ?? ""),
                    ("[Фамилия]", client.surname ?? ""),
                    ("{Имя}", client.name ?? ""),
                    ("{Отчество}", client.patronymic ?? ""),
                    ("{Паспорт}", client.passport ?? ""),
                    ("{ДатаРождения}", client.birthDate.ToString("dd.MM.yyyy")),
                    ("[адрес]", client.address ?? ""),
                    ("{Телефон}", phoneNumber),
                    ("{НомерТелефона}", phoneNumber),
                    ("{НазваниеТарифа}", tariff?.name ?? "Не указан"),
                    ("{Минуты}", tariff?.minutes.ToString() ?? "0"),
                    ("{SMS}", tariff?.sms.ToString() ?? "0"),
                    ("{ГБ}", tariff?.gb.ToString() ?? "0"),
                    ("{Стоимость}", tariff?.price.ToString("F2") ?? "0"),
                    ("{ДатаДоговора}", DateTime.Now.ToString("dd.MM.yyyy")),
                    ("{Дата}", DateTime.Now.ToString("dd.MM.yyyy"))
                };

                foreach (var (find, replace) in replacements)
                    ReplaceText(body, find, replace);

                doc.MainDocumentPart.Document.Save();
            }

            return outputPath;
        }

        private static void ReplaceText(Body body, string find, string replace)
        {
            foreach (var paragraph in body.Descendants<Paragraph>())
            {
                if (paragraph.InnerText.Contains(find))
                {
                    string newText = paragraph.InnerText.Replace(find, replace);
                    var runProps = paragraph.Elements<Run>().FirstOrDefault()?.RunProperties?.Clone() as RunProperties;
                    paragraph.RemoveAllChildren<Run>();
                    paragraph.Append(new Run(new Text(newText)) { RunProperties = runProps });
                }
            }
        }
    }
}
