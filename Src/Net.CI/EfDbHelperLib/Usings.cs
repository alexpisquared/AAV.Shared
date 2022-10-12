global using System;
global using System.Collections.Generic;
global using System.Data;
global using System.Data.Common;
global using System.Data.Entity.Infrastructure;
global using System.Data.Entity.Validation;
global using System.Diagnostics;
global using System.Dynamic;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Threading.Tasks;
global using StandardLib.Extensions;
global using StandardLib.Helpers;
global using Microsoft.Data.SqlClient;
global using Microsoft.EntityFrameworkCore; // <== instead of  System.Data.Entity  or System.InvalidOperationException: The source IQueryable doesn't implement IDbAsyncEnumerable. Only sources that implement IDbAsyncEnumerable can be used for Entity Framework asynchronous operations. For more details see http://go.microsoft.com/fwlink/?LinkId=287068. ==> https://stackoverflow.com/questions/26296091/idbasyncenumerable-not-implemented ==> use 
global using Microsoft.Extensions.Logging;
global using static System.Diagnostics.Trace;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using StandardContractsLib;

