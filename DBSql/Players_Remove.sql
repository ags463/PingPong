-----------------------------------------------------------------------------------------------------
-- Read a single player
-----------------------------------------------------------------------------------------------------
-- Created: 08 Jan 2019, Alan G. Stewart
-----------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Players_Remove] (
	@PlayerID INTEGER
) AS BEGIN
	SET NOCOUNT ON;

	DELETE Players
	WHERE	( Players.PlayerID = @PlayerID )

	RETURN 0;
END;
