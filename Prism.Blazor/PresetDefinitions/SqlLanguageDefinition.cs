using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class SqlLanguageDefinition : ILanguageDefinition
{
    public string Name => "SQL";

    // Styles (inspired by common SQL editor themes)
    private const string CommentStyle = "color: #008000;"; // Green
    private const string StringStyle = "color: #A31515;"; // Red
    private const string NumberStyle = "color: #098658;"; // Dark Cyan/Green
    private const string KeywordStyle = "color: #0000FF; font-weight: bold;"; // Blue, Bold
    private const string DataTypeStyle = "color: #2B91AF;"; // Teal
    private const string FunctionStyle = "color: #795E26;"; // Brown/Dark Yellow
    private const string OperatorStyle = "color: #808080;"; // Grey
    private const string PunctuationStyle = "color: #333333;"; // Dark Grey
    private const string IdentifierStyle = "color: #808080;"; // Grey
    private const string QuotedIdentifierStyle = "color: #795E26;"; // Brown (like functions, for [Table], "Table")
    private const string VariableStyle = "color: #C586C0;"; // Magenta (like TS keywords)

    // Priorities
    private const int PrioComment = 100;
    private const int PrioString = 90;
    private const int PrioVariable = 85; // For @variable
    private const int PrioQuotedIdentifier = 80;
    private const int PrioKeyword = 70;
    private const int PrioDataType = 65;
    private const int PrioFunction = 60;
    private const int PrioNumber = 50;
    private const int PrioOperator = 40;
    private const int PrioIdentifier = 10; // General unquoted identifiers
    private const int PrioPunctuation = 5;

    private static readonly List<TokenRule> Rules =
    [
        new("--.*", PrioComment, "comment sql-line-comment", CommentStyle, RegexOptions.IgnoreCase),
        new(@"/\*[\s\S]*?\*/", PrioComment, "comment sql-block-comment", CommentStyle,
            RegexOptions.Multiline | RegexOptions.IgnoreCase),

        // 2. String Literals (single quotes, '' for escape)
        new("N?'(?:''|[^'])*'", PrioString, "string sql-string", StringStyle, RegexOptions.IgnoreCase),
        // Some dialects might use double quotes for strings, but more commonly for identifiers.
        // If double-quoted strings are needed: new TokenRule(@"""(?:""""|[^""])*""", PrioString, "string", StringStyle),


        // 3. Variables/Parameters (e.g., T-SQL @variable, Oracle :variable)
        new(@"@\w+", PrioVariable, "variable sql-variable", VariableStyle),
        new(@":\w+", PrioVariable, "variable sql-bind-variable",
            VariableStyle), // For Oracle/Postgres bind vars

        // 4. Quoted Identifiers (SQL Server [], ANSI "", MySQL ``)
        new(@"\[[^\]]+\]", PrioQuotedIdentifier, "identifier sql-quoted-identifier bracket",
            QuotedIdentifierStyle),
        new(@"""(?:""""|[^""])+""", PrioQuotedIdentifier, "identifier sql-quoted-identifier double-quote",
            QuotedIdentifierStyle), // Handles escaped double quotes if used inside
        new("`[^`]+`", PrioQuotedIdentifier, "identifier sql-quoted-identifier backtick",
            QuotedIdentifierStyle),

        // 5. Keywords (Common ANSI SQL keywords + some T-SQL/others)
        // This list can be very extensive.
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
            PrioKeyword, "keyword sql-keyword", KeywordStyle, RegexOptions.IgnoreCase),

        // 6. Data Types

        new(
            @"\b(BIGINT|BINARY|BIT|BLOB|BOOL|BOOLEAN|CHAR|CHARACTER|CLOB|DATE|DATETIME|DATETIME2|DATETIMEOFFSET|" +
            "DEC|DECIMAL|DOUBLE PRECISION|ENUM|FLOAT|IMAGE|INT|INTEGER|INTERVAL|JSON|LONGBLOB|LONGTEXT|" +
            "MEDIUMBLOB|MEDIUMINT|MEDIUMTEXT|MONEY|NCHAR|NTEXT|NUMBER|NUMERIC|NVARCHAR|REAL|SMALLDATETIME|" +
            "SMALLINT|SMALLMONEY|SQL_VARIANT|TEXT|TIME|TIMESTAMP|TINYBLOB|TINYINT|TINYTEXT|UNIQUEIDENTIFIER|VARBINARY|" +
            @"VARCHAR|XML|YEAR)\b",
            PrioDataType, "type sql-datatype", DataTypeStyle, RegexOptions.IgnoreCase),

        // 7. Built-in Functions (Common ones)
        // This is not exhaustive and highly dialect-dependent.

        new(
            @"\b(AVG|CHECKSUM_AGG|COUNT|COUNT_BIG|GROUPING|GROUPING_ID|MAX|MIN|SUM|STDEV|STDEVP|VAR|VARP|" + // Aggregate
            "CAST|CONVERT|PARSE|TRY_CAST|TRY_CONVERT|TRY_PARSE|" + // Conversion
            "ABS|ACOS|ASIN|ATAN|ATN2|CEILING|COS|COT|DEGREES|EXP|FLOOR|LOG|LOG10|PI|POWER|RADIANS|RAND|ROUND|SIGN|SIN|SQRT|SQUARE|TAN|" + // Math
            "ASCII|CHAR|CHARINDEX|CONCAT|CONCAT_WS|DIFFERENCE|FORMAT|LEFT|LEN|LOWER|LTRIM|NCHAR|PATINDEX|QUOTENAME|REPLACE|REPLICATE|REVERSE|RIGHT|RTRIM|SOUNDEX|SPACE|STR|STUFF|SUBSTRING|TRANSLATE|TRIM|UNICODE|UPPER|" + // String
            "DATEADD|DATEDIFF|DATEFROMPARTS|DATENAME|DATEPART|DAY|GETDATE|GETUTCDATE|ISDATE|MONTH|SMALLDATETIMEFROMPARTS|SYSDATETIME|SYSDATETIMEOFFSET|SYSUTCDATETIME|TIMEFROMPARTS|YEAR|" + // Date/Time
            "CHOOSE|IIF|ISNULL|NULLIF|SESSION_USER|SYSTEM_USER|USER_NAME|SUSER_ID|SUSER_NAME|HOST_ID|HOST_NAME|DB_ID|DB_NAME|OBJECT_ID|OBJECT_NAME|ROW_NUMBER|RANK|DENSE_RANK|NTILE|LAG|LEAD|FIRST_VALUE|LAST_VALUE" +
            @")\b(?=\s*\()", // Ensure it's followed by a parenthesis for function call
            PrioFunction, "function sql-function", FunctionStyle, RegexOptions.IgnoreCase),

        // 8. Numbers (integers, decimals, scientific notation)

        new(@"\b\d+(\.\d+)?([eE][+-]?\d+)?\b", PrioNumber, "number sql-number", NumberStyle),
        new(@"0x[0-9a-fA-F]+\b", PrioNumber, "number sql-hex-number", NumberStyle,
            RegexOptions.IgnoreCase), // Hex numbers

        // 9. Operators
        new(@"[+*/%&|\-^<>=!]=?|<>|!>|!<", PrioOperator, "operator sql-operator",
            OperatorStyle), // Arithmetic, Bitwise, Comparison
        // Logical operators often covered by keywords (AND, OR, NOT, LIKE, IN, BETWEEN, IS) but if not:
        // new TokenRule(@"\b(AND|OR|NOT|LIKE|IN|BETWEEN|IS)\b", PrioOperator, "operator sql-logical-operator", OperatorStyle, RegexOptions.IgnoreCase),

        // 10. Unquoted Identifiers (fallback, must be after keywords, functions, etc.)
        // Allows underscores and alphanumeric characters. Some dialects allow $ or # in unquoted identifiers.
        new(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", PrioIdentifier, "identifier sql-identifier", IdentifierStyle),

        // 11. Punctuation
        new("[().,;]", PrioPunctuation, "punctuation sql-punctuation", PunctuationStyle)

    ];

    public IEnumerable<TokenRule> GetRules() => Rules;
}