using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LectureMe.Model
{
    public class Emotion
    {
        public decimal anger { get; set; }
        public decimal contempt { get; set; }
        public decimal disgust { get; set; }
        public decimal fear { get; set; }
        public decimal happiness { get; set; }
        public decimal neutral { get; set; }
        public decimal sadness { get; set; }
        public decimal surprise { get; set; }

        public static implicit operator Emotion(Task<Emotion> v)
        {
            throw new NotImplementedException();
        }
    }
}