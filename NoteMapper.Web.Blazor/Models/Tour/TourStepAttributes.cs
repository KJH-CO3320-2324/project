using System.Collections;

namespace NoteMapper.Web.Blazor.Models.Tour
{
    public class TourStepAttributes : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly IDictionary<string, object> _attributes = new Dictionary<string, object>();

        public TourStepAttributes(int step, string title, string message,
            bool hideArrow = false)
        {
            _attributes.Add("data-nm-tour-step", step);
            _attributes.Add("data-nm-tour-step-title", title);
            _attributes.Add("data-nm-tour-step-message", message);
            
            if (hideArrow)
            {
                _attributes.Add("data-nm-tour-step-arrow", (!hideArrow).ToString().ToLower());
            }            

            Step = step;
            Title = title;
        }

        public int Step { get; }

        public string Title { get; }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
