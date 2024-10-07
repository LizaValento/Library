

public class AuthorRepositoryTests
{
    private Context CreateContext()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        return new Context(options);
    }

    [Fact]
    public void Add_ShouldAddAuthor()
    {
        
        using var context = CreateContext();
        var repository = new AuthorRepository(context);
        var author = new Author { FirstName = "John", LastName = "Doe" , DateOfBirth = new DateTime(1990, 1, 1) , Country = "Belarus"};

        
        repository.Add(author);
        context.SaveChanges();

        
        var addedAuthor = context.Authors.FirstOrDefault(a => a.FirstName == "John" && a.LastName == "Doe");
        Assert.NotNull(addedAuthor);
        Assert.Equal("John", addedAuthor.FirstName);
        Assert.Equal("Doe", addedAuthor.LastName);
    }

    [Fact]
    public void GetById_ShouldReturnAuthor_WhenExists()
    {
        
        using var context = CreateContext();
        var repository = new AuthorRepository(context);
        var author = new Author { FirstName = "Jane", LastName = "Doe" };
        context.Authors.Add(author);
        context.SaveChanges(); 

        
        var result = repository.GetById(author.Id);

        
        Assert.NotNull(result);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenAuthorDoesNotExist()
    {
        
        using var context = CreateContext();
        var repository = new AuthorRepository(context);

        
        var result = repository.GetById(999);

        
        Assert.Null(result);
    }
}
