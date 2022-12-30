using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.File(@"log\LogFile.txt")
                    .WriteTo.Console()
                    .CreateLogger();