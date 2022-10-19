namespace tx_signature_nofields.Models {
   public class Location {
      public double x { get; set; }
      public double y { get; set; }
   }

   public class Pen {
      public string type { get; set; }
      public double objectWidth { get; set; }
      public double objectHeight { get; set; }
   }

   public class Annotation {
      public Pen pen { get; set; }
      public string user { get; set; }
      public Location location { get; set; }
      public long time { get; set; }
      public List<Comment> comments { get; set; }
      public string id { get; set; }
      public string image { get; set; }
   }

   public class Comment {
      public string comment { get; set; }
      public List<string> user { get; set; }
      public long date { get; set; }
      public string id { get; set; }
      public string status { get; set; }
   }
}

