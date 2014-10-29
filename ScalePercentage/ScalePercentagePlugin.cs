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
            var scalePercentage = settings[PluginName];
            if (string.IsNullOrEmpty(scalePercentage))
            {
                return RequestedAction.None;
            }

            int scaling = 0;
            try
            {
                scaling = 100/int.Parse(scalePercentage);
                if (scaling >= 1)
                {
                    return RequestedAction.None;
                }
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
