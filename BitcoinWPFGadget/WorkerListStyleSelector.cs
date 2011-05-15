﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace BitcoinWPFGadget
{
    public class WorkerListStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item,
            DependencyObject container)
        {
            Style st = new Style();
            st.TargetType = typeof(ListViewItem);

            Setter backGroundSetter = new Setter();

            backGroundSetter.Property = ListViewItem.BackgroundProperty;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(container) as ListView;

            int index = listView.ItemContainerGenerator.IndexFromContainer(container);

            BTCMine.Miner worker = (listView.Items[index] as BTCMine.Miner);

            backGroundSetter.Value = worker.onlineStatusColor;

            st.Setters.Add(backGroundSetter);

            return st;
        }
    }

}