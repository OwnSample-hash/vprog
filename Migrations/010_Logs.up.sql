CREATE VIEW LogsView AS
SELECT
	l.Id,
	u.Username AS "User",
	l.Description,
	ll.Name AS Level,
	l.Source,
	l.Line,
	l.Date
	FROM Logs l
	LEFT JOIN Users u ON l.UserId = u.Id
		LEFT JOIN LogLevels ll ON l.LevelId = ll.Id;