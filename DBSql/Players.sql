CREATE TABLE [dbo].[Players] (
    [PlayerID]   INT           IDENTITY (1, 1) NOT NULL,
    [FirstName]  VARCHAR (50)  NULL,
    [LastName]   VARCHAR (50)  NULL,
    [Age]        INT           NULL,
    [SkillLevel] VARCHAR (20)  NULL,
    [Email]      VARCHAR (128) NULL,
    CONSTRAINT [PK__Players__4A4E74A8495155EB] PRIMARY KEY CLUSTERED ([PlayerID] ASC)
);


