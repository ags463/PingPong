-----------------------------------------------------------------------------------------------------
-- Create/Update a single player
-----------------------------------------------------------------------------------------------------
-- Created: 08 Jan 2019, Alan G. Stewart
-----------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Players_Save] (
	@PlayerID   INTEGER,
	@FirstName  VARCHAR(50),
	@LastName   VARCHAR(50),
	@Age        INTEGER,
	@SkillLevel VARCHAR(20),
	@Email      VARCHAR(128)
) AS BEGIN
	SET NOCOUNT ON;

	IF ( @PlayerID IS NOT NULL ) BEGIN
		UPDATE Players SET
			FirstName  = @FirstName,
			LastName   = @LastName,
			Age        = @Age,
			SkillLevel = @SkillLevel,
			Email      = @Email
		WHERE	( PlayerID = @PlayerID );
	END ELSE BEGIN
		INSERT Players (
			FirstName, LastName, Age, SkillLevel, Email
		) VALUES (
			@FirstName, @LastName, @Age, @SkillLevel, @Email
		);
	END;

	RETURN 0;
END;
