using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class SqlLanguageDefinition : ILanguageDefinition
{
    public string Name => "SQL";
    
    private const int PrioComment = 100;
    private const int PrioString = 90;
    private const int PrioVariable = 85;
    private const int PrioQuotedIdentifier = 80;
    private const int PrioKeyword = 70;
    private const int PrioDataType = 65;
    private const int PrioFunction = 60;
    private const int PrioNumber = 50;
    private const int PrioOperator = 40;
    private const int PrioIdentifier = 10;
    private const int PrioPunctuation = 5;

    private static readonly List<TokenRule> Rules =
    [
        new("--.*", PrioComment, "comment sql-line-comment", null, RegexOptions.IgnoreCase),
        new(@"/\*[\s\S]*?\*/", PrioComment, "comment sql-block-comment", null,
            RegexOptions.Multiline | RegexOptions.IgnoreCase),
        new("N?'(?:''|[^'])*'", PrioString, "string sql-string", null, RegexOptions.IgnoreCase),
        new(@"@\w+", PrioVariable, "variable sql-variable", null),
        new(@":\w+", PrioVariable, "variable sql-bind-variable",
            null),
        new(@"\[[^\]]+\]", PrioQuotedIdentifier, "identifier sql-quoted-identifier bracket",
            null),
        new(@"""(?:""""|[^""])+""", PrioQuotedIdentifier, "identifier sql-quoted-identifier double-quote",
            null),
        new("`[^`]+`", PrioQuotedIdentifier, "identifier sql-quoted-identifier backtick",
            null),
        new(
            @"\b(ADD|ALL|ALTER|AND|ANY|AS|ASC|AUTHORIZATION|BACKUP|BEGIN|BETWEEN|BREAK|BROWSE|BULK|" +
            "BY|CASCADE|CASE|CHECK|CHECKPOINT|CLOSE|CLUSTERED|COALESCE|COLLATE|COLUMN|COMMIT|COMPUTE|CONSTRAINT|" +
            "CONTAINS|CONTAINSTABLE|CONTINUE|CONVERT|CREATE|CROSS|CURRENT|CURRENT_DATE|CURRENT_TIME|" +
            "CURRENT_TIMESTAMP|CURRENT_USER|CURSOR|DATABASE|DBCC|DEALLOCATE|DECLARE|DEFAULT|DELETE|DENY|DESC|" +
            "DISTINCT|DISTRIBUTED|DOUBLE|DROP|DUMP|ELSE|END|ERRLVL|ESCAPE|EXCEPT|EXEC|EXECUTE|EXISTS|EXIT|" +
            "EXTERNAL|FETCH|FILE|FILLFACTOR|FOR|FOREIGN|FREETEXT|FREETEXTTABLE|FROM|FULL|FUNCTION|GOTO|GRANT|" +
            "GROUP|HAVING|HOLDLOCK|IDENTITY|IDENTITY_INSERT|IDENTITYCOL|IF|IN|INDEX|INNER|INSERT|INTERSECT|" +
            "INTO|IS|JOIN|KEY|KILL|LEFT|LIKE|LIMIT|LINENO|LOAD|MERGE|NATIONAL|NOCHECK|NONCLUSTERED|NOT|NULL|NULLIF|" +
            "OF|OFF|OFFSETS|ON|OPEN|OPENDATASOURCE|OPENQUERY|OPENROWSET|OPENXML|OPTION|OR|ORDER|OUTER|OVER|" +
            "PERCENT|PLAN|PRECISION|PRIMARY|PRINT|PROC|PROCEDURE|PUBLIC|RAISERROR|READ|READTEXT|RECONFIGURE|" +
            "REFERENCES|REPLICATION|RESTORE|RESTRICT|RETURN|REVERT|REVOKE|RIGHT|ROLLBACK|ROWCOUNT|ROWGUIDCOL|" +
            "RULE|SAVE|SCHEMA|SECURITYAUDIT|SELECT|SEMANTICKEYPHRASETABLE|SEMANTICSIMILARITYDETAILSTABLE|" +
            "SEMANTICSIMILARITYTABLE|SESSION_USER|SET|SETUSER|SHUTDOWN|SOME|STATISTICS|SYSTEM_USER|TABLE|" +
            "TABLESAMPLE|TEXTSIZE|THEN|TO|TOP|TRAN|TRANSACTION|TRIGGER|TRUNCATE|TRY_CONVERT|TSEQUAL|UNION|UNIQUE|" +
            "UNPIVOT|UPDATE|UPDATETEXT|USE|USER|VALUES|VARYING|VIEW|WAITFOR|WHEN|WHERE|WHILE|WITH|WITHIN GROUP|" +
            @"WRITETEXT|OFFSET|FETCH NEXT|FETCH FIRST|ROWS ONLY)\b",
            PrioKeyword, "keyword sql-keyword", null, RegexOptions.IgnoreCase),
        new(
            @"\b(BIGINT|BINARY|BIT|BLOB|BOOL|BOOLEAN|CHAR|CHARACTER|CLOB|DATE|DATETIME|DATETIME2|DATETIMEOFFSET|" +
            "DEC|DECIMAL|DOUBLE PRECISION|ENUM|FLOAT|IMAGE|INT|INTEGER|INTERVAL|JSON|LONGBLOB|LONGTEXT|" +
            "MEDIUMBLOB|MEDIUMINT|MEDIUMTEXT|MONEY|NCHAR|NTEXT|NUMBER|NUMERIC|NVARCHAR|REAL|SMALLDATETIME|" +
            "SMALLINT|SMALLMONEY|SQL_VARIANT|TEXT|TIME|TIMESTAMP|TINYBLOB|TINYINT|TINYTEXT|UNIQUEIDENTIFIER|VARBINARY|" +
            @"VARCHAR|XML|YEAR)\b",
            PrioDataType, "type sql-datatype", null, RegexOptions.IgnoreCase),
        new(
            @"\b(AVG|CHECKSUM_AGG|COUNT|COUNT_BIG|GROUPING|GROUPING_ID|MAX|MIN|SUM|STDEV|STDEVP|VAR|VARP|" +
            "CAST|CONVERT|PARSE|TRY_CAST|TRY_CONVERT|TRY_PARSE|" +
            "ABS|ACOS|ASIN|ATAN|ATN2|CEILING|COS|COT|DEGREES|EXP|FLOOR|LOG|LOG10|PI|POWER|RADIANS|RAND|ROUND|SIGN|SIN|SQRT|SQUARE|TAN|" +
            "ASCII|CHAR|CHARINDEX|CONCAT|CONCAT_WS|DIFFERENCE|FORMAT|LEFT|LEN|LOWER|LTRIM|NCHAR|PATINDEX|QUOTENAME|REPLACE|REPLICATE|REVERSE|RIGHT|RTRIM|SOUNDEX|SPACE|STR|STUFF|SUBSTRING|TRANSLATE|TRIM|UNICODE|UPPER|" +
            "DATEADD|DATEDIFF|DATEFROMPARTS|DATENAME|DATEPART|DAY|GETDATE|GETUTCDATE|ISDATE|MONTH|SMALLDATETIMEFROMPARTS|SYSDATETIME|SYSDATETIMEOFFSET|SYSUTCDATETIME|TIMEFROMPARTS|YEAR|" +
            "CHOOSE|IIF|ISNULL|NULLIF|SESSION_USER|SYSTEM_USER|USER_NAME|SUSER_ID|SUSER_NAME|HOST_ID|HOST_NAME|DB_ID|DB_NAME|OBJECT_ID|OBJECT_NAME|ROW_NUMBER|RANK|DENSE_RANK|NTILE|LAG|LEAD|FIRST_VALUE|LAST_VALUE" +
            @")\b(?=\s*\()",
            PrioFunction, "function sql-function", null, RegexOptions.IgnoreCase),
        new(@"\b\d+(\.\d+)?([eE][+-]?\d+)?\b", PrioNumber, "number sql-number", null),
        new(@"0x[0-9a-fA-F]+\b", PrioNumber, "number sql-hex-number", null,
            RegexOptions.IgnoreCase),
        new(@"[+*/%&|\-^<>=!]=?|<>|!>|!<", PrioOperator, "operator sql-operator",
            null),
        new(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", PrioIdentifier, "identifier sql-identifier", null),
        new("[().,;]", PrioPunctuation, "punctuation sql-punctuation", null)

    ];

    public IEnumerable<TokenRule> GetRules() => Rules;
}