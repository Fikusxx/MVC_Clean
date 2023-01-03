using AutoFixture;
using Clean.Core.Domain.Entities;
using Clean.Core.Domain.RepositoryContracts;
using Clean.Core.DTO.PersonDTO;
using Clean.Core.Enums;
using Clean.Core.ServiceContracts;
using Clean.Core.Services;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace Clean.ServiceTests;


public class PersonServiceTest
{
    private readonly IPersonGetterService personGetterService;
    private readonly IPersonAdderService personAdderService;
    private readonly IPersonUpdaterService personUpdaterService;
    private readonly IPersonDeleterService personDeleterService;
    private readonly IPersonSorterService personSorterService;
    private readonly IFixture fixture = new Fixture();
    private readonly IPersonRepository personRepository;
    private readonly Mock<IPersonRepository> personMock;

    public PersonServiceTest()
    {
        personMock = new Mock<IPersonRepository>();
        personRepository = personMock.Object;
        personGetterService = new PersonGetterService(personRepository, null);
        personAdderService = new PersonAdderService(personRepository, null);
        personUpdaterService = new PersonUpdaterService(personRepository, null);
        personDeleterService = new PersonDeleterService(personRepository, null);
        personSorterService = new PersonSorterService(personRepository, null);
    }

    #region Add Person

    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
        Func<Task> action = async () =>
        {
            await personAdderService.AddPersonAsync(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
        //await Assert.ThrowsAsync<ArgumentNullException>(async () => await personService.AddPersonAsync(null));
    }

    [Fact]
    public async Task AddPerson_NullPersonName_ToBeArgumentException()
    {
        var request = fixture.Build<PersonAddRequest>().With(x => x.Name, null as string).Create();
        var person = request.ToPerson();

        //
        personMock.Setup(x => x.AddAsync(It.IsAny<Person>())).ReturnsAsync(person);

        Func<Task> action = async () =>
        {
            await personAdderService.AddPersonAsync(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
        //await Assert.ThrowsAsync<ArgumentException>(async () => await personService.AddPersonAsync(request));
    }

    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
    {
        PersonAddRequest request = fixture.Build<PersonAddRequest>()
            .With(x => x.Email, "example@gmail.com")
            .With(x => x.Address, "Groove St.")
            .Create();

        var person = request.ToPerson();
        var personResponse = person.ToPersonResponse();

        // Setup mock method to be executed properly.
        // Mocking AddAsync method, which accepts object of type Person and returns fixed person
        personMock.Setup(x => x.AddAsync(It.IsAny<Person>())).ReturnsAsync(person);

        // Add person and assign it's newly created personId to potenitially equal object we compare
        var response = await personAdderService.AddPersonAsync(request);
        personResponse.PersonId = response.PersonId;

        // Tests
        response.PersonId.Should().NotBe(Guid.Empty);
        response.Should().Be(personResponse);
    }

    #endregion

    #region Get Person By Id

    [Fact]
    public async Task GetPersonById_NullPersonId_ToBeNull()
    {
        var personResponse = await personGetterService.GetPersonByIdAsync(null);

        personResponse.Should().BeNull(); // NotBeNull()
        //Assert.Null(personResponse);
    }

    [Fact]
    public async Task GetPersonById_ProperPersonDetails_ToBeSuccessful()
    {
        var person = fixture.Build<Person>().With(x => x.Email, "example@gmail.com")
            .With(temp => temp.Country, null as Country)
            .Create();
        var personResponse = person.ToPersonResponse();

        // Mock
        personMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(person);

        var response = await personGetterService.GetPersonByIdAsync(person.PersonId);

        personResponse.Should().Be(response);
    }

    #endregion

    #region Get All Persons

    [Fact]
    public async Task GetAllPersons_EmptyList_ToBeEmptyList()
    {
        var persons = new List<Person>();
        personMock.Setup(x => x.GetAllAsync()).ReturnsAsync(persons);


        var responseList = await personGetterService.GetAllPersonsAsync();

        responseList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPersons_WithFewPersons_ToBeSuccesful()
    {
        var personList = new List<Person>()
        {
            fixture.Build<Person>().With(x => x.Email, "1@gmail.com").With(x => x.Country, null as Country).Create(),
            fixture.Build<Person>().With(x => x.Email, "2@gmail.com").With(x => x.Country, null as Country).Create(),
            fixture.Build<Person>().With(x => x.Email, "3@gmail.com").With(x => x.Country, null as Country).Create(),
        };

        var personResponseList = personList.Select(x => x.ToPersonResponse()).ToList();

        personMock.Setup(x => x.GetAllAsync()).ReturnsAsync(personList);

        var response = await personGetterService.GetAllPersonsAsync();

        response.Should().BeEquivalentTo(personResponseList);
    }

    #endregion

    #region Get Filtered Persons

    [Fact]
    public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
    {
        var personList = new List<Person>()
        {
            fixture.Build<Person>().With(x => x.Email, "1@gmail.com").With(x => x.Country, null as Country).Create(),
            fixture.Build<Person>().With(x => x.Email, "2@gmail.com").With(x => x.Country, null as Country).Create(),
            fixture.Build<Person>().With(x => x.Email, "3@gmail.com").With(x => x.Country, null as Country).Create(),
        };

        var personResponseList = personList.Select(x => x.ToPersonResponse()).ToList();

        personMock.Setup(x => x.GetFilteredPersonsAsync(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(personList);

        var searchList = await personGetterService.GetFilteredPersonsAsync(nameof(Person.Name), "");

        searchList.Should().BeEquivalentTo(personResponseList);
    }

    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
    {
        var personList = new List<Person>()
        {
            fixture.Build<Person>().With(x => x.Email, "1@gmail.com").With(x => x.Country, null as Country).Create(),
            fixture.Build<Person>().With(x => x.Email, "2@gmail.com").With(x => x.Country, null as Country).Create(),
            fixture.Build<Person>().With(x => x.Email, "3@gmail.com").With(x => x.Country, null as Country).Create(),
        };

        var personResponseList = personList.Select(x => x.ToPersonResponse()).ToList();

        personList = personList.Where(x => x.Name.Contains("a")).ToList();
        personResponseList = personResponseList.Where(x => x.Name.Contains("a")).ToList();

        personMock.Setup(x => x.GetFilteredPersonsAsync(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(personList);

        var searchList = await personGetterService.GetFilteredPersonsAsync(nameof(Person.Name), "a");

        searchList.Should().BeEquivalentTo(personResponseList);
    }

    #endregion

    #region Get Sorted Persons

    [Fact]
    public async Task GetSortedPersons_ToBeSuccessful()
    {
        var personList = new List<Person>()
        {
            fixture.Build<Person>().With(x => x.Email, "1@gmail.com").With(x => x.Country, null as Country).Create(),
            fixture.Build<Person>().With(x => x.Email, "2@gmail.com").With(x => x.Country, null as Country).Create(),
            fixture.Build<Person>().With(x => x.Email, "3@gmail.com").With(x => x.Country, null as Country).Create(),
        };

        var list = personList.Select(x => x.ToPersonResponse()).ToList();
        var sortedList = await personSorterService.GetSortedPersonsAsync(list, nameof(Person.Name), SortOrderOptions.DESC);

        sortedList.Should().BeInDescendingOrder(x => x.Name);
    }

    #endregion

    #region Update Person

    [Fact]
    public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
    {
        Func<Task> action = async () =>
        {
            await personUpdaterService.UpdatePersonAsync(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
        //await Assert.ThrowsAsync<ArgumentNullException>(async () => await personService.UpdatePersonAsync(null));
    }

    [Fact]
    public async Task UpdatePerson_InvalidPersonId_ToBeArgumentException()
    {
        var updateRequest = fixture.Build<PersonUpdateRequest>().Create();

        Func<Task> action = async () =>
        {
            await personUpdaterService.UpdatePersonAsync(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
        //await Assert.ThrowsAsync<ArgumentException>(async () => await personService.UpdatePersonAsync(updateRequest));
    }

    [Fact]
    public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
    {
        var person = fixture.Build<Person>().With(x => x.Email, "bob@gmail.com")
                                  .With(x => x.Country, null as Country)
                                  .With(x => x.Name, null as string)
                                  .With(x => x.Gender, "Male")
                                  .Create();

        var updateRequest = person.ToPersonResponse().ToPersonUpdateRequest();

        Func<Task> action = async () =>
        {
            await personUpdaterService.UpdatePersonAsync(updateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
        //await Assert.ThrowsAsync<ArgumentException>(async () => await personService.UpdatePersonAsync(updateRequest));
    }

    [Fact]
    public async Task UpdatePerson_PersonFullDetailsUpdate_ToBeSuccessful()
    {
        var person = fixture.Build<Person>().With(x => x.Email, "tom@gmail.com")
                                  .With(x => x.Country, null as Country)
                                  .With(x => x.Gender, "Male")
                                  .Create();

        var response = person.ToPersonResponse();
        var updateRequest = response.ToPersonUpdateRequest();

        personMock.Setup(x => x.UpdateAsync(It.IsAny<Person>())).ReturnsAsync(person);
        personMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(person);

        var updatedPersonResponse = await personUpdaterService.UpdatePersonAsync(updateRequest);

        response.Should().Be(updatedPersonResponse);
        //Assert.Equal(person, updatedPersonResponse);
    }
    #endregion

    #region Delete Person

    [Fact]
    public async Task DeletePerson_NullGuid_ToBeArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await personDeleterService.DeletePersonAsync(null));
    }

    [Fact]
    public async Task DeletePerson_ValidGuid()
    {
        var person = fixture.Build<Person>().With(x => x.Email, "bob@gmail.com")
                                  .With(x => x.Country, null as Country).Create();

        personMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(person);
        personMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        var isDeleted = await personDeleterService.DeletePersonAsync(person.PersonId);

        isDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeletePerson_InvalidGuid()
    {
        var isDeleted = await personDeleterService.DeletePersonAsync(Guid.NewGuid());

        isDeleted.Should().BeFalse();
        //Assert.False(isDeleted);
    }

    #endregion
}