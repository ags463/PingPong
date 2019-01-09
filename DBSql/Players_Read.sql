-----------------------------------------------------------------------------------------------------
-- Read a single player
-----------------------------------------------------------------------------------------------------
-- Created: 08 Jan 2019, Alan G. Stewart
-----------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Players_Read] (
	@PlayerID INTEGER
) AS BEGIN
	SET NOCOUNT ON;

	SELECT * 
	FROM Players
	WHERE	( Players.PlayerID = @PlayerID )

	RETURN 0;
END;
