using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OscarBot.Tables
{
    public class ColumnConfig
    {
        public string Header { get; set; }
        public string Field { get; set; }
        public int Length { get; set; }
        public int? MaxLength { get; set; }
    }
    public class TableBuilder<T>
    {
        public List<ColumnConfig> Columns { get; set; }
        public int MaxPageLength { get; set; }

        public TableBuilder()
        {
            this.Columns = new List<ColumnConfig>();
        }

        public void AddColumn(string header, string field, int? maxlength = null)
        {
            this.Columns.Add(new ColumnConfig
            {
                Header = header,
                Field = field,
                MaxLength = maxlength
            });
        }

        public List<string> Build(List<T> input)
        {
            var json = JsonConvert.SerializeObject(input);
            var dynamiclist = JsonConvert.DeserializeObject<List<dynamic>>(json);

            var pagestrings = new List<string>();



            var header = "|";
            foreach (var c in this.Columns)
            {
                header += " ";
                c.Length = c.MaxLength == null ? dynamiclist.Max(x => x[c.Field].ToString().Length) : c.MaxLength;
                header += c.Header.PadRight(c.Length + 1);
                header += "|";
            }


            var totalWidth = header.Length;
            var headerSize = totalWidth * 2;
            var itemsPerPage = (int)Math.Floor((MaxPageLength - headerSize) / (decimal)totalWidth);
            var pages = (int)Math.Ceiling(dynamiclist.Count() / (decimal) itemsPerPage);

            for (int i = 0; i < pages; i++)
            {
                var sb = new StringBuilder();
                sb.AppendLine(header);
                var pageitems = dynamiclist.Skip(itemsPerPage * i).Take(itemsPerPage);
                sb.AppendLine("".PadRight(totalWidth, '-'));
                foreach (var item in pageitems)
                {
                    string s = "|";
                    foreach (var c in this.Columns)
                    {
                        s += " " + GetValue(item[c.Field], c.Length).PadRight(c.Length) + " |";
                    }

                    sb.AppendLine(s);
                }
                pagestrings.Add(sb.ToString().TrimEnd());
            }



            return pagestrings;

        }

        private string GetValue(JValue input, int len)
        {
            string val;
            if (input.Type == JTokenType.Date)
            {
                val = ((DateTime)input).ToString("dd-MM-yy HH:mm");
            }
            else
                val = input.ToString();

            if (val.Length <= len) return val;

            return val.Substring(0, (len - 3)) + "...";
        }
    }
}
