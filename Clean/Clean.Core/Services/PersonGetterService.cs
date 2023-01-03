using Clean.Core.Domain.Entities;
using Clean.Core.Domain.RepositoryContracts;
using Clean.Core.DTO.PersonDTO;
using Clean.Core.ServiceContracts;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using Serilog;
using SerilogTimings;
using System.Globalization;

namespace Clean.Core.Services;


public class PersonGetterService : IPersonGetterService
{
    private readonly IPersonRepository personRepository;
    private readonly IDiagnosticContext diagnosticContext;

    public PersonGetterService(IPersonRepository personRepository, IDiagnosticContext diagnosticContext)
    {
        this.personRepository = personRepository;
        this.diagnosticContext = diagnosticContext;
    }


    public async Task<List<PersonResponse>> GetAllPersonsAsync()
    {
        var persons = await personRepository.GetAllAsync();
        var responseList = persons.Select(x => x.ToPersonResponse()).ToList();

        return responseList;
    }

    public async Task<List<PersonResponse>> GetFilteredPersonsAsync(string searchBy, string? searchString)
    {
        List<Person> persons = new List<Person>();

        if (string.IsNullOrWhiteSpace(searchString))
        {
            persons = await personRepository.GetAllAsync();
            return persons.Select(x => x.ToPersonResponse()).ToList();
        }

        using (Operation.Time("Time for Filtered Persons"))
        {
            persons = searchBy switch
            {
                nameof(Person.Name) =>
                        await personRepository.GetFilteredPersonsAsync(x => x.Name.Contains(searchString)),

                nameof(Person.Email) =>
                        await personRepository.GetFilteredPersonsAsync(x => x.Email.Contains(searchString)),

                nameof(PersonResponse.Gender) =>
                        await personRepository.GetFilteredPersonsAsync(x => x.Gender != null && x.Gender.Contains(searchString)),

                nameof(PersonResponse.CountryName) =>
                        await personRepository.GetFilteredPersonsAsync(x => x.Country!.Name != null && x.Country.Name.Contains(searchString)),

                nameof(Person.Address) =>
                        await personRepository.GetFilteredPersonsAsync(x => x.Address != null && x.Address.Contains(searchString)),

                _ =>
                       await personRepository.GetAllAsync()
            };
        }

        diagnosticContext.Set("Persons", persons);

        var personsResponseList = persons.Select(x => x.ToPersonResponse()).ToList();

        return personsResponseList;
    }

    public async Task<PersonResponse?> GetPersonByIdAsync(Guid? personId)
    {
        var person = await personRepository.GetByIdAsync(personId.GetValueOrDefault());

        if (person == null)
            return null;

        return person.ToPersonResponse();
    }

    public async Task<MemoryStream> GetPersonsCSV()
    {
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);

        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture);
        CsvWriter csvWriter = new CsvWriter(streamWriter, config);

        // Headers. "," added automatically
        csvWriter.WriteField(nameof(PersonResponse.Name));
        csvWriter.WriteField(nameof(PersonResponse.Email));
        csvWriter.WriteField(nameof(PersonResponse.Age));
        csvWriter.NextRecord(); // new line

        var list = await personRepository.GetAllAsync();
        var persons = list.Select(x => x.ToPersonResponse()).ToList();

        foreach (var item in persons)
        {
            // properties values
            csvWriter.WriteField(item.Name);
            csvWriter.WriteField(item.Email);
            csvWriter.WriteField(item.Age);
            csvWriter.NextRecord(); // new line
        }

        csvWriter.Flush(); // flushes written content to memory stream
        memoryStream.Position = 0; // sets memory stream to the beggining

        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsExcel()
    {
        var memoryStream = new MemoryStream();

        using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
            workSheet.Cells["A1"].Value = "Person Name";
            workSheet.Cells["B1"].Value = "Email";
            workSheet.Cells["C1"].Value = "Age";

            using (ExcelRange range = workSheet.Cells["A1:H1"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                range.Style.Font.Color.SetColor(System.Drawing.Color.Green);
            }

            int row = 2;
            var list = await personRepository.GetAllAsync();
            var persons = list.Select(x => x.ToPersonResponse()).ToList();

            foreach (var item in persons)
            {
                // Cells [ row, column ]
                // Either Cells["A2"] or Cells[2, 1] || Cells["B3] or Cells[3, 2]
                workSheet.Cells[row, 1].Value = item.Name;
                workSheet.Cells[row, 2].Value = item.Email;
                workSheet.Cells[row, 3].Value = item.Age;
                row++;
            }

            workSheet.Cells[$"A1:H{row}"].AutoFitColumns();
            await excelPackage.SaveAsync();
        }

        memoryStream.Position = 0;

        return memoryStream;
    }
}
