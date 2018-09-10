using System.Text;

namespace SOMLibrary.DataModel
{
    public class Instance
    {
        public int OrderNo { get; set; }
        public object[] Values { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach(var item in Values)
            {
                sb.Append(item.ToString());
                sb.Append(" ");
            }  

            return sb.ToString().TrimEnd();
        }
    }
}