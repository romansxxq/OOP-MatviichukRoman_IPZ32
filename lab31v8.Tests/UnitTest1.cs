using Moq;
using lab31v8;
using Xunit;

namespace lab31v8.Tests;

public class UnitTest1
{
    [Fact]
    public void CreateReport_CallsGetDataAndGeneratePdf()
    {
        var mockDataSource = new Mock<IDataSource>();
        var mockPdfGenerator = new Mock<IPdfGenerator>();
        mockDataSource.Setup(ds => ds.GetData()).Returns("Test data");

        var service = new ReportService(mockDataSource.Object, mockPdfGenerator.Object);
        service.CreateReport();

        mockDataSource.Verify(ds => ds.GetData(), Times.Once);
        mockPdfGenerator.Verify(pg => pg.GeneratePdf("Test data"), Times.Once);
    }

    [Fact]
    public void CreateReport_GeneratePdfCalledWithCorrectData()
    {
        var mockDataSource = new Mock<IDataSource>();
        var mockPdfGenerator = new Mock<IPdfGenerator>();
        mockDataSource.Setup(ds => ds.GetData()).Returns("Special data");

        var service = new ReportService(mockDataSource.Object, mockPdfGenerator.Object);
        service.CreateReport();

        mockPdfGenerator.Verify(pg => pg.GeneratePdf("Special data"), Times.Once);
    }

    [Fact]
    public void CreateReport_DoesNotCallGeneratePdfIfNoData()
    {
        var mockDataSource = new Mock<IDataSource>();
        var mockPdfGenerator = new Mock<IPdfGenerator>();
        mockDataSource.Setup(ds => ds.GetData()).Returns(string.Empty);

        var service = new ReportService(mockDataSource.Object, mockPdfGenerator.Object);
        service.CreateReport();

        mockPdfGenerator.Verify(pg => pg.GeneratePdf(string.Empty), Times.Once);
    }

    [Fact]
    public void CreateReport_CallsGeneratePdfExactlyOnce()
    {
        var mockDataSource = new Mock<IDataSource>();
        var mockPdfGenerator = new Mock<IPdfGenerator>();
        mockDataSource.Setup(ds => ds.GetData()).Returns("Data");

        var service = new ReportService(mockDataSource.Object, mockPdfGenerator.Object);
        service.CreateReport();

        mockPdfGenerator.Verify(pg => pg.GeneratePdf(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void CreateReport_GetDataReturnsNull()
    {
        var mockDataSource = new Mock<IDataSource>();
        var mockPdfGenerator = new Mock<IPdfGenerator>();
        mockDataSource.Setup(ds => ds.GetData()).Returns((string)null);

        var service = new ReportService(mockDataSource.Object, mockPdfGenerator.Object);
        service.CreateReport();

        mockPdfGenerator.Verify(pg => pg.GeneratePdf(null), Times.Once);
    }

    [Fact]
    public void CreateReport_VerifyNoOtherCalls()
    {
        var mockDataSource = new Mock<IDataSource>();
        var mockPdfGenerator = new Mock<IPdfGenerator>();
        mockDataSource.Setup(ds => ds.GetData()).Returns("Data");

        var service = new ReportService(mockDataSource.Object, mockPdfGenerator.Object);
        service.CreateReport();

        mockDataSource.Verify(ds => ds.GetData(), Times.Once);
        mockPdfGenerator.Verify(pg => pg.GeneratePdf("Data"), Times.Once);
        mockPdfGenerator.VerifyNoOtherCalls();
    }

    [Fact]
    public void CreateReport_MultipleReportsCallsGeneratePdfMultipleTimes()
    {
        var mockDataSource = new Mock<IDataSource>();
        var mockPdfGenerator = new Mock<IPdfGenerator>();
        mockDataSource.Setup(ds => ds.GetData()).Returns("Multi");

        var service = new ReportService(mockDataSource.Object, mockPdfGenerator.Object);
        service.CreateReport();
        service.CreateReport();

        mockPdfGenerator.Verify(pg => pg.GeneratePdf("Multi"), Times.Exactly(2));
    }

    [Fact]
    public void CreateReport_SetupThrowsException()
    {
        var mockDataSource = new Mock<IDataSource>();
        var mockPdfGenerator = new Mock<IPdfGenerator>();
        mockDataSource.Setup(ds => ds.GetData()).Throws(new Exception("Data error"));

        var service = new ReportService(mockDataSource.Object, mockPdfGenerator.Object);
        Assert.Throws<Exception>(() => service.CreateReport());
        mockPdfGenerator.Verify(pg => pg.GeneratePdf(It.IsAny<string>()), Times.Never);
    }
}
