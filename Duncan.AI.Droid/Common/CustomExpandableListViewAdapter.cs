
#if _included_

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;



namespace Duncan.AI.Droid.Common
{
    public class CustomExpandableDrawerAdapter : BaseExpandableListAdapter
    {

        private static const int CAMPAIGNS = 0;
        private static const int CONVERSIONS = 1;
        private static const int MYACCOUNT = 2;

        private LayoutInflater mInflater;
        private String[] mainNavItems, campaignNavItems, myAccountNavItems;

        public CustomExpandableDrawerAdapter(Activity context)
        {
            this.mInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            this.mainNavItems = context.Resources.GetStringArray(Resource.Id.main_nav_items);
            this.campaignNavItems = context.Resources.GetStringArray(Resource.Id.campaigns_nav_items);
            this.myAccountNavItems = context.Resources.GetStringArray(Resource.Id.my_account_nav_items);
        }

        public Object getChild(int groupPosition, int childPosition)
        {

            switch (groupPosition)
            {
                case CAMPAIGNS:
                    return campaignNavItems[childPosition];
                case MYACCOUNT:
                    return myAccountNavItems[childPosition];
                default:
                    return "";
            }
        }

        public long getChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public View getChildView(int groupPosition, int childPosition,
                                 bool isLastChild, View convertView, ViewGroup parent)
        {

            if (convertView == null)
            {
                convertView = mInflater.Inflate(Resource.Id.drawer_list_item_expandable, null);
            }

            TextView childText = (TextView)convertView.FindViewById(Resource.Id.drawer_child_list_item_text);
            childText.Text = ((String)getChild(groupPosition, childPosition));

            return convertView;
        }

        public int getChildrenCount(int groupPosition)
        {
            switch (groupPosition)
            {
                case CAMPAIGNS:
                    //Constants.logMessage("Children for group position: " + groupPosition + " are: " + campaignNavItems.length);
                    return campaignNavItems.Length;

                case CONVERSIONS:
                    //Constants.logMessage("Children for group position: " + groupPosition + " are: " + 0);
                    return 0;

                case MYACCOUNT:
                    //Constants.logMessage("Children for group position: " + groupPosition + " are: " + myAccountNavItems.length);
                    return myAccountNavItems.Length;

                default:
                    return 0;
            }
        }

        public Object getGroup(int groupPosition)
        {
            return mainNavItems[groupPosition];
        }

        public int getGroupCount()
        {
            return mainNavItems.Length;
        }

        public long getGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public View getGroupView(int groupPosition, bool isExpanded,
                                 View convertView, ViewGroup parent)
        {

            if (convertView == null)
            {
                convertView = mInflater.Inflate(Resource.Id.drawer_list_item_expandable,
                        null);
            }

            TextView groupText = (TextView)convertView.FindViewById(R.id.drawer_list_item_text);
            groupText.Text = ((String)getGroup(groupPosition));
            return convertView;
        }

        public bool hasStableIds()
        {
            return true;
        }

        public bool isChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}

#endif
