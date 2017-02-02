using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Discord;
using Discord.Commands;

namespace Funbot
{
    public class ImageGenerator
    {
        WebClient webClient = new WebClient();
        Image defaultImage;
        Image vs2Image;

        public ImageGenerator()
        {
            webClient = new WebClient();
            defaultImage = Image.FromFile("defaultAvatar.png");
            vs2Image = Image.FromFile("2v2.png");
        }

        ~ImageGenerator()
        {
            webClient.Dispose();
            defaultImage.Dispose();
            vs2Image.Dispose();
        }

        public MemoryStream GenerateVs2Image(User a, User b)
        {
            MemoryStream stream = new MemoryStream(1024);
            Image imageA = null, imageB = null;

            try
            {
                if(a.AvatarUrl == null)
                {
                    imageA = defaultImage;
                }
                else
                {
                    imageA = getImageFromUrl(a.AvatarUrl);
                }

                if (b.AvatarUrl == null)
                {
                    imageB = defaultImage;
                }
                else
                {
                    imageB = getImageFromUrl(b.AvatarUrl);
                }

                using(Bitmap image = new Bitmap(256, 128))
                {
                    using (Graphics graphic = Graphics.FromImage(image))
                    {
                        graphic.DrawImage(imageA, 0, 0, 128, 128);
                        graphic.DrawImage(imageB, 128, 0, 128, 128);
                        graphic.DrawImage(vs2Image, 0, 0, 256, 128);
                    }

                    image.Save(stream, ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            finally
            {
                if (imageA != defaultImage)
                    imageA.Dispose();

                if (imageB != defaultImage)
                    imageB.Dispose();
            }
            
            return stream;
        }

        public Image getImageFromUrl(string url)
        {
            byte[] data = webClient.DownloadData(new Uri(url));

            return Image.FromStream(new MemoryStream(data));
        }

        [Command("vs", "Génère un image cool de toi et ton opposant")]
        [Parameter("other", ParameterType.Required)]
        public void genereateVsImage(CommandEventArgs args)
        {
            ulong otherId = 0;
            if (Bot.TryGetIdFromMention(args.GetArg("other"), ref otherId))
            {
                args.Channel.SendIsTyping();
                User other = args.Channel.GetUser(otherId);
                args.Channel.SendFile(args.User.Name + "vs" + other.Name + ".png", GenerateVs2Image(args.User, other));
            }
            else
            {
                args.Channel.SendMessage("Utilisateur invalide.");
            }
        }
    }
}
