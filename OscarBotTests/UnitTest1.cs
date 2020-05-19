using System;
using System.Collections.Generic;
using OscarBot.Models;
using OscarBot.Tables;
using Xunit;
using Xunit.Abstractions;

namespace OscarBotTests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void Test1()
        {
            var builder = new TableBuilder<Movie>();
            builder.AddColumn("Id", "Id");
            builder.AddColumn("Title", "Title", 20);
            builder.AddColumn("Director", "Director");
            builder.AddColumn("Runtime", "Runtime");
            builder.AddColumn("Added", "AddedAt");

            var m1 = new Movie() { Id = "tt390430", Title = "Three Billboards Outside Ebbing, Missouri", Director = "Martin McDonagh", Runtime = "1h 55min", AddedAt = DateTime.Now };
            var m2 = new Movie() { Id = "tt343234", Title = "Blow the Man Down", Director = "Bridget Savage Cole", Runtime = "1h 31min", AddedAt = DateTime.Now };
            var l = new List<Movie>() { m1, m2 };

            var result = builder.Build(l);

            Assert.NotNull(result);

            //var lines = result.Split(Environment.NewLine);

            //Assert.Equal(4, lines.Length);

            //var count = lines[0].Length;

            //foreach (var line in lines)
            //{
            //    Assert.Equal(count, line.Length);
            //}

            //output.WriteLine(result);
        }
    }
}
