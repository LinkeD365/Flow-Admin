﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace LinkeD365.FlowAdmin
{
    public class FlowDefinition
    {
        public bool Solution;
        public string Id;
        public string Definition;
        private string plan;

        public string Name { get; set; }
        public string Description { get; set; }
        public bool Managed { get; set; }
        public string OwnerType { get; set; }

        [Browsable(false)]
        public bool LogicApp { get; internal set; }

        [Browsable(false)]
        public string UniqueId { get; set; }

        public string Status { get; internal set; }

        public DateTime CreatedOn { get; set; }

        public DateTime Modified { get; set; }

        [Browsable(false)]
        public string Type { get; set; }

        [Browsable(false)]
        public string Plan
        {
            get
            {
                switch (plan)
                {
                    case "Owner":
                        return "User who runs the flow";

                    default:
                        return plan;
                }
            }
            internal set => plan = value;
        }

        [Browsable(false)]
        public Guid Guid
        { get { return Guid.Parse(Id); } }

        [Browsable(false)]
        public string OwnerId { get; internal set; }

        [Browsable(false)]
        public string AzureOwnerId { get; internal set; }

        //public List<Comment> Comments = new List<Comment>();
    }

    internal class FlowDefComparer : IComparer<FlowDefinition>
    {
        private string memberName = string.Empty; // specifies the member name to be sorted
        private SortOrder sortOrder = SortOrder.None; // Specifies the SortOrder.

        public FlowDefComparer(string strMemberName, SortOrder sortingOrder)
        {
            memberName = strMemberName;
            sortOrder = sortingOrder;
        }

        public int Compare(FlowDefinition flow1, FlowDefinition flow2)
        {
            int returnValue = 1;
            switch (memberName)
            {
                case "Name":
                    if (sortOrder == SortOrder.Ascending)
                    {
                        returnValue = flow1.Name.CompareTo(flow2.Name);
                    }
                    else
                    {
                        returnValue = flow2.Name.CompareTo(flow1.Name);
                    }

                    break;

                case "Description":
                    if (sortOrder == SortOrder.Ascending)
                    {
                        returnValue = flow1.Description.CompareTo(flow2.Description);
                    }
                    else
                    {
                        returnValue = flow2.Description.CompareTo(flow1.Description);
                    }

                    break;

                case "Managed":
                    if (sortOrder == SortOrder.Ascending)
                    {
                        returnValue = flow1.Managed.CompareTo(flow2.Managed);
                    }
                    else
                    {
                        returnValue = flow2.Managed.CompareTo(flow1.Managed);
                    }
                    break;

                default:
                    if (sortOrder == SortOrder.Ascending)
                    {
                        returnValue = flow1.Name.CompareTo(flow2.Name);
                    }
                    else
                    {
                        returnValue = flow2.Name.CompareTo(flow1.Name);
                    }
                    break;
            }
            return returnValue;
        }
    }
}