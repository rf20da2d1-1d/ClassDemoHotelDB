using System;
using System.Collections.Generic;
using System.Text;

namespace HotelModelLib.model
{
    public class Guest
    {
        private int _id;
        private string _name;
        private string _address;

        public Guest()
        {
        }

        public Guest(int id, string name, string address)
        {
            _id = id;
            _name = name;
            _address = address;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Address
        {
            get => _address;
            set => _address = value;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Address)}: {Address}";
        }
    }
}
