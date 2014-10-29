using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ImageResizer.Configuration;
using ImageResizer.Resizing;

namespace ImageResizer.Plugins.ScalePercentage
{
    public class ScalePercentagePlugin : BuilderExtension, IPlugin, IQuerystringPlugin
    {
        private const string PluginName = @"scalepercentage";

        public IPlugin Install(Config c)
        {
            c.Plugins.add_plugin(this);
            return this;
        }

        protected override RequestedAction PostDecodeStream(ref Bitmap b, ResizeSettings settings)
        {
            var scalePercentageQuery = settings[PluginName];
            if (string.IsNullOrEmpty(scalePercentageQuery))
            {
                return RequestedAction.None;
            }

            double scaling = 0;
            try
            {
                int scalePercentage = int.Parse(scalePercentageQuery);
                if (scalePercentage < 1 || scalePercentage >= 100)
                {
                    return RequestedAction.None;
                }
                scaling = (double)100/scalePercentage;
            }
            catch
            {
                return RequestedAction.None;
            }
            var settingsString = string.Format("width={0}", (int)(b.Width / scaling));
            var stream = new MemoryStream();
            ImageBuilder.Current.Build(b, stream, new ResizeSettings(settingsString));
            b = new Bitmap(stream);
            return RequestedAction.None;
        }

      

        public bool Uninstall(Config c)
        {
            c.Plugins.remove_plugin(this);
            return true;
        }

        public IEnumerable<string> GetSupportedQuerystringKeys()
        {
            return new string[] { PluginName };
        }
    }
}
