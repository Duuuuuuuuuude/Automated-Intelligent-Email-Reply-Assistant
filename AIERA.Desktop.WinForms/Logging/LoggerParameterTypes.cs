// This contains the rules for what types the log parameters should be. Every log parameter should have a corresponding rule here.
//https://www.meziantou.net/roslyn-analyzer-to-check-the-types-of-structured-log-messages.htm

using Meziantou.Analyzer.Annotations;

[assembly: StructuredLogField("test", typeof(string))]
[assembly: StructuredLogField("Test", typeof(string))]
