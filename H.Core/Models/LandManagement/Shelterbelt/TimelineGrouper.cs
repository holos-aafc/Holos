using System;
using Prism.Mvvm;

namespace H.Core.Models.LandManagement.Shelterbelt
{
    public class TimelineGrouper : BindableBase
    {
        #region Constructors

        public TimelineGrouper()
        {
            _display = "TimelineGrouper - " + _id;
        }

        #endregion

        #region Fields

        private Guid _id;
        private string _display;

        #endregion

        #region Properties

        public Guid ID
        {
            get { return _id; }
            set { this.SetProperty(ref _id, value); }
        }

        public string Display
        {
            get { return _display; }
            set { this.SetProperty(ref _display, value); }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return _display;
        }

        public override bool Equals(object obj)
        {
            //RadTimeline Appears to use a combination of GetHashCode and .Equals to decide if these guys are equal
            if (obj is TimelineGrouper grouper)
            {
                return _id == grouper._id;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //RadTimeline Appears to use a combination of GetHashCode and .Equals to decide if these guys are equal
            try
            {
                var a = _id.ToByteArray();
                uint number = 0;
                var i = 0;
                foreach (var b in a)
                {
                    if (i == 8) //guid must fit return type
                    {
                        i = 0;
                    }

                    number ^= Convert.ToUInt32(b) << (i * 8);
                }

                var room = Convert.ToInt64(number) - Convert.ToInt64(int.MaxValue);
                return Convert.ToInt32(room);
            }
            catch (Exception)
            {
                return base.GetHashCode();
            }
        }

        #endregion

        #region Operator Overloads

        //Could try ovveriding Object.Equals too, whatever is necessary to not have to write name collision code.

        public static bool operator ==(TimelineGrouper a, TimelineGrouper b)
        {
            return a.ID == b.ID;
        }

        public static bool operator !=(TimelineGrouper a, TimelineGrouper b)
        {
            return a.ID != b.ID;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}