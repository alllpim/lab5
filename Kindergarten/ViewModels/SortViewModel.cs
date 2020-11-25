using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kindergarten.ViewModels
{
    public enum SortState
    {
        // Groups

        GroupNameAsc,
        GroupNameDesc,
        GroupStaffAsc,
        GroupStaffDesc,
        GroupCountAsc,
        GroupCountDesc,
        GroupYearAsc,
        GroupYearDesc,
        GroupTypeAsc,
        GroupTypeDesc,

        // GROUP TYPES
        GtNameAsc,
        GtNameDesc,
        GtNoteAsc,
        GtNoteDesc,

        // STAFF
        StaffNameAsc,
        StaffNameDesc,
        StaffAdressAsc,
        StaffAdressDesc,
        StaffPhoneAsc,
        StaffPhoneDesc,
        StaffPosAsc,
        StaffPosDesc,
        StaffInfoAsc,
        StaffInfoDesc,
        StaffRewardAsc,
        StaffRewardDesc,

        // POSITIONS
        PosNameAsc,
        PosNameDesc,
    }
    public class SortViewModel
    {
        // Groups

        public SortState GroupName { get; set; }
        public SortState GroupStaff { get; set; }
        public SortState GroupCount { get; set; }
        public SortState GroupYear { get; set; }
        public SortState GroupType { get; set; }

        // GROUP TYPES
        public SortState GtName { get; set; }
        public SortState GtNote { get; set; }

        // STAFF
        public SortState StaffName { get; set; }
        public SortState StaffAdress { get; set; }
        public SortState StaffPhone { get; set; }
        public SortState StaffPos { get; set; }
        public SortState StaffInfo { get; set; }
        public SortState StaffReward { get; set; }

        // POSITIONS
        public SortState PosName { get; set; }


        public SortState CurrentState { get; set; }

        public SortViewModel(SortState state)
        {
            GroupName = state == SortState.GroupNameAsc ? SortState.GroupNameDesc : SortState.GroupNameAsc;
            GroupStaff = state == SortState.GroupStaffAsc ? SortState.GroupStaffDesc : SortState.GroupStaffAsc;
            GroupCount = state == SortState.GroupCountAsc ? SortState.GroupCountDesc : SortState.GroupCountAsc;
            GroupYear = state == SortState.GroupYearAsc ? SortState.GroupYearDesc : SortState.GroupYearAsc;
            GroupType = state == SortState.GroupTypeAsc ? SortState.GroupTypeDesc : SortState.GroupTypeAsc;


            GtName = state == SortState.GtNameAsc ? SortState.GtNameDesc : SortState.GtNameAsc;
            GtNote = state == SortState.GtNoteAsc ? SortState.GtNoteDesc : SortState.GtNoteAsc;


            StaffName = state == SortState.StaffNameAsc ? SortState.StaffNameDesc : SortState.StaffNameAsc;
            StaffAdress = state == SortState.StaffAdressAsc ? SortState.StaffAdressDesc : SortState.StaffAdressAsc;
            StaffPhone = state == SortState.StaffPhoneAsc ? SortState.StaffPhoneDesc : SortState.StaffPhoneAsc;
            StaffPos = state == SortState.StaffPosAsc ? SortState.StaffPosDesc : SortState.StaffPosAsc;
            StaffInfo = state == SortState.StaffInfoAsc ? SortState.StaffInfoDesc : SortState.StaffInfoAsc;
            StaffReward = state == SortState.StaffRewardAsc ? SortState.StaffRewardDesc : SortState.StaffRewardAsc;


            PosName = state == SortState.PosNameAsc ? SortState.PosNameDesc : SortState.PosNameAsc;

            CurrentState = state;
        }
    }
}
