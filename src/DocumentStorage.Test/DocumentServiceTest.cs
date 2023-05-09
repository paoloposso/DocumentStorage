using DocumentStorage.Core;
using DocumentStorage.Document;
using DocumentStorage.User;
using Moq;

namespace DocumentStorage.Test;

public class DocumentServiceTest
{
    DocumentService? _service;

    [SetUp]
    public void Setup()
    {
        var repository = new Mock<IDocumentRepository>();
        var fileStorage = new Mock<IFileStorage>();

        repository
            .Setup(x => x.InsertDocumentMetadata(It.Is<DocumentMetadata>(p => p.Name.Equals("ValidDocumentName.pdf"))));

        repository
            .Setup(x => x.GetDocumentByIdForUser(It.Is<int>(p => p == 1), It.Is<int>(p => p == 1)))
            .ReturnsAsync(() => new DocumentMetadata {
                Id = 1, 
                Name = "ValidDocumentName.pdf", 
                Description = "Test Document", 
                CreatedByUser = 1,
                FilePath = "./Storage/ValidDocumentName.pdf",
            });

        _service = new DocumentService(repository.Object, fileStorage.Object);
    }

    [Test]
    public async Task ShouldGetDocumentMetadataForAllowedUser()
    {
        var documentMetadata = await _service!.GetDocumentMetadata(1, 1);

        Assert.IsNotNull(documentMetadata);
        Assert.IsTrue(documentMetadata.Name.Equals("ValidDocumentName.pdf"));
    }

    [Test]
    public async Task ShouldNotGetDocumentMetadataForAllowedUser()
    {
        try 
        {
            var _ = await _service!.GetDocumentMetadata(1, 2);

            Assert.Fail("Should Throw ArgumentException");
        }
        catch (ArgumentException)
        {
            Assert.Pass();
        }
    }
}