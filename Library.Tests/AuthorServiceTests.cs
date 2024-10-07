using Business.Repositories.Interfaces;

public class AuthorServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthorService _authorService;

    public AuthorServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        var authorsMock = new Mock<IAuthorRepository>();
        _unitOfWorkMock.Setup(uow => uow.Authors).Returns(authorsMock.Object);

        _mapperMock.Setup(m => m.Map<Author>(It.IsAny<AuthorModel>()))
                   .Returns((AuthorModel model) => new Author
                   {
                       Id = model.Id,
                       FirstName = model.FirstName,
                       LastName = model.LastName,
                       DateOfBirth = DateTime.TryParse(model.DateOfBirth, out var dob) ? dob : (DateTime?)null,
                       Country = model.Country
                   });

        _authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public void GetAuthorByIdForBooks_ShouldReturnAuthorModel_WhenAuthorExists()
    {
        var author = new Author { Id = 1, FirstName = "John", LastName = "Doe" };
        var authorModel = new AuthorModel { Id = 1, FirstName = "John", LastName = "Doe" };
        var books = new List<Book>(); 

        _unitOfWorkMock.Setup(uow => uow.Authors.GetById(1)).Returns(author);
        _unitOfWorkMock.Setup(uow => uow.Books.GetBooksByAuthorId(1)).Returns(books);
        _mapperMock.Setup(m => m.Map<AuthorModel>(author)).Returns(authorModel);

        int totalBooks;

        var result = _authorService.GetAuthorByIdForBooks(1, 1, 10, out totalBooks);

        Assert.NotNull(result);
        Assert.Equal(authorModel.FirstName, result.FirstName);
        Assert.Equal(0, totalBooks); 
    }

    [Fact]
    public void GetAuthorById_ShouldReturnAuthorModel_WhenAuthorExists()
    {
        var author = new Author { Id = 1, FirstName = "John", LastName = "Doe" };
        var authorModel = new AuthorModel { Id = 1, FirstName = "John", LastName = "Doe" };

        _unitOfWorkMock.Setup(uow => uow.Authors.GetById(1)).Returns(author);
        _mapperMock.Setup(m => m.Map<AuthorModel>(author)).Returns(authorModel);

        var result = _authorService.GetAuthorById(1);

        Assert.NotNull(result);
        Assert.Equal(authorModel.FirstName, result.FirstName);
    }

    [Fact]
    public void GetAuthors_ShouldReturnListOfAuthors()
    {
            var authors = new List<Author>
        {
            new Author { Id = 1, FirstName = "John", LastName = "Doe" },
            new Author { Id = 2, FirstName = "Jane", LastName = "Doe" }
        };
            var authorModels = new List<AuthorModel>
        {
            new AuthorModel { Id = 1, FirstName = "John", LastName = "Doe" },
            new AuthorModel { Id = 2, FirstName = "Jane", LastName = "Doe" }
        };

        _unitOfWorkMock.Setup(uow => uow.Authors.GetAll()).Returns(authors);
        _mapperMock.Setup(m => m.Map<List<AuthorModel>>(authors)).Returns(authorModels);

        var result = _authorService.GetAuthors();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetAuthorsPagination_ShouldReturnPaginatedAuthors()
    {
            var authors = new List<Author>
        {
            new Author { Id = 1, FirstName = "John", LastName = "Doe" },
            new Author { Id = 2, FirstName = "Jane", LastName = "Doe" }
        };
            var authorModels = new List<AuthorModel>
        {
            new AuthorModel { Id = 1, FirstName = "John", LastName = "Doe" },
            new AuthorModel { Id = 2, FirstName = "Jane", LastName = "Doe" }
        };

        _unitOfWorkMock.Setup(uow => uow.Authors.GetAll()).Returns(authors);
        _mapperMock.Setup(m => m.Map<List<AuthorModel>>(authors)).Returns(authorModels);

        var result = _authorService.GetAuthorsPagination(1, 2);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Authors.Count);
    }

    [Fact]
    public void AddAuthor_ShouldAddAuthor()
    {
        var authorModel = new AuthorModel
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = "1990-01-01",
            Country = "Belarus"
        };

        _authorService.AddAuthor(authorModel);

        _unitOfWorkMock.Verify(uow => uow.Authors.Add(It.IsAny<Author>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Complete(), Times.Once);
    }


    [Fact]
    public void UpdateAuthor_ShouldUpdateAuthor()
    {
        var authorModel = new AuthorModel { Id = 1, FirstName = "John", LastName = "Doe" };
        var authorEntity = new Author { Id = 1, FirstName = "John", LastName = "Doe" };

        _mapperMock.Setup(m => m.Map<Author>(authorModel)).Returns(authorEntity);

        _authorService.UpdateAuthor(authorModel);

        _unitOfWorkMock.Verify(uow => uow.Authors.Update(authorEntity), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Complete(), Times.Once);
    }

    [Fact]
    public void DeleteAuthor_ShouldRemoveAuthor_WhenAuthorExists()
    {
        
        var author = new Author { Id = 1, FirstName = "John", LastName = "Doe" };
        var books = new List<Book>(); 

        _unitOfWorkMock.Setup(uow => uow.Authors.GetById(1)).Returns(author);
        _unitOfWorkMock.Setup(uow => uow.Books.GetBooksByAuthorId(1)).Returns(books);

        
        _authorService.DeleteAuthor(1);

        
        _unitOfWorkMock.Verify(uow => uow.Books.Remove(It.IsAny<Book>()), Times.Never); 
        _unitOfWorkMock.Verify(uow => uow.Authors.Remove(author), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Complete(), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateAuthorAsync_ShouldReturnExistingAuthorId_WhenExists()
    {
        
        var existingAuthorId = 1;
        _unitOfWorkMock.Setup(uow => uow.Authors.GetOrCreateAuthorAsync("John", "Doe"))
            .ReturnsAsync(existingAuthorId);

        
        var result = await _authorService.GetOrCreateAuthorAsync("John", "Doe");

        
        Assert.Equal(existingAuthorId, result);
    }

}