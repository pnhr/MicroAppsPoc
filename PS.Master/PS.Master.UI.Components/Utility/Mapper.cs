using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.UI.Components.Utility
{
    internal static class Mapper
    {
        public static Variant GetVariant(this PsVariant uIVariant)
        {
            Variant item;
            switch(uIVariant)
            {
                case PsVariant.Outlined:
                    item = Variant.Outlined;
                    break;
                case PsVariant.Text:
                    item = Variant.Text;
                    break;
                case PsVariant.Filled:
                    item = Variant.Filled;
                    break;
                default:
                    item = Variant.Outlined;
                    break;
            }
            return item;
        }

        public static Margin GetMargin(this PsMargin uiMargin)
        {
            Margin item;
            switch (uiMargin)
            {
                case PsMargin.None:
                    item = Margin.None;
                    break;
                case PsMargin.Normal:
                    item = Margin.Normal;
                    break;
                case PsMargin.Dense:
                    item = Margin.Dense;
                    break;
                default:
                    item = Margin.Dense;
                    break;
            }
            return item;
        }

        public static Color GetColor(this PsColor uiColor)
        {
            Color item;
            switch (uiColor)
            {
                case PsColor.Primary:
                    item = Color.Primary;
                    break;
                case PsColor.Secondary:
                    item = Color.Secondary;
                    break;
                case PsColor.Tertiary:
                    item = Color.Tertiary;
                    break;
                case PsColor.Info:
                    item = Color.Info;
                    break;
                case PsColor.Success:
                    item = Color.Success;
                    break;
                case PsColor.Warning:
                    item = Color.Warning;
                    break;
                case PsColor.Error:
                    item = Color.Error;
                    break;
                case PsColor.Dark:
                    item = Color.Dark;
                    break;
                case PsColor.Transparent:
                    item = Color.Transparent;
                    break;
                case PsColor.Inherit:
                    item = Color.Inherit;
                    break;
                case PsColor.Surface:
                    item = Color.Surface;
                    break;
                default:
                    item = Color.Default;
                    break;
            }
            return item;
        }
    }
}
