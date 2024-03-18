CREATE TABLE [dbo].[WeatherData] (
    [Id]            INT IDENTITY (1, 1) NOT NULL,
    [Year]          INT NOT NULL,
    [Month]         INT NOT NULL,
    [Day]           INT NOT NULL,
    [MaxTemp]       FLOAT NOT NULL,
    [MeanTemp]      FLOAT NOT NULL,
    [MinTemp]       FLOAT NOT NULL,
    [Precipitation] FLOAT NULL,
    [SunshineHours] FLOAT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
