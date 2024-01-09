using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Processing;
using MarketOps.DataPump.Providers.PkoFunds.Exceptions;
using MarketOps.Tests.DataPump.Providers.PkoFunds.TestDataTools;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.DataDownload.Processing;

[TestFixture]
internal class PkoDataStreamReaderTests
{
    private const int ColumnsCount = 62;
    private readonly List<string> _testDataHeaderOnly = new()
    {
        "Data;PKO Obligacji Skarbowych Plus - fio;PKO Rynku Pieniężnego;PKO Obligacji Skarbowych Krótkoterminowy;PKO Obligacji Samorządowych;PKO Obligacji Samorządowych II;PKO Obligacji Uniwersalny;PKO Skarbowy Plus;PKO Obligacji;PKO Obligacji Skarbowych Długoterminowy;PKO Obligacji Rynku Polskiego;PKO Obligacji Globalny;PKO Papierów Dłużnych USD;PKO Dynamicznej Alokacji;PKO Stabilnego Wzrostu Plus;PKO Strategicznej Alokacji;PKO Zrównoważony;PKO Zrównoważony Plus;PKO Akcji;PKO Akcji Polskich Plus;PKO Akcji Nowa Europa;PKO Akcji Małych i Średnich Spółek;PKO Akcji Małych i Średnich Spółek Plus;PKO Dóbr Luksusowych Globalny;PKO Surowców Globalny;PKO Infrastruktury i Budownictwa Globalny;PKO Technologii i Innowacji Globalny;PKO Akcji Dywidendowych Globalny;PKO Medycyny i Demografii Globalny;PKO Ekologii i Odpowiedzialności Społecznej Globalny;PKO Akcji Strefy Euro;PKO Akcji Rynku Amerykańskiego;PKO Akcji Rynku Japońskiego;PKO Akcji Rynku Złota;PKO Akcji Rynku Polskiego;PKO Akcji Rynków Wschodzących;PKO Zabezpieczenia Emerytalnego 2020;PKO Zabezpieczenia Emerytalnego 2030;PKO Zabezpieczenia Emerytalnego 2040;PKO Zabezpieczenia Emerytalnego 2050;PKO Zabezpieczenia Emerytalnego 2060;PKO Zabezpieczenia Emerytalnego 2070;PKO Bursztynowy;PKO Szafirowy;PKO Rubinowy;PKO Szmaragdowy;PKO Diamentowy;PKO Akcji Rynku Azji i Pacyfiku;PKO Emerytura 2025;PKO Emerytura 2030;PKO Emerytura 2035;PKO Emerytura 2040;PKO Emerytura 2045;PKO Emerytura 2050;PKO Emerytura 2055;PKO Emerytura 2060;PKO Emerytura 2065;Fundusz Własności Pracowniczej PKP SFIO;Alfa SFIO;GAMMA;mFundusz Konserwatywny;PKO Stabilnego Wzrostu",
    };
    private readonly List<string> _testData = new()
    {
        "Data;PKO Obligacji Skarbowych Plus - fio;PKO Rynku Pieniężnego;PKO Obligacji Skarbowych Krótkoterminowy;PKO Obligacji Samorządowych;PKO Obligacji Samorządowych II;PKO Obligacji Uniwersalny;PKO Skarbowy Plus;PKO Obligacji;PKO Obligacji Skarbowych Długoterminowy;PKO Obligacji Rynku Polskiego;PKO Obligacji Globalny;PKO Papierów Dłużnych USD;PKO Dynamicznej Alokacji;PKO Stabilnego Wzrostu Plus;PKO Strategicznej Alokacji;PKO Zrównoważony;PKO Zrównoważony Plus;PKO Akcji;PKO Akcji Polskich Plus;PKO Akcji Nowa Europa;PKO Akcji Małych i Średnich Spółek;PKO Akcji Małych i Średnich Spółek Plus;PKO Dóbr Luksusowych Globalny;PKO Surowców Globalny;PKO Infrastruktury i Budownictwa Globalny;PKO Technologii i Innowacji Globalny;PKO Akcji Dywidendowych Globalny;PKO Medycyny i Demografii Globalny;PKO Ekologii i Odpowiedzialności Społecznej Globalny;PKO Akcji Strefy Euro;PKO Akcji Rynku Amerykańskiego;PKO Akcji Rynku Japońskiego;PKO Akcji Rynku Złota;PKO Akcji Rynku Polskiego;PKO Akcji Rynków Wschodzących;PKO Zabezpieczenia Emerytalnego 2020;PKO Zabezpieczenia Emerytalnego 2030;PKO Zabezpieczenia Emerytalnego 2040;PKO Zabezpieczenia Emerytalnego 2050;PKO Zabezpieczenia Emerytalnego 2060;PKO Zabezpieczenia Emerytalnego 2070;PKO Bursztynowy;PKO Szafirowy;PKO Rubinowy;PKO Szmaragdowy;PKO Diamentowy;PKO Akcji Rynku Azji i Pacyfiku;PKO Emerytura 2025;PKO Emerytura 2030;PKO Emerytura 2035;PKO Emerytura 2040;PKO Emerytura 2045;PKO Emerytura 2050;PKO Emerytura 2055;PKO Emerytura 2060;PKO Emerytura 2065;Fundusz Własności Pracowniczej PKP SFIO;Alfa SFIO;GAMMA;mFundusz Konserwatywny;PKO Stabilnego Wzrostu",
        "2023-11-24;;;2310,07;112,23;106,99;108,99;;;211,41;;93,96;106,32;159,23;;;;;;108,29;;336,34;;285,03;226,32;120,81;405,65;158,28;149,66;125,59;149,91;181,00;139,18;154,87;112,68;87,22;11,96;12,79;13,32;15,37;15,89;11,50;10,38;10,17;10,32;10,46;10,36;;11,24;11,94;12,66;12,69;13,34;13,38;13,23;13,19;11,13;526,11;;295,48;193,30;",
        "2023-11-23;;;2309,50;112,21;106,98;108,96;;;211,55;;94,01;106,02;159,18;;;;;;108,23;;337,29;;285,27;225,31;120,37;405,92;157,67;149,04;125,33;149,49;180,93;138,88;154,96;112,64;87,30;11,97;12,79;13,31;15,36;15,88;11,50;10,38;10,17;10,32;10,46;10,35;;11,24;11,95;12,67;12,70;13,35;13,39;13,24;13,20;11,13;526,04;;295,37;193,26;",
        "2023-11-22;;;2309,40;112,19;106,96;108,99;;;212,06;;94,11;106,56;159,34;;;;;;108,61;;336,18;;285,66;224,62;120,29;405,88;157,04;148,77;125,17;149,18;180,91;138,43;155,38;113,14;87,17;12,00;12,82;13,35;15,40;15,93;11,54;10,40;10,18;10,35;10,48;10,37;;11,26;11,97;12,69;12,72;13,37;13,41;13,26;13,22;11,15;527,16;3,71;295,42;193,39;",
        "2023-11-21;;;2310,00;112,16;106,94;108,99;;;212,43;;94,12;105,80;159,49;;;;;;108,48;;335,31;;283,93;226,17;119,75;403,90;157,08;148,39;124,67;148,42;180,12;137,29;154,84;113,04;87,34;12,02;12,83;13,35;15,39;15,91;11,52;10,41;10,19;10,34;10,47;10,35;;11,26;11,96;12,68;12,70;13,35;13,39;13,23;13,20;11,13;527,38;;295,43;193,35;",
        "2023-11-20;;;2309,91;112,14;106,92;108,97;;;212,21;;94,00;105,86;159,53;;;;;;108,68;;334,94;;285,26;226,55;119,63;405,76;157,62;147,91;125,15;148,81;180,48;137,64;152,11;113,18;87,53;12,01;12,83;13,36;15,42;15,95;11,55;10,40;10,19;10,34;10,49;10,37;;11,26;11,96;12,69;12,71;13,37;13,41;13,25;13,22;11,15;527,48;;295,40;193,23;",
    };

    [Test]
    public void Read_OnlyHeader__ReturnsNoData()
    {
        using var stream = DataStreamTools.CreateDataStream(_testDataHeaderOnly);

        var result = PkoDataStreamReader.Read(stream);

        result.ShouldNotBeNull();
        result.FundNameToIndex.Count.ShouldBe(ColumnsCount);
        result.DateToIndex.ShouldBeEmpty();
        result.Data.ShouldBeEmpty();
    }

    [Test]
    public void Read_WithData__ReturnsData()
    {
        using var stream = DataStreamTools.CreateDataStream(_testData);

        var result = PkoDataStreamReader.Read(stream);

        result.ShouldNotBeNull();
        result.FundNameToIndex.Count.ShouldBe(ColumnsCount);
        result.DateToIndex.Count.ShouldBe(5);
        result.Data.Length.ShouldBe(5);
        foreach (var item in result.Data)
            item.Length.ShouldBe(ColumnsCount);
    }

    [Test]
    public void Read_EmptyStream__Throws()
    {
        using var stream = new MemoryStream();

        Should.Throw<PkoFundsDataException>(() => PkoDataStreamReader.Read(stream));
    }
}
