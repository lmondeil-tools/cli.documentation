namespace MyApi;

[MyDocumentation(FirstInfo = "Hello", SecondInfo = "World")]
internal class MyRepository: IMyRepository
{

}

public interface IMyRepository { }

internal class MyService : IMyService
{
    private readonly IMyRepository _myRepository;

    MyService(IMyRepository myRepository)
    {
        _myRepository = myRepository;
    }
}

public interface IMyService { }