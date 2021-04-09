using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace SmartThermo.Modules.Dialog.SettingsSensor.Helpers
{
    public static class ExcelProvider
    {
        public static IEnumerable<string> ExtractDataFromFile()
        {
            var result = new List<string>();

            var openFileDialog = new OpenFileDialog
            {
                Title = "Открыть файл.",
                Multiselect = false,
                Filter = "Файлы Excel (*.xlsx)|*.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() != true)
                throw new Exception("Операция прервана пользователем.");

            using var file = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
            var workBook = new XSSFWorkbook(file);

            var sheet = workBook.GetSheetAt(0);
            for (var row = 0; row <= 31; row++)
            {
                var formatter = new DataFormatter();
                var value = formatter.FormatCellValue(sheet.GetRow(row).GetCell(1));

                if (value != null)
                    result.Add(value);
            }

            return result;
        }

        public static void UploadDataToFile(List<string> data)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Сохранить файл.",
                Filter = "Файлы Excel (*.xlsx)|*.xlsx",
                FileName = $"Таблица RT(R25) от {DateTime.Now.ToShortDateString()} {DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (saveFileDialog.ShowDialog() != true)
                throw new Exception("Операция прервана пользователем.");

            using var fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
            IWorkbook workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Sheet1");

            var temperature = 0;
            for (var i = 0; i <= 31; i++)
            {
                var row = sheet.CreateRow(i);
                row.CreateCell(0).SetCellValue(temperature);
                row.CreateCell(1).SetCellValue(data[i]);
                temperature += 5;
            }
            workbook.Write(fs);
        }
    }
}
