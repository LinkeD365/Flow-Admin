using System;
using System.ComponentModel;

namespace LinkeD365.FlowAdmin
{
    public class FlowOwner
    {
        [Browsable(false)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Email { get; internal set; }
        public string Principle { get; internal set; }

        [Browsable(false)]
        public Guid SysId { get; set; }
    }
}